

# Una Calculadora para tu Agente de Chat

Este pequeño proyecto te proporciona una herramienta diseñada para su uso con LLMs, con el fin de ayudar a reducir el riesgo de que alucinen respuestas inexactas cuando se requieran números y operaciones aritméticas.

Puedes ejecutar fácilmente esta calculadora como un servidor MCP, o invocarla desde la biblioteca de dotnet de OpenAI (o Semantic Kernel/Agent Framework/etc.) como una herramienta de función.

La calculadora funciona tomando una expresión (suma) como una cadena de texto, la cual, por supuesto, sería proporcionada por el LLM como parámetro a una 'Llamada a Herramienta' (Tool Call) si decide que necesita la ayuda de una calculadora. La cadena se analiza primero en tokens y luego en un AST simple, e implementa el patrón 'Evaluar' (Evaluate) para calcular el resultado, el cual también se devuelve como una cadena (lista para agregar directamente al historial de chat como resultado de la herramienta).

Intentamos seguir las convenciones estándar de las calculadoras en cuanto a decimales y formato, pero las calculadoras presentan muchos casos extremos. Internamente utiliza el tipo `decimal` en todas partes, lo cual quizás no sea la mejor opción. En el futuro podríamos cambiar a BigDecimal (lamentablemente no está nativo en la biblioteca estándar de dotnet) o incluso simplemente a `double`, pero por ahora estoy bastante satisfecho con la precisión y el comportamiento de redondeo.

Para usarla en el código, simplemente llama al método estático `Calculate` de la siguiente manera:

```csharp
result = LlmCalculator.Calculate("4 + 3 * 2");
```

Esto devuelve el resultado en claro como una cadena, por ejemplo, "10" en este caso.

Ejecutar el proyecto inicia el servidor MCP en stdio. Utiliza la siguiente configuración de MCP como ejemplo de cómo lanzar directamente un servidor MCP de dotnet (por supuesto, necesitas tener instalado el SDK de dotnet):

```json
{
    "inputs": [],
    "servers": {
        "LLMCalcMCP": {
            "type": "stdio",
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "/path/to/LLM_CalcDotNet/src/LLM_Calc/LLM_Calc.csproj"
            ]
        }
    }
}
```


Para usarla como herramienta con la biblioteca de OpenAI para dotnet, debes leer la documentación del repositorio de git sobre herramientas y llamadas a funciones (https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-use-chat-completions-with-tools-and-function-calling) para entender el flujo, y luego registrar tu herramienta de la siguiente manera:

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

En tu bucle de finalización de chat, responde a la llamada de herramienta de la siguiente manera:

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

En este ejemplo, he creado un tipo `record` simple llamado `CalculatorToolRequestParameters` para deserializar el JSON del LLM:

```csharp
private record CalculatorToolRequestParameters
{
    [JsonPropertyName("expression")]
    public required string Expression { get; set; }
}
```

Hay un archivo de ejemplo en las pruebas unitarias que muestra cómo funciona si necesitas un ejemplo funcional (solo asegúrate de tener `OPENAI_API_KEY` configurado como una variable de entorno).

Para la suma que estamos usando como prueba en nuestras pruebas unitarias (0.1 + 46,000 + 1,000,000 / 2.5 + 0.1 + 0.2), GPT 3.5-turbo se equivoca el 100% de las veces. GPT 4 y 4.1 lo aciertan aproximadamente el 75% de las veces, y esa cifra aumenta si le pides al LLM que explique su razonamiento; pero, por supuesto, entonces el chat se llena de charla innecesaria del LLM, que normalmente queremos evitar.

Registrar esta calculadora simple como herramienta la lleva a una precisión del 100% sin charla adicional el 100% de las veces para los 3 modelos (en mis pruebas básicas y limitadas).

Ten en cuenta que en 2026, muchos agentes de programación simplemente ejecutarán un comando de Python para hacer matemáticas, por lo que en esos casos esta herramienta puede ahorrar unos pocos miles de tokens de razonamiento, pero no cambiará el resultado. Sin embargo, si deseas cálculos para un LLM en un entorno no de desarrollo, esto seguirá funcionando como la seda. Si quieres ejecutar el servidor MCP sobre HTTP, necesitarás cambiar el comando de inicio en `Program.cs`, aunque probablemente en la próxima actualización lo convertiré en una opción de línea de comandos.

Tenga en cuenta que este es un proyecto personal secundario, no hay soporte comercial ni garantía. Si encuentras algún problema, no dudes en contactarme y haré lo posible por ayudarte.

---

Copyright (c) 2026 Adrian O'Connor

Disponible bajo la Licencia MIT, por favor consulta el archivo LICENSE en esta carpeta
