using System.CommandLine;
using Fictional.App.ConsoleApp.Binders;
using Fictional.Data.Core.Models;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp.Commands.Tags;

internal abstract partial class UpdateTagCommand: ICommand
{
	public static Command Configure()
	{
		var command = new Command("update", "Updates a tag")
		{
			IdOption
		};

		command.SetHandler(ctx =>
		{
			var logger = new LoggerBinder<UpdateTagCommand>().GetValue(ctx.BindingContext);
			var repo = new TagRepositoryBinder(GlobalOptions.TargetExistingDatabaseOption).GetValue(ctx.BindingContext);
			var tagId = ctx.ParseResult.GetValueForArgument(IdOption);
			var name = ctx.ParseResult.GetValueForOption(NameOption);
			var shorthand = ctx.ParseResult.GetValueForOption(ShorthandOption);
			var colourString = ctx.ParseResult.GetValueForOption(ColourOption);
			var hidden = ctx.ParseResult.GetValueForOption(HiddenOption);
			var cancellationToken = ctx.GetCancellationToken();

			logger.CommandParsingCompleted();

			return Handle(logger, repo, tagId, name, shorthand, colourString, hidden, cancellationToken);
		});

		return command;
	}

	internal static async Task<int> Handle(
		ILogger<UpdateTagCommand> logger,
		ITagRepository repo,
		long tagId,
		string? name,
		string? shorthand,
		string? colourString,
		bool? hidden,
		CancellationToken ctx
	)
	{
		logger.CommandExecutionStarted();

		var existingTag = await repo.GetByIdAsync((TagId)tagId, ctx);

		if (!existingTag.HasValue)
		{
			CannotUpdateMissingTag(logger, tagId);
			return 1;
		}

		Colour? colour;
		if (colourString == null)
		{
			colour = existingTag.Value.Colour;
		}
		else if (string.IsNullOrWhiteSpace(colourString))
		{
			colour = null;
		}
		else
		{
			colour = Colour.Parse(colourString, null);
		}

		string? shortHand;

		if (shorthand == null)
		{
			shortHand = existingTag.Value.Shorthand;
		}
		else if (string.IsNullOrWhiteSpace(shorthand))
		{
			shortHand = null;
		}
		else
		{
			shortHand = shorthand;
		}

		var newTag = existingTag.Value with
		{
			Name = name ?? existingTag.Value.Name,
			Shorthand = shortHand,
			Colour = colour,
			Hidden = hidden ?? existingTag.Value.Hidden
		};

		try
		{
			await repo.UpdateAsync(newTag, ctx);
		}
		catch (KeyNotFoundException ex)
		{
			logger.CommandExecutionFailed(ex);
			return 1;
		}

		logger.CommandExecutionCompleted();

		return 0;
	}

	static UpdateTagCommand()
	{
		IdOption = new Argument<long>(
			"ID",
			"ID of the tag to delete"
		)
		{
			Arity = ArgumentArity.ExactlyOne
		};

		NameOption = new Option<string?>(
			"--name",
			"New name for the tag"
		)
		{
			Arity = ArgumentArity.ZeroOrOne
		};

		ShorthandOption = new Option<string?>(
			"--shorthand",
			"New shorthand for the tag"
		)
		{
			Arity = ArgumentArity.ZeroOrOne
		};

		ColourOption = new Option<string?>(
			"--colour",
			"New colour for the tag"
		)
		{
			Arity = ArgumentArity.ZeroOrOne
		};

		HiddenOption = new Option<bool?>(
			"--hidden",
			"Whether the tag should be hidden"
		)
		{
			Arity = ArgumentArity.ZeroOrOne
		};

		ColourOption.AddValidator(v =>
		{
			var colourString = v.GetValueOrDefault<string?>();

			if (!Colour.TryParse(colourString, null, out _))
			{
				v.ErrorMessage ="Invalid colour, expected a RGB(A) hex value";
			}
		});
	}

	private static Argument<long> IdOption { get; }

	private static Option<string?> NameOption { get; }

	private static Option<string?> ShorthandOption { get; }

	private static Option<string?> ColourOption { get; }

	private static Option<bool?> HiddenOption { get; }

	[LoggerMessage(LogLevel.Error, "Tag with ID {TagId} not found, unable to update")]
	private static partial void CannotUpdateMissingTag(ILogger<UpdateTagCommand> logger, long tagId);
}