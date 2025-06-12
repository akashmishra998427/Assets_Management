using System.Data;
using AssetManagement_EntityClass;
namespace AssetManagement_DataAccess
{
    public class DashboardReports
    {
        private readonly SQL_DB _SQL_DB;
        public DashboardReports(SQL_DB SQL_DB)
        {
            _SQL_DB = SQL_DB;
        }

        string Query = @"";

        public async Task<DataTable> GetAssetDetailsAsync(DashboardReportsEntity Entity)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Action",Entity.Action},
                {"@AssetType",Entity.AssetType},
                {"brand",Entity.Brand},
                {"PurchaseYear",Entity.PurchaseYear},
                {"unit",Entity.Unit},
                {"department",Entity.Department},
                {"model",Entity.Model},
                {"@processor",Entity.Processor} ,
                {"@Column",Entity.Col_Name}
            };
            return await _SQL_DB.ExecuteProcedureAsync("SP_assetDetails", parameters);
        }

        public async Task<DataTable> BindColumnNames()
        {
            Query = @"SELECT COLUMN_NAME AS [COLUMN_NAME] FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'asset'
                     AND COLUMN_NAME NOT LIKE 'Group' ORDER BY
                     CASE WHEN COLUMN_NAME LIKE '2003%' THEN 1
                     ELSE 0
                     END,
                     COLUMN_NAME"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindProcessor(DashboardReportsEntity Entity)
        {
            Query = @"SELECT DISTINCT ISNULL(PROCESSOR,'N/A') AS [Processor] FROM asset";
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(Entity.AssetType))
                filters.Add($"(@AssetType IS NULL OR @AssetType = '' OR Asset_Type = @AssetType)");

            if (filters.Count > 0)
                Query += " WHERE " + string.Join(" AND ", filters);

            var parameters = new Dictionary<string, object>
            {
                { "@AssetType", Entity.AssetType }
            };

            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> BindMake(DashboardReportsEntity Entity)
        {
            Query = @"SELECT DISTINCT ISNULL(MAKE,'N/A') AS [Brand] FROM asset";
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(Entity.AssetType))
                filters.Add($"(@AssetType IS NULL OR @AssetType = '' OR Asset_Type = @AssetType)");

            if (filters.Count > 0)
                Query += " WHERE " + string.Join(" AND ", filters);

            var parameters = new Dictionary<string, object>
            {
                { "@AssetType", Entity.AssetType }
            };

            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> BindDepartment(DashboardReportsEntity Entity)
        {
            Query = @"SELECT DISTINCT DEPT AS [Department] FROM asset";
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(Entity.AssetType))
                filters.Add($"(@AssetType IS NULL OR @AssetType = '' OR Asset_Type = @AssetType)");

            if (filters.Count > 0)
                Query += " WHERE " + string.Join(" AND ", filters);

            var parameters = new Dictionary<string, object>
            {
                { "@AssetType", Entity.AssetType }
            };

            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> BindUnit(DashboardReportsEntity Entity)
        {
            Query = @"SELECT DISTINCT INSTALLED_UNIT AS [UNIT] FROM asset";
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(Entity.AssetType))
                filters.Add($"(@AssetType IS NULL OR @AssetType = '' OR Asset_Type = @AssetType)");

            if (filters.Count > 0)
                Query += " WHERE " + string.Join(" AND ", filters);

            var parameters = new Dictionary<string, object>
            {
                { "@AssetType", Entity.AssetType }
            };

            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> BindPurchaseYear(DashboardReportsEntity Entity)
        {
            Query = @"SELECT DISTINCT ISNULL(PURCHASE_DATE_YEAR,'N/A') AS [YearPurchase]FROM asset";
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(Entity.AssetType))
                filters.Add($"(@AssetType IS NULL OR @AssetType = '' OR Asset_Type = @AssetType)");

            if (filters.Count > 0)
                Query += " WHERE " + string.Join(" AND ", filters);

            var parameters = new Dictionary<string, object>
            {
                { "@AssetType", Entity.AssetType }
            };

            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> BinModel(DashboardReportsEntity Entity)
        {
            Query = @" SELECT DISTINCT ISNULL(Model_Number, 'N/A') AS [model] FROM asset";
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(Entity.AssetType))
                filters.Add($"(@AssetType IS NULL OR @AssetType = '' OR Asset_Type = @AssetType)");

            if (filters.Count > 0)
                Query += " WHERE " + string.Join(" AND ", filters);

            var parameters = new Dictionary<string, object>
            {
                { "@AssetType", Entity.AssetType }
            };

            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> BindFilteredDatas(DashboardReportsEntity Entity)
        {
            string deptFilter;

            if (Entity.Action == "AssetInStock")
                deptFilter = "DEPT LIKE 'EDP STOCK'";
            else if (Entity.Action == "AssetAllocated")
                deptFilter = "DEPT NOT LIKE 'EDP STOCK'";
            else if (Entity.Action == "AssetRoom")
                deptFilter = "DEPT = 'SCRAP IN STOCK'";
            else
                deptFilter = "1=1";

            var filters = new List<string>
            {
                deptFilter,
            };

            if (!string.IsNullOrWhiteSpace(Entity.AssetType))
                filters.Add($"(Asset_Type IS NULL OR Asset_Type = '' OR Asset_Type = '{Entity.AssetType}')");

            if (!string.IsNullOrWhiteSpace(Entity.PurchaseYear))
                filters.Add($"PURCHASE_DATE_YEAR IN ({Entity.PurchaseYear})");

            if (!string.IsNullOrWhiteSpace(Entity.Processor))
                filters.Add($"PROCESSOR IN ({Entity.Processor})");

            if (!string.IsNullOrWhiteSpace(Entity.Brand))
                filters.Add($"MAKE IN ({Entity.Brand})");

            if (!string.IsNullOrEmpty(Entity.Model))
                filters.Add($"Model_Number IN ({Entity.Model})");

            if (!string.IsNullOrWhiteSpace(Entity.Unit))
                filters.Add($"INSTALLED_UNIT IN ({Entity.Unit})");

            if (!string.IsNullOrWhiteSpace(Entity.Department))
                filters.Add($"DEPT IN ({Entity.Department})");

            Query = $"SELECT {Entity.Col_Name} FROM asset WHERE {string.Join(" AND ", filters)}";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindCounting(DashboardReportsEntity Entity)
        {
            var Parameters = new Dictionary<string, object>
            {
                { "@AssetType",Entity.AssetType},
                { "@brand",Entity.Brand},
                { "@PurchaseYear",Entity.PurchaseYear},
                { "@unit",Entity.Unit},
                { "@department",Entity.Department},
                { "@model",Entity.Model},
                { "@processor",Entity.Processor},
            };
            return await _SQL_DB.ExecuteProcedureAsync("sp_PendingDashboard_Asset", Parameters);
        }

        public async Task<DataTable> BindDetailsRow(DashboardReportsEntity Entity)
        {
            var parameter = new Dictionary<string, object>()
            {
                {"@assetCode",Entity.AssetCoode }
            };
            Query = @"SELECT * FROM asset WHERE CPU_ASSET_CODE = @assetCode";
            return await _SQL_DB.ExecuteQuerySelect(Query, parameter);
        }

        public async Task<DataTable> BindInvoiceDetails(DashboardReportsEntity Entity)
        {
            var Parameters = new Dictionary<string, object> {
               {"@AssetType",Entity.AssetType}
            };
            Query = @"SELECT
                        CPU_ASSET_CODE,
                        Model_Number,
                        Ip_No,
                        ISNULL(UPTO,'') AS [UPTO],        
                        Bill_No,
						Bill_Date,
                        [USER_NAME],
                        Asset_Type, 
                        MAKE,
                        PURCHASE_DATE_YEAR,
                        DEPT, INSTALLED_UNIT,
                        Machine_Sl_No
                FROM asset  WHERE Asset_Type IS NULL OR Asset_Type = '' OR Asset_Type = @AssetType 
                AND ISNULL(Invoice_Tag,'') <> 'Y' "
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query, Parameters);
        }

        public async Task<int> SaveAttachedInvoice(DashboardReportsEntity Entity, List<string> selectedAssetCodes)
        {
            int totalAffectedRows = 0;

            foreach (var assetCode in selectedAssetCodes)
            {
                try
                {
                    var parameters = new Dictionary<string, object>
                    {
                       {"@BillNO", Entity.InvoiceNumber},
                       {"@BillDate", Entity.Date},
                       {"@Asset_Code", assetCode},
                    };
                    Query = @"UPDATE asset  
                           SET [Invoice_Tag] = 'Y',  
                               [Bill_No] = @BillNO,  
                               [Bill_Date] = @BillDate  
                           WHERE CPU_ASSET_CODE = @Asset_Code"
                    ;
                    int affectedRows = await _SQL_DB.ExecuteNonQuery(Query, parameters);
                    totalAffectedRows += affectedRows;
                }
                catch (Exception ex)
                {
                    _SQL_DB.ExceptionLogs($"Error updating asset code {assetCode}: {ex.Message}");
                }
            }
            return totalAffectedRows;
        }
    }
}
