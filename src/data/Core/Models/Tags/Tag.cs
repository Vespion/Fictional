using System.Diagnostics;
using Fictional.Data.Core.Models.Ids;
using Fictional.Data.Core.Sql;

namespace Fictional.Data.Core.Models.Tags;

/// <summary>
///     A struct that represents a tag.
/// </summary>
/// <param name="Id">The tag's ID</param>
/// <param name="Name">The name of the tag</param>
/// <param name="Shorthand">A shorthand for quick referencing</param>
/// <param name="Colour">A displayable colour for this tag</param>
/// <param name="Hidden">If this tag should be included in searches</param>
/// <remarks>
///     <para>
///         The <see cref="Hidden" /> property is designed to allow meta tags to be hidden from searches/filtering for
///         example hiding the character meta tag while still allowing it to be used as a parent
///     </para>
/// </remarks>
[DebuggerDisplay("{Name} (ID: {Id})")]
public readonly record struct Tag(
	TagId Id,
	string Name,
	string? Shorthand,
	Colour? Colour,
	bool Hidden
) : IHaveId<TagId>, IDataRecordMapper<Tag>
{
	/// <inheritdoc />
	public static Tag Map(OptimisedDataRecord dataRecord)
	{
		return new Tag(
			new TagId(dataRecord.GetInt64(nameof(Id))),
			dataRecord.GetString(nameof(Name)),
			dataRecord.IsDBNull(nameof(Shorthand)) ? null : dataRecord.GetString(nameof(Shorthand)),
			dataRecord.IsDBNull(nameof(Colour)) ? null : (Colour?)dataRecord.GetInt32(nameof(Colour)),
			dataRecord.GetBoolean(nameof(Hidden))
		);
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return HashCode.Combine(
			Id.Value,
			Name,
			Shorthand,
			Colour,
			Hidden
		);
	}
}