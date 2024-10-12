using Fictional.Data.Core.Models.Tags;
using Fictional.TestUtils.Data;
using Fictional.TestUtils.Data.Models;

namespace Fictional.Data.Core.Tests.Models.TagTests;

public class HashCodeTests
{
	[Theory]
	[MemberData(nameof(TagData.Tags), MemberType = typeof(TagData))]
	public void HashCodeIsConsistent(SerializableTag srcTag)
	{
		Tag tag1 = srcTag;
		var tag2 = tag1 with { Id = new TagId(tag1.Id.Value) };

		var hash1 = tag1.GetHashCode();
		var hash2 = tag2.GetHashCode();

		Assert.Equal(hash1, hash2);
	}
	//
	// [Fact]
	// public void HashCodeDoesNotCollide_PropertyTest()
	// {
	// 	TagData
	// 		.TagBatchGen()
	// 		.Sample(HashCodeDoesNotCollide);
	// }

	[Theory]
	[MemberData(nameof(TagData.TagBatches), MemberType = typeof(TagData))]
	public void HashCodeDoesNotCollide(SerializableTag[] tags)
	{
		var hashes = tags.Select(t => t.GetHashCode()).ToArray();

		Assert.True(hashes.Distinct().Count() == hashes.Length);
	}
}