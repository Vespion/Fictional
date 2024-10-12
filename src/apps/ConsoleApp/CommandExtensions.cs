using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp;

internal static partial class CommandExtensions
{
	[LoggerMessage(LogLevel.Debug, "Command execution started")]
	internal static partial void CommandExecutionStarted(this ILogger logger);

	[LoggerMessage(LogLevel.Debug, "Extended command parsing completed")]
	internal static partial void CommandParsingCompleted(this ILogger logger);

	[LoggerMessage(LogLevel.Debug, "Command execution completed")]
	internal static partial void CommandExecutionCompleted(this ILogger logger);

	[LoggerMessage(LogLevel.Debug, "Command execution failed")]
	internal static partial void CommandExecutionFailed(this ILogger logger, Exception ex);
}