using System.Data;
using AssetManagement_DataAccess;
using AssetManagement_EntityClass;
using Assets_Management.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Assets_Management.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApiConnect _apiConnect;
        private readonly DashboardReports _dashboardReports;
        private readonly SQL_DB _SQL_DB;
        private DataTable Result;
        public DashboardController(IConfiguration configuration, ApiConnect apiConnect, DashboardReports dashboardReports, SQL_DB sQL_DB)
        {
            _configuration = configuration;
            _apiConnect = apiConnect;
            _dashboardReports = dashboardReports;
            this._SQL_DB = sQL_DB;
            Result = new DataTable();
        }

        public IActionResult Dashboard(string? Action, string? AssetType)
        {
            try
            {
                ViewBag.ApiBasurl = _apiConnect?.WebApiUrl() ?? "DefaultUrl";
                return View();
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        public IActionResult Production()
        {
            ViewBag.ApiBasurl = _apiConnect.WebApiUrl();
            return View();
        }

        public IActionResult AttachInvoice(string? AssetType, string? MenuID)
        {
            return View();
        }

        public IActionResult DetailPage(string AssetCode)
        {
            ViewBag.RootUrl = _configuration["Apisettings:RootUrl"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAssetCounts_AndDetails([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.GetAssetDetailsAsync(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No asset details found.");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs($"Unexpected Error Occured While Processing{ex.Message} \n {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadToExcel([FromBody] DashboardReportsEntity Entity)
        {
            Result = await _dashboardReports.GetAssetDetailsAsync(Entity);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Asset Details");
                worksheet.Cell(1, 1).InsertTable(Result);
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), $"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Entity.Action}.xlsx");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindColumns()
        {
            try
            {
                Result = await _dashboardReports.BindColumnNames();
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No column names  found.");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindProcessor([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindProcessor(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Processor not found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindMake([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindMake(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Make not Fount");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindDepartment([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindDepartment(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Department found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindUnit([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindUnit(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Unit Not Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindFilteredData([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindFilteredDatas(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindPurchaseYear([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindPurchaseYear(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BinModel(DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BinModel(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindCounting([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindCounting(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindRowDetails([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindDetailsRow(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Data Not Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindInvoiceDetails([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindInvoiceDetails(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Data Not Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttachedInvoice([FromForm] DashboardReportsEntity Entity, [FromForm] IFormFile ImageFile, [FromForm] List<string> SelectedAssetCodes)
        {
            try
            {
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    return BadRequest("Please upload asset invoice.");
                }

                if (SelectedAssetCodes == null || SelectedAssetCodes.Count == 0)
                {
                    return BadRequest("Please select at least one asset.");
                }

                var uploadLocation = @"D:\Asset_Invoice";
                if (!Directory.Exists(uploadLocation))
                {
                    Directory.CreateDirectory(uploadLocation);
                }

                var fileLocation = Path.Combine(uploadLocation, Path.GetFileName(ImageFile.FileName));

                using (var stream = new FileStream(fileLocation, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                int Result = await _dashboardReports.SaveAttachedInvoice(Entity, SelectedAssetCodes);

                if (Result > 0)
                {
                    return Ok($"Invoice attached successfully for {SelectedAssetCodes} asset(s).");
                }
                else
                {
                    return BadRequest("Failed to save invoice.");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}


