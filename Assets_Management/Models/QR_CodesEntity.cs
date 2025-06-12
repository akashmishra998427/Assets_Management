namespace Assets_Management.Models
{
    public class QR_CodesEntity
    {
        public bool isSelected { get; set; }
        public string Premises { get; set; }
        public string Machine_Code { get; set; }
        public string Asset_Code { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string Asset_Type { get; set; }
        public string ImageURL { get; set; }
        public List<QR_CodesEntity> GeneratedQRCodeList { get; set; } = new List<QR_CodesEntity>();
    }
}
