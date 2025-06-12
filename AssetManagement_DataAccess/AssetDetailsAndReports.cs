using System.Data;
using AssetManagement_EntityClass;

namespace AssetManagement_DataAccess
{
    public class AssetDetailsAndReports
    {
        private readonly SQL_DB _SQL_DB;

        public AssetDetailsAndReports(SQL_DB sQl_DB)
        {
            _SQL_DB = sQl_DB;
        }

        string Query = @"";

        public async Task<DataTable> AssetUpdations()
        {
            Query = @"SELECT 
                      CPU_ASSET_CODE,INSTALLED_UNIT,USER_NAME,DEPARTMENT,Employee_Code,Asset_Type,ID AS Status 
                      FROM Asset where Asset_Type in ('C')"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindAssetDetals(Dictionary<string, object> parameters = null)
        {
            Query = @"Select 
                      CPU_ASSET_CODE,Employee_Code,Asset_Type,FORMAT(Last_Patch_Update ,'yyyy-MM-dd' ) AS Last_Patch_Update,
                      FORMAT(Last_Anti_Virus ,'yyyy-MM-dd' )Last_Anti_Virus,
                      FORMAT(Last_Archive ,'yyyy-MM-dd' )Last_Archive from Asset where ID = @ID"
            ;
            return await _SQL_DB.ExecuteQuerySelect(Query, parameters);
        }

        public async Task<DataTable> GetDepartments()
        {
            Query = "Select DEPT from asset Group by DEPT ORDER BY DEPT";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> GetModel_No()
        {
            Query = @"SELECT Model_Number FROM asset GROUP BY Model_Number ORDER BY Model_Number";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindMoles()
        {
            Query = @"SELECT MAKE FROM asset GROUP BY MAKE ORDER BY MAKE";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> ReportingHead()
        {
            Query = @"SELECT ISNULL(Reporting_Head,'') AS Reporting_Head FROM asset  GROUP BY Reporting_Head  ORDER BY Reporting_Head";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindUnits()
        {
            Query = @"SELECt INSTALLED_UNIT  FROM asset  GROUP BY INSTALLED_UNIT";
            return await _SQL_DB.ExecuteQuerySelect(Query);
        }

        public async Task<DataTable> BindReports(AssetEntity Entity)
        {
            if (Entity.Fillter == "Computer")
            {
                Entity.Type = "3";
            }
            var Parameters = new Dictionary<string, object>
            {
                {"@Fillter",Entity.Fillter},
                {"@ReportType",Entity.ReportType} ,
                {"@GROUPBy" ,Entity.GROUPBy} ,
                {"@UNIT " ,Entity.UNIT} ,
                {"@Dept" ,Entity.Dept},
                {"@MAKE ",Entity.MAKE} ,
                {"@MODELNO",Entity.MODELNO},
                {"@REPORTINGHEAD",Entity.REPORTINGHEAD},
                {"Detailed",Entity.Type} ,
                {"@AssetType",Entity.AssetType}
            };
            return await _SQL_DB.ExecuteProcedureAsync("Sp_SoftwareReport", Parameters);
        }

        public async Task<DataTable> BindReportDetail(AssetEntity Entity)
        {
            if (Entity.Fillter == "Computer")
            {
                Entity.GroupCPU_V = Entity.SwName;
                Entity.SwName = "";
                Entity.Fillter = "";
                Entity.AssetDetail = "4";
            }
            if (Entity.Scrap == "1")
            {
                Entity.AssetDetail = "6";
            }
            if (Entity.ColName != "")
            {
                Entity.AssetDetail = "8";
            }
            var Parameters = new Dictionary<string, object>
            {
               {" @Fillter",Entity.Fillter},
               {" @ReportType",Entity.ReportType},
               {" @GROUPBy",Entity.GROUPBy},
               {" @UNIT",Entity.UNIT},
               {" @Dept",Entity.Dept},
               {" @MAKE",Entity.MAKE},
               {" @MODELNO",Entity.MODELNO},
               {" @REPORTINGHEAD",Entity.REPORTINGHEAD},
               {" @Detailed",Entity.Type},
               {" @SwName",Entity.SwName},
               {" @GroupUnit",Entity.GroupUnit},
               {" @GroupDept",Entity.GroupDept},
               {" @GroupUser",Entity.GroupUser},
               {" @GroupAsset",Entity.GroupAsset},
               {" @GroupCPU_V",Entity.GroupCPU_V},
               {" @AssetType",Entity.AssetType},
            };
            return await _SQL_DB.ExecuteProcedureAsync("Sp_SoftwareReport", Parameters);
        }

        public async Task<int> UpdateAssetDetal(AssetEntity Entity)
        {
            var Parameters = new Dictionary<string, object>
            {
                {"@Last_Patch_Update", Entity.Last_Patch},
                {"@Last_Anti_Virus", Entity.ILast_Anti_VirusD},
                {"@Last_Archive", Entity.Last_Archive},
                {"@ID",Entity.ID}
            };
            Query = @"UPDATE Asset SET Last_Patch_Update = @Last_Patch_Update, 
                      Last_Anti_Virus = @Last_Anti_Virus,
                      Last_Archive = @Last_Archive
                      WHERE ID = @ID"
            ;
            return await _SQL_DB.ExecuteNonQuery(Query, Parameters);
        }
    }
}

