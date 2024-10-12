using System.Text;
using Fictional.Data.Core.Models;

namespace Fictional.Data.Core.Tests.Models.ColourTests;

public class ColourStringTests
{
	internal static string HexStringBuilder(byte r, byte g, byte b, byte? a, bool includeHash)
	{
		return
			$"{(includeHash ? "#" : string.Empty)}{r:X2}{g:X2}{b:X2}{(a.HasValue ? a.Value.ToString("X2") : string.Empty)}";
	}

	internal static void HexStringParsingAssertions(
		string hexString,
		byte r,
		byte g,
		byte b,
		byte a = 255,
		bool tryParse = false)
	{
		Colour colour;

		if (tryParse)
		{
			var result = Colour.TryParse(hexString, null, out colour);

			Assert.True(result);
		}
		else
		{
			colour = Colour.Parse(hexString.AsSpan(), null);
		}

		Assert.Equal(r, colour.R);
		Assert.Equal(g, colour.G);
		Assert.Equal(b, colour.B);
		Assert.Equal(a, colour.A);


		if (tryParse)
		{
			var result = Colour.TryParse(hexString.AsSpan(), null, out colour);
			Assert.True(result);
		}
		else
		{
			colour = Colour.Parse(hexString.AsSpan(), null);
		}

		Assert.Equal(r, colour.R);
		Assert.Equal(g, colour.G);
		Assert.Equal(b, colour.B);
		Assert.Equal(a, colour.A);

		if (tryParse)
		{
			var result = Colour.TryParse(Encoding.UTF8.GetBytes(hexString), null, out colour);
			Assert.True(result);
		}
		else
		{
			colour = Colour.Parse(hexString.AsSpan(), null);
		}

		Assert.Equal(r, colour.R);
		Assert.Equal(g, colour.G);
		Assert.Equal(b, colour.B);
		Assert.Equal(a, colour.A);
	}

	public static IEnumerable<TheoryDataRow<string>> InvalidHexStrings()
	{
		yield return "invalid";
		yield return "ffffff#";
		yield return "ffffffff#";
		yield return "ffffffff$";
		yield return "ffffff$";
		yield return "$ffffff";
		yield return "$ffffffff";
		yield return "~~ffffff";
		yield return "~ffffff";
		yield return "ff~~ffff";
		yield return "ff~ffff";
		yield return "ffff~~ff";
		yield return "ffff~ff";
		yield return "ffffff~~";
		yield return "ffffff~";
	}

	public static IEnumerable<TheoryDataRow<string?>> EmptyNullStrings()
	{
		yield return null!;
		yield return string.Empty;
		yield return " ";
		yield return "   ";
		yield return "\t";
		yield return "\n";
		yield return "\r\n";
	}

	[Theory]
	[MemberData(nameof(EmptyNullStrings))]
	public void ThrowsOnNullOrEmptyString(string? hexString)
	{
		Assert.Throws<ArgumentNullException>(() => Colour.Parse(hexString!, null));
		Assert.Throws<ArgumentNullException>(() => Colour.Parse(hexString.AsSpan(), null));
	}

	[Theory]
	[MemberData(nameof(EmptyNullStrings))]
	public void TryParseFailsOnNullOrEmptyString(string? hexString)
	{
		var result = Colour.TryParse(hexString!, null, out var colour);

		Assert.False(result);
		Assert.True(colour == default);

		result = Colour.TryParse(hexString.AsSpan(), null, out colour);

		Assert.False(result);
		Assert.True(colour == default);
	}

	[Fact]
	public void ParsesFullHexString_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte, Gen.Byte)
			.Sample(ParsesFullHexString);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void ParsesFullHexString(byte r, byte g, byte b, byte a)
	{
		var hexString = HexStringBuilder(r, g, b, a, true);

		HexStringParsingAssertions(hexString, r, g, b, a);
	}

	[Fact]
	public void TryParsesFullHexStringSucceeds_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte, Gen.Byte)
			.Sample(TryParsesFullHexStringSucceeds);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void TryParsesFullHexStringSucceeds(byte r, byte g, byte b, byte a)
	{
		var hexString = HexStringBuilder(r, g, b, a, true);

		HexStringParsingAssertions(hexString, r, g, b, a, true);
	}

	[Fact]
	public void ParsesMissingHashHexString_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte, Gen.Byte)
			.Sample(ParsesMissingHashHexString);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void ParsesMissingHashHexString(byte r, byte g, byte b, byte a)
	{
		var hexString = HexStringBuilder(r, g, b, a, false);

		HexStringParsingAssertions(hexString, r, g, b, a);
	}

	[Fact]
	public void TryParsesMissingHashHexStringSucceeds_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte, Gen.Byte)
			.Sample(TryParsesMissingHashHexStringSucceeds);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void TryParsesMissingHashHexStringSucceeds(byte r, byte g, byte b, byte a)
	{
		var hexString = HexStringBuilder(r, g, b, a, false);

		HexStringParsingAssertions(hexString, r, g, b, a, true);
	}

	[Fact]
	public void ParsesMissingAlphaHexString_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte)
			.Sample(ParsesMissingAlphaHexString);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void ParsesMissingAlphaHexString(byte r, byte g, byte b)
	{
		var hexString = HexStringBuilder(r, g, b, null, true);

		HexStringParsingAssertions(hexString, r, g, b);
	}

	[Fact]
	public void TryParsesMissingAlphaHexStringSucceeds_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte)
			.Sample(TryParsesMissingAlphaHexStringSucceeds);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void TryParsesMissingAlphaHexStringSucceeds(byte r, byte g, byte b)
	{
		var hexString = HexStringBuilder(r, g, b, null, true);

		HexStringParsingAssertions(hexString, r, g, b, tryParse: true);
	}

	[Fact]
	public void ParsesMissingAlphaHashHexString_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte)
			.Sample(ParsesMissingAlphaHashHexString);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void ParsesMissingAlphaHashHexString(byte r, byte g, byte b)
	{
		var hexString = HexStringBuilder(r, g, b, null, true);

		HexStringParsingAssertions(hexString, r, g, b);
	}

	[Fact]
	public void TryParsesMissingAlphaHashHexStringSucceeds_PropertyTest()
	{
		Gen.Byte.Select(Gen.Byte, Gen.Byte)
			.Sample(TryParsesMissingAlphaHashHexStringSucceeds);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void TryParsesMissingAlphaHashHexStringSucceeds(byte r, byte g, byte b)
	{
		var hexString = HexStringBuilder(r, g, b, null, true);

		HexStringParsingAssertions(hexString, r, g, b, tryParse: true);
	}

	[Fact]
	public void TryParseFailsOnInvalidHexString_PropertyTest()
	{
		Gen.String.Where(x => !string.IsNullOrWhiteSpace(x)).Sample(TryParseFailsOnInvalidHexString);
	}


	[Theory]
	[MemberData(nameof(InvalidHexStrings))]
	public void TryParseFailsOnInvalidHexString(string hexString)
	{
		var success = Colour.TryParse(hexString, null, out var colour);

		Assert.False(success);
		Assert.True(colour == default);

		success = Colour.TryParse(hexString.AsSpan(), null, out colour);

		Assert.False(success);
		Assert.True(colour == default);

		success = Colour.TryParse(Encoding.UTF8.GetBytes(hexString), null, out colour);

		Assert.False(success);
		Assert.True(colour == default);
	}

	[Fact]
	public void ThrowsOnInvalidHexString_PropertyTest()
	{
		Gen.String.Where(x => !string.IsNullOrWhiteSpace(x)).Sample(ThrowsOnInvalidHexString);
	}

	[Theory]
	[MemberData(nameof(InvalidHexStrings))]
	public void ThrowsOnInvalidHexString(string hexString)
	{
		Assert.Throws<FormatException>(() => Colour.Parse(hexString, null));
		Assert.Throws<FormatException>(() => Colour.Parse(hexString.AsSpan(), null));
		Assert.Throws<FormatException>(() => Colour.Parse(Encoding.UTF8.GetBytes(hexString), null));
	}
}