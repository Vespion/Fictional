using System.CommandLine;
using Fictional.App.ConsoleApp.Binders;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp.Commands.Tags.Link;

internal abstract class AddTagLinkCommand: ICommand
{
	public static Command Configure()
	{
		var command = new Command("add", "Adds a tag link")
		{
			CanonicalIdArg,
			AliasIdArg
		};

		command.SetHandler(
			Handle,
			new LoggerBinder<AddTagLinkCommand>(),
			new TagRepositoryBinder(GlobalOptions.TargetExistingDatabaseOption),
			CanonicalIdArg,
			AliasIdArg,
			new CancellationTokenBinder()
		);

		return command;
	}

	internal static async Task<int> Handle(
		ILogger<AddTagLinkCommand> logger,
		ITagRepository repo,
		long canonicalTagId,
		long aliasTagId,
		CancellationToken ctx
	)
	{
		logger.CommandExecutionStarted();

		var added = await repo.AddGraphLinkAsync((TagId)canonicalTagId, (TagId)aliasTagId, ctx);

		if (!added)
		{
			logger.LogWarning("Link already exists");
			logger.CommandExecutionCompleted();
			return 1;
		}

		logger.CommandExecutionCompleted();

		return 0;
	}

	static AddTagLinkCommand()
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
			"ID of the child tag"
		)
		{
			Arity = ArgumentArity.ExactlyOne
		};
	}

	private static Argument<long> CanonicalIdArg { get; }

	private static Argument<long> AliasIdArg { get; }
}