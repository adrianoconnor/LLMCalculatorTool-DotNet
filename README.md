# LLM Calculator Tool (dotnet version)

This tiny project gives you a tool that is designed for use with LLMs, to help reduce the risk of hallucinating inaccurate answers when arithmetic is involved.

The calculator works by taking an expression (sum) as a string, which of course would be provided by the LLM as a parameter to a 'Tool Call' if the LLM decides that it needs the help of a calculator. The string is parsed, first into tokens and then into a simple AST, and it implements the 'Evaluate' pattern to calculate the result, which is also returned as a string (ready for directly adding to the chat history as a tool result).

We try to follow standard calculator conventions in terms of decimal places and formatting, but calculators expose a lot of edge cases. Internally it uses decimal type everywhere, which might not be the best choice. We might switch to either BigDecimal (sadly not natively in the dotnet standard library) or even just double at some point in the future, but for now I'm mostly happy with the accuracy and rounding behaviour.

To use it simply, just call like this:

```csharp
result = LlmCalculator.Calculate("4 + 3 * 2");
```

In this example, result will of course be the string value "10".

To use it as a tool with the OpenAI library for dotnet, you should read the documentation in the git repo on tools and function calling (https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-use-chat-completions-with-tools-and-function-calling) to understand the flow, and then register your tool like this:

```csharp
ChatTool llmCalculatorTool = ChatTool.CreateFunctionTool(
    functionName: "LLM_Calculator",
    functionDescription: "A calculator that can perform basic arithmetic operations for use by LLMs",
    functionParameters: BinaryData.FromBytes("""
        {
            "type": "object",
            "properties": {
                "expression": {
                    "type": "string",
                    "description": "An expression to calculate, e.g. '2 * (-3 + 4) / 3'. You can call sin, cos and tan functions (including asin, sinh and asinh etc) if required, but note that they always work in radians so you might need to include a suitable conversion from degrees. You can use constants PI, E, LN2, LOG2E, LOG10E, SQRT1_2 and SQRT2 if required.",
                    "example": "2.6 * (3.6 + 4.2)"
                }
            }
        }
        """u8.ToArray())
);
```

In your chat completion loop respond to the tool call something like this:

```csharp
switch (completion.FinishReason)
{
    ...
    case ChatFinishReason.ToolCalls:
    {
        messages.Add(new AssistantChatMessage(completion));

        foreach (ChatToolCall toolCall in completion.ToolCalls)
        {
            switch (toolCall.FunctionName)
            {
                case "LLM_Calculator":
                    {
                        var expression = toolCall.FunctionArguments.ToObjectFromJson<CalculatorToolRequestParameters>()!.Expression;

                        var result = LlmCalculator.Calculate(expression);

                        messages.Add(new ToolChatMessage(toolCall.Id, result));
                        break;
                    }

```

In this example I've got a simple record type called CalculatorToolRequestParameters for deserializing the JSON from the LLM:

```csharp
private record CalculatorToolRequestParameters
{
    [JsonPropertyName("expression")]
    public required string Expression { get; set; }
}
```

There's an example file in the unit tests showing how it works if you need a working example (you just need to make sure you have OPENAI_API_KEY set as an environment variable).

For the sum we're using as a test in our unit test (0.1 + 46,000 + 1,000,000 / 2.5 + 0.1 + 0.2), GPT 3.5-turbo gets it wrong 100% of the time. GPT 4 and 4.1 get it right about 75% of the time, and that goes up if you ask the LLM to explain its reasoning -- but then of course the chat is full of unwanted LLM chatter, which we typically want to avoid.

Registering this simple calculator as a tool takes it to 100% accuracy with no additional yap 100% of the time for all 3 models (in my basic and limited testing).

Please note that this is a personal side project, there is no commercial support or guarantee. If you find any issues please do reach out and I'll see what I can do.

---

Copyright (c) 2025 Adrian O'Connor

Made available under the MIT License, please see the LICENSE file in this folder