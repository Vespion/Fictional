using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Fictional.TestUtils.Logging;

/// <inheritdoc />
public class XunitLoggingProvider: ILoggerProvider
{
	/// <summary>
	/// The registered loggers.
	/// </summary>
	private readonly ConcurrentDictionary<string, XunitLogger> _loggers = new();

	/// <inheritdoc />
	public void Dispose()
	{
		_loggers.Clear();
	}

	/// <inheritdoc />
	public ILogger CreateLogger(string categoryName)
	{
		return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
	}

	private XunitLogger CreateLoggerImplementation(string categoryName)
	{
		return new XunitLogger(categoryName);
	}
}

/// <inheritdoc />
/// <remarks>
/// This logger will log all messages to the test output, and will also add warnings to the test context.
/// </remarks>
public class XunitLogger(string name): ILogger
{
	/// <inheritdoc />
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (logLevel >= LogLevel.Warning)
		{
			TestContext.Current.AddWarning(formatter(state, exception));
		}

		TestContext.Current.TestOutputHelper?.WriteLine($"{DateTime.Now:O} [{logLevel:G}] {name} {(eventId != default ? $"({eventId})" : "")} - {formatter(state, exception)}");
	}

	/// <inheritdoc />
	/// <remarks>Always returns true</remarks>
	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	/// <inheritdoc />
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return null;
	}
}