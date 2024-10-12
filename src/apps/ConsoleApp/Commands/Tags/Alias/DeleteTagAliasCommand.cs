using System.CommandLine;
using Fictional.App.ConsoleApp.Binders;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp.Commands.Tags.Alias;

internal abstract class DeleteTagAliasCommand: ICommand
{
	public static Command Configure()
	{
		var command = new Command("delete", "Deletes a tag alias")
		{
			CanonicalIdArg,
			AliasIdArg
		};

		command.SetHandler(
			Handle,
			new LoggerBinder<DeleteTagAliasCommand>(),
			new TagRepositoryBinder(GlobalOptions.TargetExistingDatabaseOption),
			CanonicalIdArg,
			AliasIdArg,
			new CancellationTokenBinder()
		);

		return command;
	}

	internal static async Task<int> Handle(
		ILogger<DeleteTagAliasCommand> logger,
		ITagRepository repo,
		long canonicalTagId,
		long aliasTagId,
		CancellationToken ctx
	)
	{
		logger.CommandExecutionStarted();

		await repo.RemoveAliasLinkAsync((TagId)canonicalTagId, (TagId)aliasTagId, ctx);

		logger.CommandExecutionCompleted();

		return 0;
	}

	static DeleteTagAliasCommand()
	{
		CanonicalIdArg = new Argument<long>(
			"Canonical Id",
			"ID of the canonical tag"
		)
		{
			Arity = ArgumentArity.ExactlyOne
		};

		AliasIdArg = new Argument<long>(
			"Alias Id",
			"ID of the alias tag"
		)
		{
			Arity = ArgumentArity.ExactlyOne
		};
	}

	private static Argument<long> CanonicalIdArg { get; }

	private static Argument<long> AliasIdArg { get; }
}