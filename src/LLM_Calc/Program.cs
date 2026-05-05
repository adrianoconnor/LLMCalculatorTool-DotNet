using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace LLM_Calc;

public static class Program
{
	public static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.Logging.AddConsole(consoleLogOptions =>
		{
		    // Configure all logs to go to stderr
		    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
		});

		builder.Services
		    .AddMcpServer()
		    .WithStdioServerTransport()	    
		    .WithToolsFromAssembly();

		await builder.Build().RunAsync();
	}

	[McpServerToolType]
	public static class CalculatorTool
	{
	    [McpServerTool, Description("A calculator that can perform basic arithmetic operations for use by LLMs")]
	    public static string Calculate([Description("An expression to calculate, e.g. '2 * (-3 + 4) / 3'. You can call sin, cos and tan functions (including asin, sinh and asinh etc) if required, but note that they always work in radians so you might need to include a suitable conversion from degrees. You can use constants PI, E, LN2, LOG2E, LOG10E, SQRT1_2 and SQRT2 if required")] string expression) => LlmCalculator.Calculate(expression);
	}
}