using Fictional.Data.Core.Models;
using Fictional.Data.Core.Models.Tags;
using Fictional.TestUtils.Data.Models;

namespace Fictional.TestUtils.Data;

/// <summary>
/// Test data helpers for <see cref="Tag" /> and related types.
/// </summary>
public static class TagData
{
	/// <summary>
	/// Generates a random colour hex string, including hashtag
	/// </summary>
	public static Gen<string?> ColourHexStringGen()
	{
		return Gen.Byte.Select(Gen.Byte, Gen.Byte, Gen.Byte)
			.Select((r, g, b, a) => new Colour(r, g, b, a).ToString())
			.Null();
	}

	/// <summary>
	/// Generator for <see cref="SerializableTag" />.
	/// </summary>
	public static Gen<SerializableTag> TagGen()
	{
		return Gen.Long.Select(
				Gen.String,
				Gen.Char.Select(c => c.ToString()).Null(),
				ColourHexStringGen(),
				Gen.Bool
			)
			.Where((_, name, _, _, _) => !string.IsNullOrWhiteSpace(name))
			.Select((id, name, shorthand, colourHex, hidden) =>
				new SerializableTag(new TagId(id),
					name,
					shorthand,
					colourHex != null
						? Colour.Parse(colourHex, null)
						: null,
					hidden
				)
			);
	}

	/// <summary>
	/// Generator for a batch of <see cref="SerializableTag" />.
	/// </summary>
	public static Gen<SerializableTag[]> TagBatchGen()
	{
		return TagGen()
			.Select(TagGen(), TagGen(), TagGen(), TagGen())
			.Where((tag1, tag2, tag3, tag4, tag5) =>
			{
				var ids = new HashSet<TagId> { tag1.Id, tag2.Id, tag3.Id, tag4.Id, tag5.Id };
				return ids.Count == 5;
			})
			.Select((tag1, tag2, tag3, tag4, tag5) => new[] { tag1, tag2, tag3, tag4, tag5 });
	}

	/// <summary>
	/// A set of paired <see cref="SerializableTag" />.
	/// </summary>
	public static IEnumerable<TheoryDataRow<SerializableTag, SerializableTag>> TagPair()
	{
		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "Test", "tst", null, false),
			new SerializableTag((TagId)2, "Test2", "tst2", null, false)
		);

		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "Test", "tst", null, true),
			new SerializableTag((TagId)2, "Test2", "tst2", null, true)
		);

		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "Test", "tst", new Colour(150, 150, 150), false),
			new SerializableTag((TagId)2, "Test2", "tst2", new Colour(150, 150, 150), false)
		);

		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "Test", "tst", new Colour(150, 150, 150), true),
			new SerializableTag((TagId)2, "Test2", "tst2", new Colour(150, 150, 150), true)
		);

		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "86FEAD35-461E-4BB1-A6B1-E0F093299D08", "86", null, false),
			new SerializableTag((TagId)2, "1D1D5ED4-7287-487F-A482-8AC36C1B4BC7", "1D", null, false)
		);

		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "86FEAD35-461E-4BB1-A6B1-E0F093299D08", "86", null, true),
			new SerializableTag((TagId)2, "1D1D5ED4-7287-487F-A482-8AC36C1B4BC7", "1D", null, true)
		);

		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "86FEAD35-461E-4BB1-A6B1-E0F093299D08", "86", new Colour(150, 150, 150), false),
			new SerializableTag((TagId)2, "1D1D5ED4-7287-487F-A482-8AC36C1B4BC7", "1D", new Colour(150, 150, 150), false)
		);

		yield return new TheoryDataRow<SerializableTag, SerializableTag>(
			new SerializableTag((TagId)1, "86FEAD35-461E-4BB1-A6B1-E0F093299D08", "86", new Colour(150, 150, 150), true),
			new SerializableTag((TagId)2, "1D1D5ED4-7287-487F-A482-8AC36C1B4BC7", "1D", new Colour(150, 150, 150), true)
		);
	}

	/// <summary>
	/// A set of <see cref="SerializableTag" /> batches.
	/// </summary>
	public static IEnumerable<TheoryDataRow<SerializableTag[]>> TagBatches()
	{
		yield return
			new[]
			{
				new SerializableTag((TagId)1, "Test", "tst", null, false),
				new SerializableTag((TagId)2, "Test2", "tst2", null, false),
				new SerializableTag((TagId)3, "Test3", "tst3", null, false),
				new SerializableTag((TagId)4, "Test4", "tst4", null, false),
				new SerializableTag((TagId)5, "Test5", "tst5", null, false)
			};

		yield return
			new[]
			{
				new SerializableTag((TagId)1, "Test", "tst", null, true),
				new SerializableTag((TagId)2, "Test2", "tst2", null, true),
				new SerializableTag((TagId)3, "Test3", "tst3", null, true),
				new SerializableTag((TagId)4, "Test4", "tst4", null, true),
				new SerializableTag((TagId)5, "Test5", "tst5", null, true)
			};

		yield return
			new[]
			{
				new SerializableTag((TagId)1, "Test", "tst", new Colour(150, 150, 150), false),
				new SerializableTag((TagId)2, "Test2", "tst2", new Colour(150, 150, 150), false),
				new SerializableTag((TagId)3, "Test3", "tst3", new Colour(150, 150, 150), false),
				new SerializableTag((TagId)4, "Test4", "tst4", new Colour(150, 150, 150), false),
				new SerializableTag((TagId)5, "Test5", "tst5", new Colour(150, 150, 150), false)
			};

		yield return
			new[]
			{
				new SerializableTag((TagId)1, "Test", "tst", new Colour(150, 150, 150), true),
				new SerializableTag((TagId)2, "Test2", "tst2", new Colour(150, 150, 150), true),
				new SerializableTag((TagId)3, "Test3", "tst3", new Colour(150, 150, 150), true),
				new SerializableTag((TagId)4, "Test4", "tst4", new Colour(150, 150, 150), true),
				new SerializableTag((TagId)5, "Test5", "tst5", new Colour(150, 150, 150), true)
			};
	}

	/// <summary>
	/// A set of <see cref="SerializableTag" /> instances.
	/// </summary>
	public static IEnumerable<TheoryDataRow<SerializableTag>> Tags()
	{
		yield return new SerializableTag((TagId)1, "Test", "tst", null, false);
		yield return new SerializableTag((TagId)2, "Test2", "tst2", null, false);
		yield return new SerializableTag((TagId)3, "Test3", "tst3", null, false);
		yield return new SerializableTag((TagId)4, "Test4", "tst4", null, false);
		yield return new SerializableTag((TagId)5, "Test5", "tst5", null, false);

		yield return new SerializableTag((TagId)1, "Test", "tst", null, true);
		yield return new SerializableTag((TagId)2, "Test2", "tst2", null, true);
		yield return new SerializableTag((TagId)3, "Test3", "tst3", null, true);
		yield return new SerializableTag((TagId)4, "Test4", "tst4", null, true);
		yield return new SerializableTag((TagId)5, "Test5", "tst5", null, true);

		yield return new SerializableTag((TagId)1, "Test", "tst", new Colour(150, 150, 150), false);
		yield return new SerializableTag((TagId)2, "Test2", "tst2", new Colour(150, 150, 150), false);
		yield return new SerializableTag((TagId)3, "Test3", "tst3", new Colour(150, 150, 150), false);
		yield return new SerializableTag((TagId)4, "Test4", "tst4", new Colour(150, 150, 150), false);
		yield return new SerializableTag((TagId)5, "Test5", "tst5", new Colour(150, 150, 150), false);

		yield return new SerializableTag((TagId)1, "Test", "tst", new Colour(150, 150, 150), true);
		yield return new SerializableTag((TagId)2, "Test2", "tst2", new Colour(150, 150, 150), true);
		yield return new SerializableTag((TagId)3, "Test3", "tst3", new Colour(150, 150, 150), true);
		yield return new SerializableTag((TagId)4, "Test4", "tst4", new Colour(150, 150, 150), true);
		yield return new SerializableTag((TagId)5, "Test5", "tst5", new Colour(150, 150, 150), true);
	}
}