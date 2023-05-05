
namespace PDFDigitalSignatureApi.Model
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PfxFile { get; set; }
        public string Password { get; set; }
        public DateTime CreatedOn { get; set; }

    }

    public class UserInfoDto
    {
        public string UserId { get; set; }
        public IFormFile PfxFile { get; set; }
        public string Password { get; set; }

    }
}
