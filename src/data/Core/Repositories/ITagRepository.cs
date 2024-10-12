using Fictional.Data.Core.Models.Tags;

namespace Fictional.Data.Core.Repositories;

/// <summary>
///     A repository for <see cref="Tag" /> entities.
/// </summary>
public interface ITagRepository : IBasicRepository<Tag, TagId>
{
	/// <summary>
	///	 Updates a tag.
	/// </summary>
	/// <param name="tag">The tag to update</param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="KeyNotFoundException">Thrown when attempting to update a tag that has not been added to the repository</exception>
	ValueTask UpdateAsync(Tag tag, CancellationToken cancellationToken = default);

	/// <summary>
	///     Marks a tag as an alias of another tag.
	/// </summary>
	/// <param name="canonicalTag">The canonical tag</param>
	/// <param name="aliasTag">The tag being used as an alias</param>
	/// <param name="cancellationToken"></param>
	/// <returns>True if the alias was added, false if it already existed</returns>
	ValueTask<bool> AddAliasLinkAsync(TagId canonicalTag, TagId aliasTag,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Removes an alias link between two tags.
	/// </summary>
	/// <param name="canonicalTag">The canonical tag</param>
	/// <param name="aliasTag">The tag being used as an alias</param>
	/// <param name="cancellationToken"></param>
	/// <returns>True if the alias was removed, false if it did not exist</returns>
	ValueTask<bool> RemoveAliasLinkAsync(TagId canonicalTag, TagId aliasTag,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Gets all tags that are aliases of a given tag.
	/// </summary>
	/// <param name="canonicalTag">The canonical tag to get the aliases for</param>
	/// <param name="cancellationToken"></param>
	/// <returns>All aliased <see cref="TagId">TagIds</see></returns>
	ValueTask<IEnumerable<TagId>> GetAliasesAsync(TagId canonicalTag, CancellationToken cancellationToken = default);

	/// <summary>
	///     Adds a link between two tags in the graph.
	/// </summary>
	/// <param name="parentTag">The parent tag</param>
	/// <param name="childTag">The child tag</param>
	/// <param name="cancellationToken"></param>
	/// <returns>True if the link was added, false if it already existed</returns>
	ValueTask<bool> AddGraphLinkAsync(TagId parentTag, TagId childTag, CancellationToken cancellationToken = default);

	/// <summary>
	///     Removes a link between two tags in the graph.
	/// </summary>
	/// <param name="parentTag">The parent tag</param>
	/// <param name="childTag">The child tag</param>
	/// <param name="cancellationToken"></param>
	/// <returns>True if the link was removed, false if it did not exist</returns>
	ValueTask<bool> RemoveGraphLinkAsync(TagId parentTag, TagId childTag,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Gets all children of a given tag.
	/// </summary>
	/// <param name="parentTag">The parent tag to get the children for</param>
	/// <param name="cancellationToken"></param>
	/// <returns>All children <see cref="TagId">TagIds</see></returns>
	ValueTask<IEnumerable<TagId>> GetChildrenAsync(TagId parentTag, CancellationToken cancellationToken = default);
}