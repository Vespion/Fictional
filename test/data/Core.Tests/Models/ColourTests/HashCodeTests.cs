using Fictional.Data.Core.Models;

namespace Fictional.Data.Core.Tests.Models.ColourTests;

public class HashCodeTests
{
	[Theory]
	[MemberData(nameof(ColourIntTests.RgbaValues), MemberType = typeof(ColourIntTests))]
	public void RgbaHashCodeIsConsistent(byte r, byte g, byte b, byte a)
	{
		var colour1 = new Colour(r, g, b, a);
		var colour2 = new Colour(r, g, b, a);

		var hash1 = colour1.GetHashCode();
		var hash2 = colour2.GetHashCode();

		Assert.Equal(hash1, hash2);
	}

	[Theory]
	[MemberData(nameof(ColourIntTests.RgbValues), MemberType = typeof(ColourIntTests))]
	public void RgbHashCodeIsConsistent(byte r, byte g, byte b)
	{
		var colour1 = new Colour(r, g, b);
		var colour2 = new Colour(r, g, b);

		var hash1 = colour1.GetHashCode();
		var hash2 = colour2.GetHashCode();

		Assert.Equal(hash1, hash2);
	}

	[Fact]
	public void RgbHashCodeDoesNotCollide_PropertyTest()
	{
		Gen<(byte, byte, byte)> RgbGen()
		{
			return Gen.Byte.Select(Gen.Byte, Gen.Byte);
		}

		RgbGen().Select(RgbGen()).Sample(RgbHashCodeDoesNotCollide);
	}

	[Theory]
	[MemberData(nameof(RgbValues))]
	public void RgbHashCodeDoesNotCollide((byte r, byte g, byte b) colourValues1,
		(byte r, byte g, byte b) colourValues2)
	{
		var colour1 = new Colour(colourValues1.r, colourValues1.g, colourValues1.b);
		var colour2 = new Colour(colourValues2.r, colourValues2.g, colourValues2.b);

		var hash1 = colour1.GetHashCode();
		var hash2 = colour2.GetHashCode();

		Assert.NotEqual(hash1, hash2);
	}

	[Fact]
	public void RgbaHashCodeDoesNotCollide_PropertyTest()
	{
		Gen<(byte, byte, byte, byte)> RgbaGen()
		{
			return Gen.Byte.Select(Gen.Byte, Gen.Byte, Gen.Byte);
		}

		RgbaGen().Select(RgbaGen()).Sample(RgbaHashCodeDoesNotCollide);
	}

	[Theory]
	[MemberData(nameof(RgbaValues))]
	public void RgbaHashCodeDoesNotCollide((byte r, byte g, byte b, byte a) colourValues1,
		(byte r, byte g, byte b, byte a) colourValues2)
	{
		var colour1 = new Colour(colourValues1.r, colourValues1.g, colourValues1.b, colourValues1.a);
		var colour2 = new Colour(colourValues2.r, colourValues2.g, colourValues2.b, colourValues2.a);

		var hash1 = colour1.GetHashCode();
		var hash2 = colour2.GetHashCode();

		Assert.NotEqual(hash1, hash2);
	}

	public static IEnumerable<TheoryDataRow<(byte, byte, byte), (byte, byte, byte)>> RgbValues()
	{
		yield return new TheoryDataRow<(byte, byte, byte), (byte, byte, byte)>((0, 0, 0), (1, 1, 1));
		yield return new TheoryDataRow<(byte, byte, byte), (byte, byte, byte)>((12, 45, 55), (221, 225, 13));
	}

	public static IEnumerable<TheoryDataRow<(byte, byte, byte, byte), (byte, byte, byte, byte)>> RgbaValues()
	{
		yield return new TheoryDataRow<(byte, byte, byte, byte), (byte, byte, byte, byte)>((0, 0, 0, 4), (1, 1, 1, 0));
		yield return new TheoryDataRow<(byte, byte, byte, byte), (byte, byte, byte, byte)>((12, 45, 55, 14),
			(221, 225, 13, 56));
	}
}