namespace Fictional.Plugins.PluginRuntimeLib.Exceptions;

/// <summary>
/// Can be thrown when access to a resource is denied by the resource origin.
/// </summary>
/// <param name="msg">The exception message</param>
/// <param name="origin">The queried origin</param>
/// <param name="causedByRobotsPolicy">True if this was due to a robots.txt or similar, false otherwise</param>
public class AccessDeniedByOrigin(string msg, string origin, bool causedByRobotsPolicy): UnauthorizedAccessException(msg)
{
	/// <summary>
	/// True if this was due to a robots.txt or similar, false otherwise
	/// </summary>
	public bool CausedByRobotsPolicy { get; } = causedByRobotsPolicy;

	/// <summary>
	/// The queried origin
	/// </summary>
	public string Origin { get; } = origin;
}