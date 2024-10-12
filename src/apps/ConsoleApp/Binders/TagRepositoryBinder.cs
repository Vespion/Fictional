using System.CommandLine;
using System.CommandLine.Binding;
using Fictional.Data.Core.Repositories;
using Fictional.Data.Core.Repositories.Sql;

namespace Fictional.App.ConsoleApp.Binders;

/// <inheritdoc />
internal class TagRepositoryBinder(Option<DirectoryInfo> directoryOption): BinderBase<ITagRepository>
{
	/// <inheritdoc />
	protected override ITagRepository GetBoundValue(BindingContext bindingContext)
	{
		var database = new DatabaseBinder(directoryOption).GetValue(bindingContext);

		return new SqliteTagRepository(database);
	}

	internal ITagRepository GetValue(BindingContext bindingContext)
	{
		return GetBoundValue(bindingContext);
	}
}