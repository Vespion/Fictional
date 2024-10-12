using System.Text;
using Fictional.Data.Core.Models;

namespace Fictional.Data.Core.Tests.Models.ColourTests;

public class ColourFormattingTests
{
	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void RgbaToStringRoundTrips(byte r, byte g, byte b, byte a)
	{
		var hexString = ColourStringTests.HexStringBuilder(r, g, b, a, true);

		var colour = new Colour(r, g, b, a);

		Assert.Equal(hexString.ToLowerInvariant(), colour.ToString().ToLowerInvariant());
		Assert.Equal(hexString.ToLowerInvariant(), colour.ToString("{0}", null).ToLowerInvariant());
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void RgbToStringRoundTrips(byte r, byte g, byte b)
	{
		var hexString = ColourStringTests.HexStringBuilder(r, g, b, null, true);

		var colour = new Colour(r, g, b);

		Assert.Equal($"{hexString}FF".ToLowerInvariant(), colour.ToString().ToLowerInvariant());
		Assert.Equal($"{hexString}FF".ToLowerInvariant(), colour.ToString("{0}", null).ToLowerInvariant());
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void RgbaToFormattedString(byte r, byte g, byte b, byte a)
	{
		var hexString = ColourStringTests.HexStringBuilder(r, g, b, a, true);

		var colour = new Colour(r, g, b, a);

		var formattedString = $"{colour}";

		Assert.Equal(hexString.ToLowerInvariant(), formattedString.ToLowerInvariant());
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void RgbToFormattedString(byte r, byte g, byte b)
	{
		var hexString = ColourStringTests.HexStringBuilder(r, g, b, null, true);

		var colour = new Colour(r, g, b);

		var formattedString = $"{colour}";

		Assert.Equal($"{hexString}ff".ToLowerInvariant(), formattedString.ToLowerInvariant());
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void RgbaToFormattedBytes(byte r, byte g, byte b, byte a)
	{
		var hexString = ColourStringTests.HexStringBuilder(r, g, b, a, true);

		var colour = new Colour(r, g, b, a);

		Span<byte> bytes = stackalloc byte[9];

		colour.TryFormat(bytes, out var _, "{0}", null)
			.Should().BeTrue();

		var formattedString = Encoding.UTF8.GetString(bytes);

		Assert.Equal(hexString.ToLowerInvariant(), formattedString.ToLowerInvariant());
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void RgbToFormattedBytes(byte r, byte g, byte b)
	{
		var hexString = ColourStringTests.HexStringBuilder(r, g, b, null, true);

		var colour = new Colour(r, g, b);

		Span<byte> bytes = stackalloc byte[9];

		colour.TryFormat(bytes, out var _, "{0}", null)
			.Should().BeTrue();

		var formattedString = Encoding.UTF8.GetString(bytes);

		Assert.Equal($"{hexString}ff".ToLowerInvariant(), formattedString.ToLowerInvariant());
	}
}