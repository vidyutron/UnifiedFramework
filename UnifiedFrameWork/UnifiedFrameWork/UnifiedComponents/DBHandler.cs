using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.Controller
{
    class DBHandler
    {
        internal DataTable SqlSelectQuery(string connectionString,string sqlSelectAllQuery)
        {
            DataTable dataTable = new DataTable();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
            {
                // Create the command and set its properties
                sqlDataAdapter.SelectCommand = new SqlCommand();
                sqlDataAdapter.SelectCommand.Connection = new SqlConnection(connectionString);
                sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;
                // Assign the SQL to the command object
                sqlDataAdapter.SelectCommand.CommandText = sqlSelectAllQuery;
                // Fill the datatable from adapter
                sqlDataAdapter.Fill(dataTable);
            }
            return dataTable;
        }
        internal DataTable SqlFilterByIdQuery(string connectionString,string sqlFilterByIdQuery, int id)
        {
            DataTable dataTable = new DataTable();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
            {
                // Create the command and set its properties
                sqlDataAdapter.SelectCommand = new SqlCommand();
                sqlDataAdapter.SelectCommand.Connection = new SqlConnection(connectionString);
                sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;
                sqlDataAdapter.SelectCommand.CommandText = sqlFilterByIdQuery;
                // Add the parameter to the parameter collection
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@Id", id);
                // Fill the datatable from adapter
                sqlDataAdapter.Fill(dataTable);
            }
            return dataTable;
        }
        internal DataTable SqlSelectFilterQuery(string connectionString,
            Dictionary<string,string> filterCondition, string sqlSelectFilterQuery)
        {
            DataTable dataTable = new DataTable();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
            {
                // Create the command and set its properties
                sqlDataAdapter.SelectCommand = new SqlCommand();
                sqlDataAdapter.SelectCommand.Connection = new SqlConnection(connectionString);
                sqlDataAdapter.SelectCommand.CommandType = CommandType.Text;
                sqlDataAdapter.SelectCommand.CommandText = sqlSelectFilterQuery;
                if (filterCondition != null)
                {
                    foreach (KeyValuePair<string, string> singlePair in filterCondition)
                    {
                        sqlDataAdapter.SelectCommand.Parameters.AddWithValue(singlePair.Key, singlePair.Value);
                    }
                }
                // Fill the datatable from adapter
                sqlDataAdapter.Fill(dataTable);
            }
            return dataTable;
        }
        internal bool SqlInsertQuery(string connectionString,
            Dictionary<string,string> insertList,string sqlInsertQuery)
        {
            using (SqlCommand sqlDbCommand = new SqlCommand())
            {
                // Set the command object properties
                sqlDbCommand.Connection = new SqlConnection(connectionString);
                sqlDbCommand.CommandType = CommandType.Text;
                sqlDbCommand.CommandText = sqlInsertQuery;
                // Add the input parameters to the parameter collection
                foreach(KeyValuePair<string,string> singlePair in insertList )
                {
                    sqlDbCommand.Parameters.AddWithValue(singlePair.Key, singlePair.Value);
                }
                // Open the connection, execute the query and close the connection
                sqlDbCommand.Connection.Open();
                var rowsAffected = sqlDbCommand.ExecuteNonQuery();
                sqlDbCommand.Connection.Close();
                return rowsAffected > 0;
            }
        }

        internal bool SqlUpdateQuery(string connectionString,
            string sqlUpdateQuery,Dictionary<string, string> updateList= null)
        {
            using (SqlCommand sqlDbCommand = new SqlCommand())
            {
                // Set the command object properties
                sqlDbCommand.Connection = new SqlConnection(connectionString);
                sqlDbCommand.CommandType = CommandType.Text;
                sqlDbCommand.CommandText = sqlUpdateQuery;
                // Add the input parameters to the parameter collection
                if (updateList != null) { 
                foreach (KeyValuePair<string, string> singlePair in updateList)
                {
                    sqlDbCommand.Parameters.AddWithValue(singlePair.Key, singlePair.Value);
                }
                 }
                // Open the connection, execute the query and close the connection
                sqlDbCommand.Connection.Open();
                var rowsAffected = sqlDbCommand.ExecuteNonQuery();
                sqlDbCommand.Connection.Close();
                return rowsAffected > 0;
            }
        }

        internal DataTable SqlSP(string connectionString, Dictionary<string, string> parameters,
            string sqlStoredProcedure)
        {
            DataTable dataTable = new DataTable();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
            {
                // Create the command and set its properties
                sqlDataAdapter.SelectCommand = new SqlCommand();
                sqlDataAdapter.SelectCommand.Connection = new SqlConnection(connectionString);
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.CommandText = sqlStoredProcedure;
                if (parameters != null)
                {
                    foreach (KeyValuePair<string, string> singlePair in parameters)
                    {
                        sqlDataAdapter.SelectCommand.Parameters.AddWithValue(singlePair.Key, singlePair.Value);
                    }
                }
                // Fill the datatable from adapter
                sqlDataAdapter.Fill(dataTable);
            }
            return dataTable;
        }
    }
}
