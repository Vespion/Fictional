using Fictional.Data.Core.Migrations;
using Fictional.Data.Core.Models.Tags;
using Fictional.Data.Core.Sql;
using Fictional.TestUtils.Data.Models;
using Microsoft.Data.Sqlite;

namespace Fictional.TestUtils;

// ReSharper disable once MissingXmlDoc
public static class TestHelpers
{
	public static async Task<SqliteConnection> ProvisionDatabase(SqliteConnection? conn = null)
	{
		conn ??= new SqliteConnection("Data Source=:memory:");

		conn.OpenAndInitalize();

		var mgr = new SqliteMigrationsManager(conn);

		await mgr.ApplyLatestMigrations(TestContext.Current.CancellationToken);

		return conn;
	}

	public static void AddTag(SqliteConnection conn, Tag tag)
	{
		using var command =
			conn.CreateCommand(
				"INSERT INTO Tags (Id, Name, Shorthand, Colour, Hidden) VALUES (@id, @name, @short, @colour, @hidden)");

		command.Parameters.AddWithValue("@id", tag.Id.Value);
		command.Parameters.AddWithValue("@name", tag.Name);

		if(tag.Shorthand == null)
		{
			command.Parameters.AddWithValue("@short", DBNull.Value);
		}
		else
		{
			command.Parameters.AddWithValue("@short", tag.Shorthand);
		}

		if (tag.Colour != null)
		{
			command.Parameters.AddWithValue("@colour", (int?)tag.Colour);
		}
		else
		{
			command.Parameters.AddWithValue("@colour", DBNull.Value);
		}

		command.Parameters.AddWithValue("@hidden", tag.Hidden);

		command.ExecuteNonQuery();
	}

	public static void AddTags(SqliteConnection conn, IEnumerable<SerializableTag> tags)
	{
		using var command =
			conn.CreateCommand(
				"INSERT INTO Tags (Id, Name, Shorthand, Colour, Hidden) VALUES (@id, @name, @short, @colour, @hidden)");
		var idParam = command.CreateParameter();
		idParam.ParameterName = "@id";
		command.Parameters.Add(idParam);
		var nameParam = command.CreateParameter();
		nameParam.ParameterName = "@name";
		command.Parameters.Add(nameParam);
		var shortParam = command.CreateParameter();
		shortParam.ParameterName = "@short";
		command.Parameters.Add(shortParam);
		var colourParam = command.CreateParameter();
		colourParam.ParameterName = "@colour";
		command.Parameters.Add(colourParam);
		var hiddenParam = command.CreateParameter();
		hiddenParam.ParameterName = "@hidden";
		command.Parameters.Add(hiddenParam);

		foreach (var tag in tags)
		{
			idParam.Value = tag.Id.Value;
			nameParam.Value = tag.Name;

			if(tag.Shorthand == null)
			{
				shortParam.Value = DBNull.Value;
			}
			else
			{
				shortParam.Value = tag.Shorthand;
			}

			if (tag.Colour != null)
			{
				colourParam.Value = (int?)tag.Colour;
			}
			else
			{
				colourParam.Value = DBNull.Value;
			}

			hiddenParam.Value = tag.Hidden;
			command.ExecuteNonQuery();
		}
	}

	public static bool TagExists(SqliteConnection conn, TagId id)
	{
		return conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Tags WHERE Id = {id.Value}") == 1;
	}
}