using System.CommandLine;
using Fictional.App.ConsoleApp.Binders;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp.Commands.Tags;

internal abstract class DeleteTagCommand: ICommand
{
	public static Command Configure()
	{
		var command = new Command("delete", "Deletes a tag")
		{
			IdOption
		};

		command.SetHandler(
			Handle,
			new LoggerBinder<DeleteTagCommand>(),
			new TagRepositoryBinder(GlobalOptions.TargetExistingDatabaseOption),
			IdOption,
			new CancellationTokenBinder()
		);

		return command;
	}

	internal static async Task<int> Handle(ILogger<DeleteTagCommand> logger, ITagRepository repo, long tagId, CancellationToken ctx)
	{
		logger.CommandExecutionStarted();

		await repo.DeleteAsync((TagId)tagId, ctx);

		logger.CommandExecutionCompleted();

		return 0;
	}

	static DeleteTagCommand()
	{
		IdOption = new Option<long>(
			"--id",
			"ID of the tag to delete"
		)
		{
			IsRequired = true
		};
	}

	private static Option<long> IdOption { get; }
}