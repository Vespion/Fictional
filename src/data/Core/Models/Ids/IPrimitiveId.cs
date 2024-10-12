namespace Fictional.Data.Core.Models.Ids;

/// <inheritdoc />
/// <summary>
///     Interface for a wrapped primitive identifier.
/// </summary>
/// <typeparam name="TPrimitive">The wrapped primitive type</typeparam>
#pragma warning disable CS1712
public interface IPrimitiveId<TSelf, out TPrimitive> : IId<TSelf> where TSelf : IPrimitiveId<TSelf, TPrimitive>
{
	/// <summary>
	///     Gets the value of the identifier.
	/// </summary>
	TPrimitive Value { get; }
}