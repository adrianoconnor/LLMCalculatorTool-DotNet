using System.Text.Json.Serialization;
using LLM_Calc;
using OpenAI;
using OpenAI.Chat;

namespace LLM_Calc_Tests;

[TestClass]
public class OpenAiToolUseTest
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private record CalculatorToolRequestParameters
    {
        [JsonPropertyName("expression")]
        public required string Expression { get; set; }
    }


    [TestMethod]
    public async Task TestOpenAiToolUse()
    {
        if (Environment.GetEnvironmentVariable("OPENAI_API_KEY") is null)
        {
            Assert.Inconclusive("Please set the OPENAI_API_KEY environment variable to run this test.");
        }

        var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
        var chatClient = client.GetChatClient("gpt-3.5-turbo");

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

        List<ChatMessage> messages = [
            new UserChatMessage("What is 0.1 + 46,000 + 1,000,000 / 2.5 + 0.1 + 0.2? Please give just the answer as a number, no other output is required."),
        ];

        ChatCompletionOptions options = new()
        {
            Tools = { llmCalculatorTool },
        };

        var loopCount = 0;
        bool complete = false;

        do
        {
            if (++loopCount > 2)
            {
                throw new Exception("Loop count exceeded 2, something is wrong with the OpenAI API call.");
            }

            ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    {
                        messages.Add(new AssistantChatMessage(completion));

                        complete = true;
                        break;
                    }

                case ChatFinishReason.ToolCalls:
                    {
                        // First, add the assistant message with tool calls to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));

                        // Then, add a new tool message for each tool call that is resolved.
                        foreach (ChatToolCall toolCall in completion.ToolCalls)
                        {
                            switch (toolCall.FunctionName)
                            {
                                case "LLM_Calculator":
                                    {
                                        string expression = toolCall.FunctionArguments.ToObjectFromJson<CalculatorToolRequestParameters>()!.Expression;

                                        Console.WriteLine($"Calling LLM Calc with requested expression: '{expression}'");

                                        string result = LlmCalculator.Calculate(expression);

                                        ToolChatMessage toolResult = new(toolCall.Id, result);
                                        messages.Add(toolResult);
                                        break;
                                    }
                                default:
                                    {
                                        throw new NotImplementedException(toolCall.FunctionName);
                                    }
                            }
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException(completion.FinishReason.ToString());
            }
        } while (!complete);

        // Randomly gets it wrong without the tool, e.g., Final result: **400,046.4**, Final result: **400,000.4**
        // (Update GPT 4 nd 4.1 get it right about 75% of the time, but 3.5-turbo gets it wrong every time)
        Console.WriteLine($"LLM Chat Response: '{messages.Last().Content.Last().Text}'");

        Assert.IsTrue(messages.Last().Content.Last().Text.Contains("446,000.4") || messages.Last().Content.Last().Text.Contains("446000.4"));
    }

}