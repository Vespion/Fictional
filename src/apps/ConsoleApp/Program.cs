using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Fictional.App.ConsoleApp.Commands.Tags;
using Fictional.App.ConsoleApp.Commands.Tags.Alias;
using Fictional.App.ConsoleApp.Commands.Tags.Link;
using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Logging;

namespace Fictional.App.ConsoleApp;

internal static class Program
{
	private static RootCommand GenerateCommandTree()
	{
		var rootCommand = new RootCommand("Fictional CLI");

		var tagsCommand = new Command("tags", "Commands for managing tags");
		tagsCommand.AddGlobalOption(GlobalOptions.TargetExistingDatabaseOption);
		tagsCommand.AddCommand(NewTagCommand.Configure());
		tagsCommand.AddCommand(DeleteTagCommand.Configure());
		tagsCommand.AddCommand(UpdateTagCommand.Configure());
		var tagAliasCommand = new Command("alias", "Commands for managing tag aliases");
		tagAliasCommand.AddCommand(AddTagAliasCommand.Configure());
		tagAliasCommand.AddCommand(DeleteTagAliasCommand.Configure());
		tagsCommand.AddCommand(tagAliasCommand);
		var tagLinkCommand = new Command("link", "Commands for managing tag links");
		tagLinkCommand.AddCommand(AddTagLinkCommand.Configure());
		tagLinkCommand.AddCommand(DeleteTagLinkCommand.Configure());
		tagsCommand.AddCommand(tagLinkCommand);

		rootCommand.AddCommand(tagsCommand);

		return rootCommand;
	}

	private static void ConfigureLogging(ILoggingBuilder lb, InvocationContext context)
	{
		lb.Configure(o =>
		{
			o.ActivityTrackingOptions =
				ActivityTrackingOptions.Baggage |
				ActivityTrackingOptions.Tags |
				ActivityTrackingOptions.ParentId |
				ActivityTrackingOptions.SpanId |
				ActivityTrackingOptions.TraceId;
		});
		lb.SetMinimumLevel(LogLevel.Trace);
		lb.AddDebug();

		if (context.ParseResult.Directives.Contains("log-to-file"))
		{

			IReadOnlyCollection<string> paths;
			if (!context.ParseResult.Directives.TryGetValues("log-to-file", out var param))
			{
				paths = param ?? ["cli_<date>-<counter>.json.log"];
			}
			else
			{
				paths = ["cli_<date>-<counter>.json.log"];
			}

			lb.AddJsonFile(o =>
			{
				o.IncludeScopes = true;

				o.Files = paths
					.Select(x => new LogFileOptions
					{
						Path = x
					})
					.ToArray();

				o.FileAccessMode = LogFileAccessMode.OpenTemporarily;
			});
		}
	}

	public static async Task<int> Main(string[] args)
	{
		var builder = new CommandLineBuilder(GenerateCommandTree())
			.UseDefaults()
			.AddMiddleware(async (context, next) =>
			{
				// context.BindingContext.AddService<IFileSystem>(_ => new FileSystem());

				context.BindingContext.AddService<ILoggerFactory>(_ =>
				{
					var factory = LoggerFactory.Create(lb => ConfigureLogging(lb, context));

					return factory;
				});

				await next(context);

			}, MiddlewareOrder.Configuration);

		var parser = builder.Build();

		var parseResult = parser.Parse(args);

		var exitCode = await parseResult.InvokeAsync();

		return exitCode;
	}
}