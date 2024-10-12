using SourceGeneratorUtils;

namespace Fictional.Build.GenUtils.Emitters.PrimitiveId;

internal record GuidPrimitiveEmitter : DefaultSourceCodeEmitter
{
	private static string StringSyntaxAttribute =>
		"[global::System.Diagnostics.CodeAnalysis.StringSyntax(global::System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.GuidFormat)]";

	public override IEnumerable<string> GetOuterUsingDirectives(DefaultGenerationSpec target)
	{
		yield return "System";
	}

	public override IEnumerable<string> GetAttributesToApply(DefaultGenerationSpec target)
	{
		yield return "global::System.Diagnostics.DebuggerDisplay(\"{Value}\")";
	}

	public override void EmitTargetSourceCode(DefaultGenerationSpec target, SourceWriter writer)
	{
		ToStringBlock(writer);
		writer.WriteLine();
		TryFormatCharsBlock(writer);
		writer.WriteLine();
		TryFormatUtf8Block(writer);
		writer.WriteLine();
		CompareToBlock(writer, target.TargetType.FullyQualifiedName);
		writer.WriteLine();
		ParseBlock(writer, target.TargetType.FullyQualifiedName);
		writer.WriteLine();
		TryParseBlock(writer, target.TargetType.FullyQualifiedName);
		writer.WriteLine();
		ParseSpanBlock(writer, target.TargetType.FullyQualifiedName);
		writer.WriteLine();
		TryParseSpanBlock(writer, target.TargetType.FullyQualifiedName);
		writer.WriteLine();
		WriteConversionOperators(writer, target.TargetType.FullyQualifiedName);
	}

	private static void WriteConversionOperators(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine($"public static explicit operator {typeName}(Guid value) => new(value);")
			.WriteLine($"public static implicit operator Guid({typeName} value) => value.Value;");
	}

	private static string NotNullWhenAttribute(bool value)
	{
		return $"[global::System.Diagnostics.CodeAnalysis.NotNullWhen({value.ToString().ToLowerInvariant()})]";
	}

	private void TryFormatCharsBlock(SourceWriter writer)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public bool TryFormat(Span<char> destination, out int charsWritten, {StringSyntaxAttribute} ReadOnlySpan<char> format, IFormatProvider? provider) => Value.TryFormat(destination, out charsWritten, format);");
	}

	private void TryFormatUtf8Block(SourceWriter writer)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, {StringSyntaxAttribute} ReadOnlySpan<char> format, IFormatProvider? provider) => Value.TryFormat(utf8Destination, out bytesWritten, format);");
	}

	private void CompareToBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine($"public int CompareTo({typeName} other) => Value.CompareTo(other.Value);");
	}

	private void ParseBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public static {typeName} Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);");
	}

	private void TryParseBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public static bool TryParse({NotNullWhenAttribute(true)} string? s, IFormatProvider? provider, out {typeName} result) => TryParse(s.AsSpan(), provider, out result);");
	}

	private void ParseSpanBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine($"public static {typeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider)");
		writer.OpenBlock();
		writer.WriteLine("var value = Guid.Parse(s, provider);");
		writer.WriteLine($"return new {typeName}(value);");
		writer.CloseBlock();
	}

	private void TryParseSpanBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out {typeName} result)");
		writer.OpenBlock();
		writer.WriteLine("if(!Guid.TryParse(s, provider, out var value))");
		writer.OpenBlock();
		writer.WriteLine("result = default;");
		writer.WriteLine("return false;");
		writer.CloseBlock();
		writer.WriteLine($"result = new {typeName}(value);");
		writer.WriteLine("return true;");
		writer.CloseBlock();
	}

	private void ToStringBlock(SourceWriter writer)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public string ToString({StringSyntaxAttribute}string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);");
	}
}