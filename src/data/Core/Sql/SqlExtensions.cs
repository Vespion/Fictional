using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Fictional.Data.Core.Sql;

/// <summary>
///	 A collection of helper extension methods for <see cref="SqliteConnection" />.
/// </summary>
public static class SqlExtensions
{

	/// <summary>
	/// Opens the connection and initalizes it with the correct settings.
	/// </summary>
	/// <param name="connection">The connection to open</param>
	public static void OpenAndInitalize(this SqliteConnection connection)
	{
		connection.Open();

		connection.ExecuteNonQuery("PRAGMA optimize(0x10002)");
	}

	/// <summary>
	/// Executes a non-query command on the connection.
	/// </summary>
	/// <param name="connection">The connection to execute the command with</param>
	/// <param name="commandText">The SQL to execute</param>
	/// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
	/// <returns>The number of rows inserted, updated, or deleted. -1 for SELECT statements.</returns>
	public static int ExecuteNonQuery(this SqliteConnection connection, SqlStringInterpolationHandler commandText)
	{
		using var command = commandText.GetCommand(connection);
		return command.ExecuteNonQuery();
	}

	/// <summary>
	/// Executes a non-query command on the connection.
	/// </summary>
	/// <param name="connection">The connection to execute the command with</param>
	/// <param name="commandText">The SQL to execute</param>
	/// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
	/// <returns>The number of rows inserted, updated, or deleted. -1 for SELECT statements.</returns>
	public static int ExecuteNonQuery(this SqliteConnection connection, string commandText)
	{
		using var command = connection.CreateCommand();
		command.CommandText = commandText;
		return command.ExecuteNonQuery();
	}

	/// <summary>
	/// Executes a scalar command on the connection.
	/// </summary>
	/// <param name="connection">The connection to execute the command with</param>
	/// <param name="commandText">The SQL to execute</param>
	/// <typeparam name="T">The type of the scalar to return</typeparam>
	/// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
	/// <returns>The scalar value of the first column of the first row of the result set.</returns>
	/// <remarks>
	/// <para>The scalar value is converted to <typeparamref name="T"/> using a simple safe cast, as such valid values may be discarded if you do not supply the correct type</para>
	/// <para>For example, if the value is a <see cref="long"/> and you request a <see cref="int"/>, the value will be discarded</para>
	/// </remarks>
	public static T? ExecuteScalar<T>(this SqliteConnection connection, string commandText)
	{
		using var command = connection.CreateCommand();
		command.CommandText = commandText;
		return command.ExecuteScalar() is T ? (T?)command.ExecuteScalar() : default;
	}

	/// <summary>
	/// Executes a scalar command on the connection.
	/// </summary>
	/// <param name="connection">The connection to execute the command with</param>
	/// <param name="commandText">The SQL to execute</param>
	/// <typeparam name="T">The type of the scalar to return</typeparam>
	/// <exception cref="SqliteException">A SQLite error occurs during execution.</exception>
	/// <returns>The scalar value of the first column of the first row of the result set.</returns>
	/// <remarks>
	/// <para>The scalar value is converted to <typeparamref name="T"/> using a simple safe cast, as such valid values may be discarded if you do not supply the correct type</para>
	/// <para>For example, if the value is a <see cref="long"/> and you request a <see cref="int"/>, the value will be discarded</para>
	/// </remarks>
	public static T? ExecuteScalar<T>(this SqliteConnection connection, SqlStringInterpolationHandler commandText)
	{
		using var command = commandText.GetCommand(connection);

		return command.ExecuteScalar() is T ? (T?)command.ExecuteScalar() : default;
	}

	/// <summary>
	/// Executes a query that returns a single result.
	/// </summary>
	/// <param name="connection">The connection to execute the command with</param>
	/// <param name="commandText">The SQL to execute</param>
	/// <typeparam name="T">The type of the result to return</typeparam>
	/// <returns>The result of the query, or <see langword="null"/> if no results are returned</returns>
	/// <remarks>
	/// <para>If the query returns multiple results, only the first result is returned</para>
	/// <para>However SQLite still processes the entire query, as such you should always use a <c>LIMIT 1</c> clause in your query</para>
	/// </remarks>
	public static T? QuerySingle<T>(this SqliteConnection connection, SqlStringInterpolationHandler commandText)
		where T : IDataRecordMapper<T>
	{
		using var command = commandText.GetCommand(connection);
		using var reader = command.ExecuteReader();

		if (!reader.HasRows) return default;

		reader.Read();

		return T.Map(new OptimisedDataRecord(reader));
	}

	/// <summary>
	/// Executes a query that returns multiple results.
	/// </summary>
	/// <param name="connection">The connection to execute the command with</param>
	/// <param name="commandText">The SQL to execute</param>
	/// <typeparam name="T">The type of the result to return</typeparam>
	/// <returns>An <see cref="IEnumerable{T}"/> of the results of the query</returns>
	/// <remarks>
	/// <para>If the query returns no results, an empty enumerable is returned</para>
	/// </remarks>
	public static IEnumerable<T> Query<T>(this SqliteConnection connection, string commandText)
		where T : IDataRecordMapper<T>
	{
		using var command = connection.CreateCommand(commandText);
		using var reader = command.ExecuteReader();

		if (!reader.HasRows) yield break;

		var record = new OptimisedDataRecord(reader.FieldCount);
		while (reader.Read())
		{
			record.AssignNewRecord(reader);
			yield return T.Map(record);
		}
	}

	/// <summary>
	/// Executes a query that returns multiple results.
	/// </summary>
	/// <param name="connection">The connection to execute the command with</param>
	/// <param name="commandText">The SQL to execute</param>
	/// <typeparam name="T">The type of the result to return</typeparam>
	/// <returns>An <see cref="IEnumerable{T}"/> of the results of the query</returns>
	/// <remarks>
	/// <para>If the query returns no results, an empty enumerable is returned</para>
	/// </remarks>
	public static IEnumerable<T> Query<T>(this SqliteConnection connection, SqlStringInterpolationHandler commandText)
		where T : IDataRecordMapper<T>
	{
		using var command = commandText.GetCommand(connection);
		using var reader = command.ExecuteReader();

		if (!reader.HasRows) return Array.Empty<T>();

		var recordList = new List<T>();

		var record = new OptimisedDataRecord(reader.FieldCount);
		while (reader.Read())
		{
			record.AssignNewRecord(reader);
			recordList.Add(T.Map(record));
		}

		return recordList.AsReadOnly();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static SqliteCommand CreateCommand(this SqliteConnection connection, string commandText)
	{
		var command = connection.CreateCommand();
		command.CommandText = commandText;
		return command;
	}

	/// <summary>
	///     A custom string interpolation handler for SQL strings.
	/// </summary>
	/// <remarks>This handler allows SQL strings and interpolated values to be converted to a parametrised command object</remarks>
	[InterpolatedStringHandler]
	public ref struct SqlStringInterpolationHandler(int literalLength, int formattedCount)
	{
		private readonly StringBuilder _builder = new(literalLength + formattedCount * 8);

		private readonly ArraySegment<SqliteParameter> _parameters = formattedCount > 0
			? new ArraySegment<SqliteParameter>(ArrayPool<SqliteParameter>.Shared.Rent(formattedCount), 0,
				formattedCount)
			: [];

		private int _parameterIndex = 0;

		/// <summary>
		///	 Appends a literal string to the command.
		/// </summary>
		/// <param name="s">The string to append</param>
		/// <remarks>This method is called by the compiler and should not be called directly</remarks>
		public void AppendLiteral(string s)
		{
			_builder.Append(s);
		}

		/// <summary>
		///	Appends a formatted value to the command as an SQL parameter.
		/// </summary>
		/// <param name="value">The value to append</param>
		/// <typeparam name="T">The type of the value to append</typeparam>
		/// <remarks>This method is called by the compiler and should not be called directly</remarks>
		public void AppendFormatted<T>(T value)
		{
			var parameterName = string.Intern($"@p{_parameterIndex}");

			if (value is null)
				_parameters[_parameterIndex] = new SqliteParameter(parameterName, DBNull.Value);
			else
				_parameters[_parameterIndex] = new SqliteParameter(parameterName, value);
			_builder.Append(parameterName);

			_parameterIndex++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void ApplyParameters(SqliteCommand command)
		{
			if (_parameters.Count <= 0) return;


			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < _parameters.Count; i++) command.Parameters.Add(_parameters[i]);
			ArrayPool<SqliteParameter>.Shared.Return(_parameters.Array!, true);
		}

		/// <summary>
		///	Converts the string interpolation to a command object.
		/// </summary>
		/// <param name="connection">The connection to create the command for</param>
		/// <returns>The command object</returns>
		/// <remarks>This method is called internally and should not be called directly</remarks>
		public readonly SqliteCommand GetCommand(SqliteConnection connection)
		{
			var command = connection.CreateCommand(GetCommandText());

			ApplyParameters(command);
			return command;
		}

		private readonly string GetCommandText()
		{
			return _builder.ToString();
		}
	}
}