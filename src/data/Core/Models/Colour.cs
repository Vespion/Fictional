using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace Fictional.Data.Core.Models;

/// <summary>
///     A struct that represents a colour with an alpha channel.
/// </summary>
/// <param name="R">Red channel</param>
/// <param name="G">Green channel</param>
/// <param name="B">Blue channel</param>
/// <param name="A">Alpha channel</param>
public readonly partial record struct Colour(byte R, byte G, byte B, byte A = 255) :
	ISpanParsable<Colour>,
	IUtf8SpanParsable<Colour>,
	IUtf8SpanFormattable, ISpanFormattable
{
	private const int RIndex = 0;
	private const int GIndex = 1;
	private const int BIndex = 2;
	private const int AIndex = 3;

	/// <inheritdoc />
	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		FormattableString f = $"#{R:x2}{G:x2}{B:x2}{A:x2}";
		return f.ToString(formatProvider);
	}

	/// <inheritdoc />
	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
		IFormatProvider? provider)
	{
		return destination.TryWrite(provider, $"#{R:x2}{G:x2}{B:x2}{A:x2}", out charsWritten);
	}

	/// <inheritdoc />
	public static Colour Parse(string s, IFormatProvider? provider)
	{
		if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s));

		if (!TryParse(s, provider, out var result))
			throw new FormatException("The string was not in a correct format.");

		return result;
	}

	/// <inheritdoc />
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
		out Colour result)
	{
		result = default;

		if (string.IsNullOrWhiteSpace(s)) return false;

		var regex = ParsingRegex();

		var match = regex.Match(s);

		if (!match.Success) return false;

		var r = byte.Parse(match.Groups[1].Value, NumberStyles.HexNumber, provider);
		var g = byte.Parse(match.Groups[2].Value, NumberStyles.HexNumber, provider);
		var b = byte.Parse(match.Groups[3].Value, NumberStyles.HexNumber, provider);

		if (match.Groups[4].Success)
		{
			var a = byte.Parse(match.Groups[4].Value, NumberStyles.HexNumber, provider);
			result = new Colour(r, g, b, a);
		}
		else
		{
			result = new Colour(r, g, b);
		}

		return true;
	}

	/// <inheritdoc />
	public static Colour Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		return Parse(s.ToString(), provider);
	}

	/// <inheritdoc />
	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider,
		out Colour result)
	{
		return TryParse(s.ToString(), provider, out result);
	}

	/// <inheritdoc />
	public bool TryFormat(Span<byte> destination, out int bytesWritten, ReadOnlySpan<char> format,
		IFormatProvider? provider)
	{
		return Utf8.TryWrite(destination, provider, $"#{R:x2}{G:x2}{B:x2}{A:x2}", out bytesWritten);
	}

	/// <inheritdoc />
	public static Colour Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
	{
		return Parse(Encoding.UTF8.GetString(utf8Text), provider);
	}

	/// <inheritdoc />
	public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider,
		out Colour result)
	{
		return TryParse(Encoding.UTF8.GetString(utf8Text), provider, out result);
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return HashCode.Combine(R, G, B, A);
	}

	/// <summary>
	///     Converts the binary representation of a <see cref="Colour" /> to an object instance
	/// </summary>
	/// <param name="colourCode">A 4 byte number <see cref="int" /></param>
	public static explicit operator Colour?(int? colourCode)
	{
		if (colourCode == null) return null;

		Span<byte> buffer = stackalloc byte[4];
		BinaryPrimitives.WriteInt32LittleEndian(buffer, colourCode.Value);
		return new Colour(buffer[RIndex], buffer[GIndex], buffer[BIndex], buffer[AIndex]);
	}

	/// <summary>
	///     Converts a <see cref="Colour" /> to a binary representation
	/// </summary>
	/// <param name="colour">The <see cref="Colour" /> instance to convert</param>
	/// <returns>A 4 byte number <see cref="int" /></returns>
	public static explicit operator int?(Colour? colour)
	{
		if (colour == null) return null;

		Span<byte> buffer = stackalloc byte[4];
		buffer[RIndex] = colour.Value.R;
		buffer[GIndex] = colour.Value.G;
		buffer[BIndex] = colour.Value.B;
		buffer[AIndex] = colour.Value.A;
		return BinaryPrimitives.ReadInt32LittleEndian(buffer);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"#{R:x2}{G:x2}{B:x2}{A:x2}";
	}

	[GeneratedRegex("^#?([A-F0-9]{2})([A-F0-9]{2})([A-F0-9]{2})([A-F0-9]{2})?$",
		RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
	private static partial Regex ParsingRegex();
}