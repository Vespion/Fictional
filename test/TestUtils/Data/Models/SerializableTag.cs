using Fictional.Data.Core.Models;
using Fictional.Data.Core.Models.Tags;
using Xunit.Sdk;

namespace Fictional.TestUtils.Data.Models;

/// <summary>
///     Helper class that allows Xunit to serialize and deserialize <see cref="Tag" /> instances.
/// </summary>
/// <remarks>
///     This type supports the infrastructure and is not intended to be used directly from your code.
/// </remarks>
public record struct SerializableTag(
	TagId Id,
	string Name,
	string? Shorthand,
	Colour? Colour,
	bool Hidden
) : IXunitSerializable
{
	/// <inheritdoc />
	public void Deserialize(IXunitSerializationInfo info)
	{
		Id = new TagId(info.GetValue<long>(nameof(Id)));
		Name = info.GetValue<string>(nameof(Name))!;
		Shorthand = info.GetValue<string?>(nameof(Shorthand));
		Colour = (Colour?)info.GetValue<int?>(nameof(Colour));
		Hidden = info.GetValue<bool>(nameof(Hidden));
	}

	/// <inheritdoc />
	public void Serialize(IXunitSerializationInfo info)
	{
		info.AddValue(nameof(Id), Id.Value);
		info.AddValue(nameof(Name), Name);
		info.AddValue(nameof(Shorthand), Shorthand);
		info.AddValue(nameof(Colour), (int?)Colour);
		info.AddValue(nameof(Hidden), Hidden);
	}

	/// <summary>
	///    Implicitly converts a <see cref="SerializableTag" /> to a <see cref="Tag" />.
	/// </summary>
	public static implicit operator Tag(SerializableTag tag)
	{
		return new Tag(tag.Id, tag.Name, tag.Shorthand, tag.Colour, tag.Hidden);
	}

	/// <summary>
	///   Implicitly converts a <see cref="Tag" /> to a <see cref="SerializableTag" />.
	/// </summary>
	public static implicit operator SerializableTag(Tag tag)
	{
		return new SerializableTag(tag.Id, tag.Name, tag.Shorthand, tag.Colour, tag.Hidden);
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return HashCode.Combine(Id.Value);
	}
}