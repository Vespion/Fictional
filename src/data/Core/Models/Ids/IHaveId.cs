namespace Fictional.Data.Core.Models.Ids;

/// <summary>
///     This interface is used to indicate that a type has an identifier.
/// </summary>
/// <typeparam name="TId">An identifier type that implements the <see cref="IId{TSelf}" /> interface</typeparam>
public interface IHaveId<out TId> where TId : IId<TId>
{
	/// <summary>
	///     ID of the object.
	/// </summary>
	TId Id { get; }
}