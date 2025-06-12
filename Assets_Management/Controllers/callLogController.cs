using System.Data;
using AssetManagement_DataAccess;
using AssetManagement_EntityClass;
using Assets_Management.Services;
//using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assets_Management.Controllers
{
    public class CallLogController : Controller
    {
        private readonly ApiConnect apiConnect;
        private readonly IConfiguration configuration;
        private readonly CallLogs _callLogs;
        private readonly SQL_DB _DB;

        public CallLogController(ApiConnect apiConnect, IConfiguration configuration, CallLogs callLogs, SQL_DB dB)
        {
            this.apiConnect = apiConnect;
            this.configuration = configuration;
            this._callLogs = callLogs;
            _DB = dB;
        }

        public IActionResult AddCallLogs(string? Assetcode = null)
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            if (string.IsNullOrEmpty(Assetcode))
            {
                ViewBag.Message = "No AssetCode provided.";
            }
            return View();
        }

        public IActionResult AssignEng()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveTempraroyEngineer([FromBody] List<CallLogsEntity> assignments)
        {
            if (assignments == null || !assignments.Any())
            {
                return BadRequest(new { success = false, Message = "No Engineer Name Provided To Assign" });
            }
            try
            {
                var (success, errors) = await _callLogs.Engineers_Assig(assignments);
                string message = success ? "Engineers assigned successfully." : "No engineers were successfully assigned.";
                return Ok(new
                {
                    success,
                    Message = message,
                    errors = success ? null : errors
                });
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs($"Unexpected Error While Assigning Engineers: {ex.Message} \n {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAssignTempEngName([FromBody] CallLogsEntity Entitiy)
        {
            try
            {
                DataTable result = await _callLogs.GetTempEngData(Entitiy);
                if (result.Rows.Count > 0)
                {
                    var response = JsonConvert.SerializeObject(result);
                    return Ok(response);
                }
                else
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No engineer data found."
                    });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs($"Unexpected Error While Getting Temp Engineer Data: {ex.Message} \n {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAllocatedEngineers([FromBody] List<CallLogsEntity> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                return BadRequest(new { success = false, message = "No entities provided" });
            }
            try
            {
                var (successCount, errors) = await _callLogs.UpdateAllocatedEngineers(entities);
                if (successCount == entities.Count)
                {
                    return Ok(new { success = true, message = $"Successfully updated {successCount} engineer allocations" });
                }
                else if (successCount > 0)
                {
                    return Ok(new
                    {
                        success = false,
                        partialSuccess = true,
                        message = $"Updated {successCount} out of {entities.Count} engineer allocations",
                        errors = errors
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No engineer allocations were updated.",
                        errors = errors
                    });
                }
            }
            catch (Exception ex)
            {
                _DB.ExceptionLogs($"Unexpected error during bulk engineer update: {ex.Message} \n {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPremises()
        {
            DataTable Result = await _callLogs.GetPremisesAll();
            if (Result.Rows.Count > 0)
            {
                var Response = JsonConvert.SerializeObject(Result);
                return Ok(Response);
            }
            return NotFound(new { success = false, message = "No premises found." });
        }
    }
}