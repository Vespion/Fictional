using System.CommandLine;
using Fictional.App.ConsoleApp.Binders;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp.Commands.Tags.Link;

internal abstract class DeleteTagLinkCommand: ICommand
{
	public static Command Configure()
	{
		var command = new Command("delete", "Deletes a tag link")
		{
			CanonicalIdArg,
			AliasIdArg
		};

		command.SetHandler(
			Handle,
			new LoggerBinder<DeleteTagLinkCommand>(),
			new TagRepositoryBinder(GlobalOptions.TargetExistingDatabaseOption),
			CanonicalIdArg,
			AliasIdArg,
			new CancellationTokenBinder()
		);

		return command;
	}

	internal static async Task<int> Handle(
		ILogger<DeleteTagLinkCommand> logger,
		ITagRepository repo,
		long canonicalTagId,
		long aliasTagId,
		CancellationToken ctx
	)
	{
		logger.CommandExecutionStarted();

		await repo.RemoveGraphLinkAsync((TagId)canonicalTagId, (TagId)aliasTagId, ctx);

		logger.CommandExecutionCompleted();

		return 0;
	}

	static DeleteTagLinkCommand()
	{
		CanonicalIdArg = new Argument<long>(
			"Parent Id",
			"ID of the parent tag"
		)
		{
			Arity = ArgumentArity.ExactlyOne
		};

		AliasIdArg = new Argument<long>(
			"Child Id",
			"ID of the chil tag"
		)
		{
			Arity = ArgumentArity.ExactlyOne
		};
	}

	private static Argument<long> CanonicalIdArg { get; }

	private static Argument<long> AliasIdArg { get; }
}