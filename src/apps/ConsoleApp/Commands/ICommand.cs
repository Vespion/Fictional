using System.CommandLine;

namespace Fictional.App.ConsoleApp.Commands;

/// <summary>
/// Interface for all executable commands.
/// </summary>
public interface ICommand
{
	/// <summary>
	/// Builds and returns a command for registration into the command tree.
	/// </summary>
	static abstract Command Configure();
}