namespace Fictional.Data.Core.Models.Ids;

/// <summary>
///     Interface for an identifier.
/// </summary>
/// <remarks>
///     <para>This interface ensures common functionality is implemented by all ID types.</para>
///     <para>ID types should be used instead of primitives in all cases to prevent 'primitive obsession'.</para>
/// </remarks>
/// <typeparam name="TSelf">A self reference to the ID type</typeparam>
public interface IId<TSelf> :
	ISpanFormattable, IUtf8SpanFormattable,
	ISpanParsable<TSelf>,
	IComparable<TSelf>, IEquatable<TSelf> where TSelf : IId<TSelf>;