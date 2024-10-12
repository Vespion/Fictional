using System.CommandLine;

namespace Fictional.App.ConsoleApp;

internal static class GlobalOptions
{
	private static Option<DirectoryInfo> TargetDatabaseOptionFactory()
	{
		var option = new Option<DirectoryInfo>("--database")
		{
			Description = "The directory of the target database for the command",
			IsRequired = true,
			Arity = ArgumentArity.ExactlyOne,
			AllowMultipleArgumentsPerToken = false,
			IsHidden = false
		};
		option.AddAlias("-d");
		option.LegalFilePathsOnly();
		option.SetDefaultValueFactory(() => new DirectoryInfo(Environment.CurrentDirectory));

		return option;
	}

	internal static Option<DirectoryInfo> TargetDatabaseOption { get; } = TargetDatabaseOptionFactory();

	private static Option<DirectoryInfo>? _targetExistingDatabaseOption;

	internal static Option<DirectoryInfo> TargetExistingDatabaseOption
	{
		get
		{
			if (_targetExistingDatabaseOption == null)
			{
				_targetExistingDatabaseOption = TargetDatabaseOptionFactory();
				_targetExistingDatabaseOption.AddValidator(
					result =>
					{
						foreach (var token in result.Tokens)
						{
							if (!Directory.Exists(token.Value))
							{
								result.ErrorMessage = result.LocalizationResources.DirectoryDoesNotExist(token.Value);
							}
						}
					}
				);
			}

			return _targetExistingDatabaseOption;
		}
	}
}