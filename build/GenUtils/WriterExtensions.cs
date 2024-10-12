using SourceGeneratorUtils;

namespace Fictional.Build.GenUtils;

internal static class WriterExtensions
{
	public static SourceWriter WriteInheritanceDocComment(this SourceWriter writer)
	{
		return writer.WriteLine("/// <inheritdoc />");
	}
}