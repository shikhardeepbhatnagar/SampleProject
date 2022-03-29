using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WebAPI.Classes
{
	public class DatabaseManager
	{
		private SqlConnection _connection;

		public SqlCommand _command { get; private set; }

		/// <summary>
		/// Constructor : Initiate a connection to the database using the AppSettings dbconnectionString
		/// </summary>
		public DatabaseManager(string connectionKey)
		{
			_connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString);
			_connection.Open();
		}

		/// <summary>
		/// Prepare a new procedure
		/// </summary>
		/// <param name="procedure_name">name of the stored procedure to execute</param>
		public void prepareProcedure(string procedure_name)
		{
			_command = new SqlCommand(procedure_name, _connection);
			_command.CommandType = CommandType.StoredProcedure;
		}

		/// <summary>
		/// Add a parameter to the prepared procedure
		/// </summary>
		/// <param name="parameter_name">name of the parameter</param>
		/// <param name="sql_type">type of the parameter in sql format</param>
		/// <param name="value">value</param>
		public void addProcedureParameter(string parameter_name, SqlDbType sql_type, Object value)
		{
			_command.Parameters.Add(parameter_name, sql_type).Value = value;
		}

		public void Close()
		{
			_connection.Close();
		}

		public void Dispose()
		{
			_connection.Dispose();
		}

		public void addProcedureParameter(List<SqlParameter> param_collection)
		{
			foreach (SqlParameter elem in param_collection)
				_command.Parameters.Add(elem);
		}
	}
}