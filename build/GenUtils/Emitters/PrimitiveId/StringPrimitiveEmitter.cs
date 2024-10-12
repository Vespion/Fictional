using SourceGeneratorUtils;

namespace Fictional.Build.GenUtils.Emitters.PrimitiveId;

internal record StringPrimitiveEmitter : DefaultSourceCodeEmitter
{
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
			.WriteLine($"public static explicit operator {typeName}(string value) => new(value);")
			.WriteLine($"public static implicit operator string({typeName} value) => value.Value;");
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
				"public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)")
			.OpenBlock()
			.WriteLine("if (Value.Length > destination.Length)")
			.OpenBlock()
			.WriteLine(
				"throw new ArgumentException(\"Destination span is not large enough to hold the source string.\");")
			.CloseBlock()
			.WriteLine("charsWritten = 0;")
			.WriteLine("for (int i = 0; i < Value.Length; i++)")
			.OpenBlock()
			.WriteLine("destination[i] = Value[i];")
			.WriteLine("charsWritten++;")
			.CloseBlock()
			.WriteLine("return true;")
			.CloseBlock();
	}

	private void TryFormatUtf8Block(SourceWriter writer)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				"public bool TryFormat(Span<byte> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)")
			.OpenBlock()
			.WriteLine("var valueBytes = global::System.Text.Encoding.UTF8.GetBytes(Value);")
			.WriteLine("if (valueBytes.Length > destination.Length)")
			.OpenBlock()
			.WriteLine(
				"throw new ArgumentException(\"Destination span is not large enough to hold the source string.\");")
			.CloseBlock()
			.WriteLine("charsWritten = 0;")
			.WriteLine("for (int i = 0; i < valueBytes.Length; i++)")
			.OpenBlock()
			.WriteLine("destination[i] = valueBytes[i];")
			.WriteLine("charsWritten++;")
			.CloseBlock()
			.WriteLine("return true;")
			.CloseBlock();
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
				$"public static {typeName} Parse(string s, IFormatProvider? provider) => new {typeName}(s);");
	}

	private void TryParseBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public static bool TryParse({NotNullWhenAttribute(true)} string? s, IFormatProvider? provider, out {typeName} result)")
			.OpenBlock()
			.WriteLine($"result = new {typeName}(s);")
			.WriteLine("return true;")
			.CloseBlock();
	}

	private void ParseSpanBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine($"public static {typeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider)");
		writer.OpenBlock();
		writer.WriteLine($"return new {typeName}(s.ToString());");
		writer.CloseBlock();
	}

	private void TryParseSpanBlock(SourceWriter writer, string typeName)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				$"public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out {typeName} result)");
		writer.OpenBlock();
		writer.WriteLine($"result = new {typeName}(s.ToString());");
		writer.WriteLine("return true;");
		writer.CloseBlock();
	}

	private void ToStringBlock(SourceWriter writer)
	{
		writer
			.WriteInheritanceDocComment()
			.WriteLine(
				"public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(formatProvider);");
	}
}