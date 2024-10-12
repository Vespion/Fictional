using Fictional.Data.Core.Models.Ids;
using Fictional.Data.Core.Sql;

namespace Fictional.Data.Core.Models.Tags;

/// <summary>
///     Strongly-typed ID for a tag.
/// </summary>
/// <param name="Value">The underlying primitive value</param>
public readonly partial record struct TagId(long Value) : IPrimitiveId<TagId, long>, IDataRecordMapper<TagId>
{
	/// <inheritdoc />
	public static TagId Map(OptimisedDataRecord dataRecord)
	{
		return new TagId(dataRecord.GetInt64(0));
	}
}