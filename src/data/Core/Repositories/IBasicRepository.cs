using Fictional.Data.Core.Models.Ids;

namespace Fictional.Data.Core.Repositories;

/// <summary>
///     Basic operations for a repository.
/// </summary>
/// <typeparam name="TObject">The type of the object.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IBasicRepository<TObject, in TKey> where TObject : struct, IHaveId<TKey> where TKey : IId<TKey>
{
	/// <summary>
	///     Fetches an object by its ID.
	/// </summary>
	/// <param name="id">The ID of the requested object.</param>
	/// <param name="cancellationToken">An optional cancellation token.</param>
	/// <returns>The object if found, null otherwise.</returns>
	ValueTask<TObject?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

	/// <summary>
	///     Fetches all objects.
	/// </summary>
	/// <param name="cancellationToken">An optional cancellation token.</param>
	/// <returns>An asynchronous stream of objects.</returns>
	IAsyncEnumerable<TObject> GetAllAsync(CancellationToken cancellationToken = default);

	/// <summary>
	///     Adds a new object.
	/// </summary>
	/// <param name="obj">The object to add.</param>
	/// <param name="cancellationToken">An optional cancellation token.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	ValueTask AddAsync(TObject obj, CancellationToken cancellationToken = default);

	/// <summary>
	///     Checks if an object exists by its ID.
	/// </summary>
	/// <param name="id">The ID of the object to check.</param>
	/// <param name="cancellationToken">An optional cancellation token.</param>
	/// <returns>True if the object exists, false otherwise.</returns>
	ValueTask<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

	/// <summary>
	///     Deletes an object by its ID.
	/// </summary>
	/// <param name="id">The ID of the object to delete.</param>
	/// <param name="cancellationToken">An optional cancellation token.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	ValueTask DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}