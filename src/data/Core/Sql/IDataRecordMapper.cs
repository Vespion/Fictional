namespace Fictional.Data.Core.Sql;

/// <summary>
///     Interface for statically creating an instance of <typeparamref name="T" /> from an
///     <see cref="OptimisedDataRecord" />.
/// </summary>
/// <typeparam name="T">The type that the implementer of <see cref="IDataRecordMapper{T}" /> can create instances of.</typeparam>
public interface IDataRecordMapper<out T> where T : IDataRecordMapper<T>
{
	/// <summary>
	///     Maps the <paramref name="dataRecord" /> to an instance of <typeparamref name="T" />.
	/// </summary>
	/// <param name="dataRecord">The <see cref="OptimisedDataRecord" />.</param>
	/// <returns>A new instance of <typeparamref name="T" />.</returns>
	static abstract T Map(OptimisedDataRecord dataRecord);
}