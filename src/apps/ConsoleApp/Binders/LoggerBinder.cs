using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp.Binders;

/// <summary>
/// Binds to an <see cref="ILogger"/> instance from the factory in the application's service provider
/// </summary>
/// <typeparam name="TLogger">The type of logger to bind to</typeparam>
internal class LoggerBinder<TLogger>: BinderBase<ILogger<TLogger>>
{
	/// <inheritdoc />
	protected override ILogger<TLogger> GetBoundValue(BindingContext bindingContext)
	{
		var factory = bindingContext.GetRequiredService<ILoggerFactory>();

		return factory.CreateLogger<TLogger>();
	}

	internal ILogger<TLogger> GetValue(BindingContext bindingContext)
	{
		return GetBoundValue(bindingContext);
	}
}