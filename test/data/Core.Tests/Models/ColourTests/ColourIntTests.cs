using System.Buffers.Binary;
using Fictional.Data.Core.Models;

namespace Fictional.Data.Core.Tests.Models.ColourTests;

public class ColourIntTests
{
	public static IEnumerable<TheoryDataRow<byte, byte, byte, byte>> RgbaValues()
	{
		yield return new TheoryDataRow<byte, byte, byte, byte>(255, 255, 255, 255);
		yield return new TheoryDataRow<byte, byte, byte, byte>(0, 0, 0, 0);
		yield return new TheoryDataRow<byte, byte, byte, byte>(255, 0, 0, 0);
		yield return new TheoryDataRow<byte, byte, byte, byte>(0, 255, 0, 0);
		yield return new TheoryDataRow<byte, byte, byte, byte>(0, 0, 255, 0);
		yield return new TheoryDataRow<byte, byte, byte, byte>(0, 0, 0, 255);
		yield return new TheoryDataRow<byte, byte, byte, byte>(255, 255, 255, 0);
		yield return new TheoryDataRow<byte, byte, byte, byte>(6, 18, 13, 46);
	}

	public static IEnumerable<TheoryDataRow<byte, byte, byte>> RgbValues()
	{
		yield return new TheoryDataRow<byte, byte, byte>(255, 255, 255);
		yield return new TheoryDataRow<byte, byte, byte>(0, 0, 0);
		yield return new TheoryDataRow<byte, byte, byte>(255, 0, 0);
		yield return new TheoryDataRow<byte, byte, byte>(0, 255, 0);
		yield return new TheoryDataRow<byte, byte, byte>(0, 0, 255);
		yield return new TheoryDataRow<byte, byte, byte>(6, 18, 13);
	}

	[Theory]
	[MemberData(nameof(RgbaValues))]
	public void ConvertsRgbaToInt(byte r, byte g, byte b, byte a)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, a]);

		var colour = new Colour(r, g, b, a);

		var result = (int)colour!;

		Assert.Equal(hexIntValue, result);
	}

	[Theory]
	[MemberData(nameof(RgbaValues))]
	public void ConvertsRgbaToNullInt(byte r, byte g, byte b, byte a)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, a]);

		var colour = new Colour(r, g, b, a);

		var result = (int?)colour!;

		Assert.Equal(hexIntValue, result);
	}

	[Fact]
	public void ConvertsNullToNullInt()
	{
		Colour? colour = null;

		var result = (int?)colour;

		Assert.Null(result);
	}

	[Fact]
	public void ConvertsNullToNullColour()
	{
		int? colour = null;

		var result = (Colour?)colour;

		Assert.Null(result);
	}

	[Theory]
	[MemberData(nameof(RgbaValues))]
	public void ConvertsRgbaToColour(byte r, byte g, byte b, byte a)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, a]);

		var result = (Colour)hexIntValue!;

		Assert.Equal(r, result.R);
		Assert.Equal(g, result.G);
		Assert.Equal(b, result.B);
		Assert.Equal(a, result.A);
	}

	[Theory]
	[MemberData(nameof(RgbaValues))]
	public void ConvertsRgbaToNullColour(byte r, byte g, byte b, byte a)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, a]);

		var result = (Colour?)hexIntValue;

		Assert.Equal(r, result!.Value.R);
		Assert.Equal(g, result.Value.G);
		Assert.Equal(b, result.Value.B);
		Assert.Equal(a, result.Value.A);
	}

	[Theory]
	[MemberData(nameof(RgbValues))]
	public void ConvertsRgbToInt(byte r, byte g, byte b)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, 255]);

		var colour = new Colour(r, g, b);

		var result = (int)colour!;

		Assert.Equal(hexIntValue, result);
	}

	[Theory]
	[MemberData(nameof(RgbValues))]
	public void ConvertsRgbToNullInt(byte r, byte g, byte b)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, 255]);

		var colour = new Colour(r, g, b);

		var result = (int?)colour!;

		Assert.Equal(hexIntValue, result);
	}


	[Theory]
	[MemberData(nameof(RgbValues))]
	public void ConvertsRgbToColour(byte r, byte g, byte b)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, 255]);

		var result = (Colour)hexIntValue!;

		RgbAssertions(result, r, g, b);
	}

	[Theory]
	[MemberData(nameof(RgbValues))]
	public void ConvertsRgbToNullColour(byte r, byte g, byte b)
	{
		var hexIntValue = BinaryPrimitives.ReadInt32LittleEndian([r, g, b, 255]);

		var result = (Colour?)hexIntValue;

		RgbAssertions(result!.Value, r, g, b);
	}

	private static void RgbAssertions(Colour colour, byte r, byte g, byte b)
	{
		Assert.Equal(r, colour.R);
		Assert.Equal(g, colour.G);
		Assert.Equal(b, colour.B);
		Assert.Equal(255, colour.A);
	}
}