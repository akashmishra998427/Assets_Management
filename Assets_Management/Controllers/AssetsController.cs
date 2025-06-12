using System.Data;
using System.Drawing;
using AssetManagement_DataAccess;
using AssetManagement_EntityClass;
using Assets_Management.Models;
using Assets_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;

namespace Assets_Management.Controllers
{
    public class AssetsController : Controller
    {
        private readonly ApiConnect apiConnect;
        private readonly IConfiguration configuration;
        private readonly AssetDetailsAndReports _Detail;

        public AssetsController(ApiConnect apiConnect, IConfiguration configuration, AssetDetailsAndReports Detail)
        {
            this.apiConnect = apiConnect;
            this.configuration = configuration;
            this._Detail = Detail;
        }

        public IActionResult genrateAssetQR()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        //method to genrate Qr codes starts
        [HttpPost]
        public IActionResult genrateAssetQR([FromBody] QR_CodesEntity CheckAassetData)
        {
            foreach (var item in CheckAassetData.GeneratedQRCodeList)
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                string code = $"https://support.richagroup.com/CallLog/AddCallLogs?AssetCode={item.Asset_Code.Replace("/", "-")}&assetType={item.Asset_Type}";
                QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                {
                    string base64 = Convert.ToBase64String(BitmapToByteArray(qrBitmap));
                    item.ImageURL = "data:image/png;base64," + base64;
                }
            }
            var result = CheckAassetData.GeneratedQRCodeList.ToList();
            return Json(result);
        }
        //method to genrate Qr codes ends
        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public IActionResult Audit_PC_Data()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult transferAssets()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult invoiceEntry()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult GENERATEQRPDF()
        {
            return View();
        }

        public IActionResult Update(string? ID)
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult ScrapAsset()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadScrapImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No File Selected");
            }
            var UploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ScrapedImage");
            if (!Directory.Exists(UploadPath))
            {
                Directory.CreateDirectory(UploadPath);
            }
            var FileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var FilePath = Path.Combine(UploadPath, FileName);
            using (var Stream = new FileStream(FilePath, FileMode.Create))
            {
                await image.CopyToAsync(Stream);
            }
            return Ok(new { Message = "Image Uploaded Successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> AssetUpdation()
        {
            DataTable Result = await _Detail.AssetUpdations();

            if (Result.Rows.Count > 0)
            {
                var Response = JsonConvert.SerializeObject(Result);
                return Ok(Response);
            }
            return NotFound(new { success = false, message = "Data Not found." });
        }

        [HttpPost]
        public async Task<IActionResult> AssetDetalUpdation([FromBody] AssetEntity Entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID",Entity.ID}
            };

            DataTable Result = await _Detail.BindAssetDetals(parameters);
            if (Result.Rows.Count > 0)
            {
                var Response = JsonConvert.SerializeObject(Result);
                return Ok(new
                {
                    Data = Response,
                });
            }
            return NotFound(new { success = false, message = "Data Not found." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAssetDetails([FromBody] AssetEntity Entity)
        {
            try
            {
                int Result = await _Detail.UpdateAssetDetal(Entity);
                if (Result > 0)
                {
                    return Ok(new { message = "Asset Detail Updated Successfully" });
                }
                else
                {
                    return NotFound(new { message = "No Assets Found To Update" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

