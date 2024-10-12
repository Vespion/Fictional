using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;

namespace Fictional.App.ConsoleApp.Binders;

internal class CancellationTokenBinder: BinderBase<CancellationToken>
{
	protected override CancellationToken GetBoundValue(BindingContext bindingContext)
	{
		return bindingContext.GetRequiredService<InvocationContext>().GetCancellationToken();
	}
}