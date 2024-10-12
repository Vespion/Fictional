using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace Fictional.App.ConsoleApp.Binders;

/// <summary>
/// Binds to an item inside the application's service provider
/// </summary>
/// <typeparam name="TService">The type of service to bind</typeparam>
internal class ServiceTypeBinder<TService> : BinderBase<TService> where TService : notnull
{
	/// <inheritdoc />
	protected override TService GetBoundValue(BindingContext bindingContext)
	{
		return bindingContext.GetRequiredService<TService>();
	}
}