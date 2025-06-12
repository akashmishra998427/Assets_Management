using System.Data;
using AssetManagement_DataAccess;
using AssetManagement_EntityClass;
using Assets_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assets_Management.Controllers
{
    public class AssetsReportController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApiConnect _apiConnect;
        private readonly AssetDetailsAndReports Reports;
        private readonly SQL_DB _DB;

        public AssetsReportController(IConfiguration configuration, ApiConnect apiConnect, AssetDetailsAndReports reports, SQL_DB DB)
        {
            _configuration = configuration;
            _apiConnect = apiConnect;
            _DB = DB;
            Reports = reports;

        }

        public IActionResult Summary(string Filter, string? ReportType, string Type)
        {
            ViewBag.ApiBasurl = _apiConnect.CoreApiUrl();
            ViewBag.RootUrl = _configuration["Apisettings:RootUrl"];
            return View();
        }

        public IActionResult ManageConfig()
        {
            ViewBag.ApiBasurl = _apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult Asset_Transfer()
        {
            ViewBag.ApiBasurl = _apiConnect.CoreApiUrl();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartment()
        {
            try
            {
                DataTable result = await Reports.GetDepartments();
                if (result != null && result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(result);
                    return Ok(Response);
                }
                else
                {
                    return Json(new { success = false, message = "No departments found." });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs(ex.ToString());
                return Json(new { success = false, message = "An error occurred while fetching departments." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindModalNo()
        {
            try
            {
                DataTable Result = await Reports.GetModel_No();
                if (Result != null && Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return Json(new { success = false, message = "No ModelNo found." });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs(ex.ToString());
                return Json(new { success = false, message = "An error occurred while fetching ModelNo." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindMake()
        {
            try
            {
                DataTable Result = await Reports.BindMoles();
                if (Result != null && Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return Json(new { success = false, message = "No Make Found" });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs(ex.ToString());
                return Json(new { success = false, message = "An error occurred while fetching MAKE" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindReportingHead()
        {
            try
            {
                DataTable Result = await Reports.ReportingHead();
                if (Result != null && Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return Json(new { success = false, message = "No Reporting Head Found" });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs(ex.ToString());
                return Json(new { success = false, message = "An error occurred while fetching ReportingHead" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindUnit()
        {
            try
            {
                DataTable Result = await Reports.BindUnits();
                if (Result != null && Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return Json(new { success = false, message = "No units found" });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs(ex.ToString());
                return Json(new { success = false, message = "An error occurred while fetching Units" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindReport([FromBody] AssetEntity Entity)

        {
            try
            {
                DataTable Result = await Reports.BindReports(Entity);
                if (Result != null && Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return Json(new { success = false, message = "No data found" });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs(ex.ToString());
                return Json(new { success = false, message = "An error occurred while fetching Report" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindReportDetails([FromBody] AssetEntity Entity)
        {
            try
            {
                DataTable Result = await Reports.BindReportDetail(Entity);
                if (Result != null && Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return Json(new { success = false, message = "No data found" });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs(ex.ToString());
                return Json(new { success = false, message = "An error occurred while fetching Report Details" });
            }
        }
    }
}

