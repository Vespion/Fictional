using System.CommandLine;
using Fictional.App.ConsoleApp.Binders;
using Fictional.Data.Core.Models;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp.Commands.Tags;

internal abstract class NewTagCommand : ICommand
{
	/// <inheritdoc />
	public static Command Configure()
	{
		var command = new Command("new", "Creates a new tag")
		{
			NameOption,
			ShorthandOption,
			AliasesOption,
			ColourOption,
			HiddenOption,
			ParentTagsOption,
			IdOption
		};

		command.SetHandler(ctx =>
		{
			var logger = new LoggerBinder<NewTagCommand>().GetValue(ctx.BindingContext);
			var name = ctx.ParseResult.GetValueForOption(NameOption)!;
			var shorthand = ctx.ParseResult.GetValueForOption(ShorthandOption);
			var colour = ctx.ParseResult.GetValueForOption(ColourOption);
			var hidden = ctx.ParseResult.GetValueForOption(HiddenOption);
			var parentTagIds = ctx.ParseResult.GetValueForOption(ParentTagsOption) ?? [];
			var aliasTagIds = ctx.ParseResult.GetValueForOption(AliasesOption) ?? [];
			var id = ctx.ParseResult.GetValueForOption(IdOption);
			var cancellationToken = ctx.GetCancellationToken();

			var repo = new TagRepositoryBinder(GlobalOptions.TargetExistingDatabaseOption).GetValue(ctx.BindingContext);

			logger.CommandParsingCompleted();

			return Handle(
				logger,
				repo,
				name,
				shorthand,
				colour,
				hidden,
				parentTagIds,
				aliasTagIds,
				id,
				cancellationToken
			);
		});

		return command;
	}

	static NewTagCommand()
	{
		ShorthandOption = new Option<string>(
			["--shorthand", "-s"],
			"Shorthand of the tag to create"
		)
		{
			IsRequired = false
		};

		ColourOption = new Option<Colour?>(
			["--colour", "-c"],
			parseArgument: result => Colour.Parse(result.GetValueOrDefault<string>(), null),
			false,
			"Colour of the tag to create"
		)
		{
			IsRequired = false
		};

		HiddenOption = new Option<bool>(
			["--hidden", "-h"],
			"Whether the tag should be hidden"
		)
		{
			IsRequired = false
		};

		NameOption = new Option<string>(
			["--name", "-n"],
			"Name of the tag to create"
		)
		{
			IsRequired = true
		};

		AliasesOption = new Option<long[]>(
			["--alias", "-a"],
			"Alias tags of this tag"
		)
		{
			AllowMultipleArgumentsPerToken = true,
			IsRequired = false
		};

		ParentTagsOption = new Option<long[]>(
			["--parent", "-p"],
			"Parent tags of this tag"
		)
		{
			AllowMultipleArgumentsPerToken = true,
			IsRequired = false
		};

		IdOption = new Option<long?>(
			["--id", "-i"],
			"ID of the tag to create"
		)
		{
			IsRequired = false, AllowMultipleArgumentsPerToken = false, Arity = ArgumentArity.ZeroOrOne
		};
	}

	private static Option<long[]> ParentTagsOption { get; }

	private static Option<string> NameOption { get; }

	private static Option<string> ShorthandOption { get; }

	private static Option<Colour?> ColourOption { get; }

	private static Option<bool> HiddenOption { get; }

	private static Option<long[]> AliasesOption { get; }

	private static Option<long?> IdOption { get; }

	internal static async Task<int> Handle(
		ILogger<NewTagCommand> logger,
		ITagRepository repo,
		string name,
		string? shorthand,
		Colour? colour,
		bool hidden,
		long[] parents,
		long[] aliases,
		long? id,
		CancellationToken cancellationToken
	)
	{
		logger.CommandExecutionStarted();

		var tag = new Tag(new TagId(id ?? 0), name, shorthand, colour, hidden);

		await repo.AddAsync(tag, cancellationToken);

		foreach (var parentId in parents)
		{
			await repo.AddGraphLinkAsync(tag.Id, new TagId(parentId), cancellationToken);
		}

		foreach (var aliasId in aliases)
		{
			await repo.AddAliasLinkAsync(tag.Id, new TagId(aliasId), cancellationToken);
		}

		logger.CommandExecutionCompleted();
		return 0;
	}
}