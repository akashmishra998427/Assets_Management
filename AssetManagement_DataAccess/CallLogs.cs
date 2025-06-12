using System.Data;
using AssetManagement_EntityClass;

namespace AssetManagement_DataAccess
{
    public class CallLogs
    {
        private readonly SQL_DB _SQL_DB;

        public CallLogs(SQL_DB SQL_DB)
        {
            _SQL_DB = SQL_DB;
        }

        string Query = "";

        public async Task<(bool success, List<string> errors)> Engineers_Assig(List<CallLogsEntity> assignments)
        {
            bool anySuccess = false;
            List<string> errors = new List<string>();

            foreach (var entity in assignments)
            {
                try
                {
                    var parameters = new Dictionary<string, object>
                    {
                        {"@AllocationDate", entity.AllocationDate},
                        {"@EngName", entity.EngName},
                        {"@Created_By", entity.Created_By},
                        {"@CompCode", entity.Compcode}
                    };
                    string Query = @"
                            SELECT COUNT(*) FROM tbl_EnginfoAllocation 
                            WHERE CompCode = @CompCode AND [DATE] = @AllocationDate"
                    ;
                    var exists = Convert.ToInt32(await _SQL_DB.ExecuteScalar(Query, parameters));
                    Query = exists > 0
                               ? @"UPDATE tbl_EnginfoAllocation
                                SET EngName = @EngName, Updated_Date = GETDATE()
                                WHERE CompCode = @CompCode AND [DATE] = @AllocationDate"
                                 : @"INSERT INTO tbl_EnginfoAllocation (
                                    [DATE], EngName, Created_Date, Created_By, CompCode)
                                    VALUES (@AllocationDate, @EngName, GETDATE(), @Created_By, @CompCode);";
                    int result = await _SQL_DB.ExecuteNonQuery(Query, parameters);
                    if (result > 0)
                    {
                        anySuccess = true;
                    }
                    else
                    {
                        errors.Add($"No rows affected for {entity.EngName}.");
                        _SQL_DB.ExceptionLogs($"No rows affected for engineer {entity.EngName}, CompCode {entity.Compcode}.");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to assign engineer {entity.EngName}: {ex.Message}");
                    _SQL_DB.ExceptionLogs($"Error assigning engineer {entity.EngName}: {ex.Message}\n{ex.StackTrace}");
                }
            }
            return (anySuccess, errors);
        }

        public async Task<DataTable> GetTempEngData(CallLogsEntity Entity)
        {
            var Parameters = new Dictionary<string, object>
            {
                { "@CompCode", Entity.Compcode }
            };
            Query = "SELECT * FROM tbl_EnginfoAllocation WHERE CompCode = @CompCode";
            return await _SQL_DB.ExecuteQuerySelect(Query, Parameters);
        }

        public async Task<(int successCount, List<string> errors)> UpdateAllocatedEngineers(List<CallLogsEntity> entities)
        {
            int successCount = 0;
            List<string> errors = new List<string>();
            foreach (var entity in entities)
            {
                try
                {
                    var parameters = new Dictionary<string, object>
                    {
                        { "@EngineerName", entity.EngName },
                        { "@CompCode", entity.Compcode }
                    };
                    Query = @"
                            UPDATE PREMISES
                            SET EngName = @EngineerName
                            WHERE CompCode = @CompCode 
                            AND Work_Prod = 'Y' 
                            AND Active = 1"
                    ;
                    int result = await _SQL_DB.ExecuteNonQuery(Query, parameters);
                    if (result > 0)
                    {
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"No rows updated for CompCode {entity.Compcode}");
                        _SQL_DB.ExceptionLogs($"No rows affected for engineer update: CompCode {entity.Compcode}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error updating engineer for {entity.Compcode}: {ex.Message}");
                    _SQL_DB.ExceptionLogs($"Exception updating engineer: {ex.Message} \n {ex.StackTrace}");
                }
            }
            return (successCount, errors);
        }

        public async Task<DataTable> GetPremisesAll()
        {
            Query = @"Select N_CompCode AS [n_CompCode],
                       CompCode AS [premises],
                       EngName AS [engName] 
                       from PREMISES 
                       where Work_Prod ='Y' 
                       and Active = 1 Order by N_CompCode"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }
    }
}