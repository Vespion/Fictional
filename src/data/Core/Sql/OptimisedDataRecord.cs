using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Fictional.Data.Core.Sql;

/// <inheritdoc />
/// <summary>
///     An optimised data record that caches ordinal lookups.
/// </summary>
public class OptimisedDataRecord : IDataRecord
{
	private readonly Dictionary<string, int> _ordinalLookup;
	private IDataRecord _dataRecordImplementation;

	/// <summary>
	///     Creates a new instance of <see cref="OptimisedDataRecord" />.
	/// </summary>
	/// <param name="dataRecordImplementation">The underlying <see cref="IDataRecord" /></param>
	public OptimisedDataRecord(IDataRecord dataRecordImplementation)
	{
		_dataRecordImplementation = dataRecordImplementation;
		_ordinalLookup = new Dictionary<string, int>(FieldCount);
	}

	/// <summary>
	///     Creates a new instance of <see cref="OptimisedDataRecord" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This constructor is used when you want to assign the <see cref="IDataRecord" /> later using
	///         <see cref="AssignNewRecord" />
	///     </para>
	/// </remarks>
	public OptimisedDataRecord(int fieldCount = 0)
	{
		_dataRecordImplementation = null!;
		_ordinalLookup = new Dictionary<string, int>(fieldCount);
	}

	/// <inheritdoc />
	public bool GetBoolean(int i)
	{
		return _dataRecordImplementation.GetBoolean(i);
	}

	/// <inheritdoc />
	public byte GetByte(int i)
	{
		return _dataRecordImplementation.GetByte(i);
	}

	/// <inheritdoc />
	public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferOffset, int length)
	{
		return _dataRecordImplementation.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
	}

	/// <inheritdoc />
	public char GetChar(int i)
	{
		return _dataRecordImplementation.GetChar(i);
	}

	/// <inheritdoc />
	public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
	{
		return _dataRecordImplementation.GetChars(i, fieldoffset, buffer, bufferoffset, length);
	}

	/// <inheritdoc />
	public IDataReader GetData(int i)
	{
		return _dataRecordImplementation.GetData(i);
	}

	/// <inheritdoc />
	public string GetDataTypeName(int i)
	{
		return _dataRecordImplementation.GetDataTypeName(i);
	}

	/// <inheritdoc />
	public DateTime GetDateTime(int i)
	{
		return _dataRecordImplementation.GetDateTime(i);
	}

	/// <inheritdoc />
	public decimal GetDecimal(int i)
	{
		return _dataRecordImplementation.GetDecimal(i);
	}

	/// <inheritdoc />
	public double GetDouble(int i)
	{
		return _dataRecordImplementation.GetDouble(i);
	}

	/// <inheritdoc />
	[return:
		DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields |
		                           DynamicallyAccessedMemberTypes.PublicProperties)]
	public Type GetFieldType(int i)
	{
		return _dataRecordImplementation.GetFieldType(i);
	}

	/// <inheritdoc />
	public float GetFloat(int i)
	{
		return _dataRecordImplementation.GetFloat(i);
	}

	/// <inheritdoc />
	public Guid GetGuid(int i)
	{
		return _dataRecordImplementation.GetGuid(i);
	}

	/// <inheritdoc />
	public short GetInt16(int i)
	{
		return _dataRecordImplementation.GetInt16(i);
	}

	/// <inheritdoc />
	public int GetInt32(int i)
	{
		return _dataRecordImplementation.GetInt32(i);
	}

	/// <inheritdoc />
	public long GetInt64(int i)
	{
		return _dataRecordImplementation.GetInt64(i);
	}

	/// <inheritdoc />
	public string GetName(int i)
	{
		return _dataRecordImplementation.GetName(i);
	}

	/// <inheritdoc />
	public int GetOrdinal(string name)
	{
		if (!_ordinalLookup.TryGetValue(name, out var ordinal))
		{
			ordinal = _dataRecordImplementation.GetOrdinal(name);
			_ordinalLookup.Add(name, ordinal);
		}

		return ordinal;
	}

	/// <inheritdoc />
	public string GetString(int i)
	{
		return _dataRecordImplementation.GetString(i);
	}

	/// <inheritdoc />
	public object GetValue(int i)
	{
		return _dataRecordImplementation.GetValue(i);
	}

	/// <inheritdoc />
	public int GetValues(object[] values)
	{
		return _dataRecordImplementation.GetValues(values);
	}

	/// <inheritdoc />
	public bool IsDBNull(int i)
	{
		return _dataRecordImplementation.IsDBNull(i);
	}

	/// <inheritdoc />
	public int FieldCount => _dataRecordImplementation.FieldCount;

	/// <inheritdoc />
	public object this[int i] => _dataRecordImplementation[i];

	/// <inheritdoc />
	public object this[string name]
	{
		get
		{
			var ordinal = GetOrdinal(name);
			return _dataRecordImplementation[ordinal];
		}
	}

	/// <summary>
	///     Assigns a new <see cref="IDataRecord" /> to the instance.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This method can be used to re-use the same instance of <see cref="OptimisedDataRecord" /> with different
	///         <see cref="IDataRecord" /> implementations.
	///     </para>
	///     <para>
	///         This is useful when you are mapping multiple rows and they have the same ordinal layout, such as when mapping
	///         multiple records from the same query
	///     </para>
	/// </remarks>
	/// <param name="dataRecord">The new <see cref="IDataRecord" /> instance</param>
	public void AssignNewRecord(IDataRecord dataRecord)
	{
		_dataRecordImplementation = dataRecord;
	}

	/// <inheritdoc cref="GetBoolean(int)" />
	/// <param name="name">The name of the field to find</param>
	public bool GetBoolean(string name)
	{
		return GetBoolean(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetByte(int)" />
	/// <param name="name">The name of the field to find</param>
	public byte GetByte(string name)
	{
		return GetByte(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetBytes(int,long,byte[],int,int)" />
	/// <param name="name">The name of the field to find</param>
#pragma warning disable CS1573
	public long GetBytes(string name, long fieldOffset, byte[]? buffer, int bufferOffset, int length)
#pragma warning restore CS1573
	{
		return GetBytes(GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
	}

	/// <inheritdoc cref="GetChar(int)" />
	/// <param name="name">The name of the field to find</param>
	public char GetChar(string name)
	{
		return GetChar(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetChars(int,long,char[],int,int)" />
	/// <param name="name">The name of the field to find</param>
#pragma warning disable CS1573
	public long GetChars(string name, long fieldOffset, char[]? buffer, int bufferOffset, int length)
#pragma warning restore CS1573
	{
		return GetChars(GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
	}

	/// <inheritdoc cref="GetData(int)" />
	/// <param name="name">The name of the field to find</param>
	public IDataReader GetData(string name)
	{
		return GetData(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetDataTypeName(int)" />
	/// <param name="name">The name of the field to find</param>
	public string GetDataTypeName(string name)
	{
		return GetDataTypeName(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetDateTime(int)" />
	/// <param name="name">The name of the field to find</param>
	public DateTime GetDateTime(string name)
	{
		return GetDateTime(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetDecimal(int)" />
	/// <param name="name">The name of the field to find</param>
	public decimal GetDecimal(string name)
	{
		return GetDecimal(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetDouble(int)" />
	/// <param name="name">The name of the field to find</param>
	public double GetDouble(string name)
	{
		return GetDouble(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetFieldType(int)" />
	/// <param name="name">The name of the field to find</param>
	public Type GetFieldType(string name)
	{
		return GetFieldType(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetFloat(int)" />
	/// <param name="name">The name of the field to find</param>
	public float GetFloat(string name)
	{
		return GetFloat(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetGuid(int)" />
	/// <param name="name">The name of the field to find</param>
	public Guid GetGuid(string name)
	{
		return GetGuid(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetInt16(int)" />
	/// <param name="name">The name of the field to find</param>
	public short GetInt16(string name)
	{
		return GetInt16(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetInt32(int)" />
	/// <param name="name">The name of the field to find</param>
	public int GetInt32(string name)
	{
		return GetInt32(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetInt64(int)" />
	/// <param name="name">The name of the field to find</param>
	public long GetInt64(string name)
	{
		return GetInt64(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetName(int)" />
	/// <param name="name">The name of the field to find</param>
	public string GetName(string name)
	{
		return GetName(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetString(int)" />
	/// <param name="name">The name of the field to find</param>
	public string GetString(string name)
	{
		return GetString(GetOrdinal(name));
	}

	/// <inheritdoc cref="GetValue(int)" />
	/// <param name="i">The name of the field to find</param>
	public object GetValue(string i)
	{
		return _dataRecordImplementation.GetValue(GetOrdinal(i));
	}

	/// <inheritdoc cref="IsDBNull(int)" />
	/// <param name="i">The name of the field to find</param>
	// ReSharper disable once InconsistentNaming
	public bool IsDBNull(string i)
	{
		return _dataRecordImplementation.IsDBNull(GetOrdinal(i));
	}
}