using System.Data;
using Microsoft.Data.SqlClient;

namespace AssetManagement_DataAccess
{
    public class SQL_DB
    {
        private readonly string _ConnectionString;
        private static readonly object lockObject = new object();

        public SQL_DB(string connectioString)
        {
            _ConnectionString = connectioString;
        }

        public void ExceptionLogs(string text)
        {
            try
            {
                string basePath = Path.Combine(Directory.GetCurrentDirectory(), "LogManager");
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                string currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                string logFilePath = Path.Combine(basePath, $"LogManager_{currentDate}.txt");
                string logMessage = $"{DateTime.UtcNow:yyyy/MM/dd HH:mm:ss} : {text}{Environment.NewLine}";
                lock (lockObject)
                {
                    File.AppendAllText(logFilePath, logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public async Task<DataTable> ExecuteProcedureAsync(string procedureName, Dictionary<string, object> parameters = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }
                        using (var dataAdapter = new SqlDataAdapter(cmd))
                        {
                            var dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    ExceptionLogs($"SQL Error: {sqlEx.Message}, Procedure: {procedureName}, StackTrace: {sqlEx.StackTrace}");
                    throw new ApplicationException($"Error while executing procedure: {procedureName}", sqlEx);
                }
                catch (Exception ex)
                {
                    ExceptionLogs($"General Error: {ex.Message}, Procedure: {procedureName}, StackTrace: {ex.StackTrace}");
                    throw new ApplicationException($"Unexpected error while executing procedure: {procedureName}", ex);
                }
            }
        }

        public async Task<int> ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (SqlException sqlEx)
                {
                    ExceptionLogs($"SQL Error: {sqlEx.Message}, Query: {query}, \n StackTrace: {sqlEx.StackTrace}");
                    throw new ApplicationException($"An error occurred while executing the query: {query}", sqlEx);
                }
                catch (Exception ex)
                {
                    ExceptionLogs($"General Error: {ex.Message}, Query: {query}, \n StackTrace: {ex.StackTrace}");
                    throw new ApplicationException($"Unexpected error occurred while executing the query: {query}", ex);
                }
            }
        }

        public async Task<DataSet> SP_ReturnDataSet(string ProcedureName, Dictionary<string, object> Parameters = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(ProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (Parameters != null)
                        {
                            foreach (var param in Parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }
                        using (var dataAdapter = new SqlDataAdapter(cmd))
                        {
                            var dataSet = new DataSet();
                            dataAdapter.Fill(dataSet);
                            return dataSet;
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    ExceptionLogs($"SQL Error: {sqlEx.Message}, \n StackTrace: {sqlEx.StackTrace}");
                    throw new ApplicationException($"Error while executing stored procedure: {ProcedureName}", sqlEx);
                }
                catch (Exception ex)
                {
                    ExceptionLogs($"General Error: {ex.Message}, \n StackTrace: {ex.StackTrace}");
                    throw new ApplicationException($"Unexpected error while executing stored procedure: {ProcedureName}", ex);
                }
            }
        }

        public async Task<DataTable> ExecuteQuerySelect(string query, Dictionary<string, object> parameters = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            return dataTable;
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    ExceptionLogs($"SQL Error: {sqlEx.Message}, \n StackTrace: {sqlEx.StackTrace}");
                    throw new ApplicationException($"Error while executing query: {query}", sqlEx);
                }
                catch (Exception ex)
                {
                    ExceptionLogs($"General Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                    throw new ApplicationException($"Unexpected error while executing query: {query}", ex);
                }
            }
        }

        public async Task<int> ExecuteScalar(string Query, Dictionary<string, object> parameters = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(Query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }
                        var result = await cmd.ExecuteScalarAsync();
                        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
                catch (SqlException sqlEX)
                {
                    ExceptionLogs($"{sqlEX.Message}, \n {sqlEX.StackTrace}");
                    throw new ApplicationException($"Error While Executing Query: {Query}", sqlEX);
                }
            }
        }

    }
}
