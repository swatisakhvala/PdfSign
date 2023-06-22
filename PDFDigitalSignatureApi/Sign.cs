using System.ComponentModel.DataAnnotations;

namespace PDFDigitalSignatureApi
{
    public class Sign
    {
        public IFormFile PdfFile { get; set; }
        //public IFormFile PfxFile { get; set; }
        [DataType(DataType.Password)]
        //public string Password { get; set; }
        public string location { get; set; }
        public string reason { get; set; }
        public string UserId { get; set; }
        public int PageX { get; set; } = 20;
        public int PageY { get; set; } = 10;
        public int Height { get; set; } = 170;
        public int Width { get; set; } = 60;

        public int PageNo { get; set; } = 1;
    }
}
