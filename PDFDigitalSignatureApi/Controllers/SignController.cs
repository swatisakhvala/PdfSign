using Microsoft.AspNetCore.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using System.Diagnostics;
using PDFDigitalSignatureApi.Model;
using PDFDigitalSignatureApi.Repository;


namespace PDFDigitalSignatureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {

        private readonly IUserInfoRepository _userInfoRepository;

        public SignController(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }
        [HttpPost]
        public IActionResult Post([FromForm] Sign data)
        {
            try
            {
                var GetPfx = _userInfoRepository.GetSign(data.UserId);
                byte[] bytes = System.Convert.FromBase64String(GetPfx.PfxFile);
                Stream stream = new MemoryStream(bytes);

                string path = Directory.GetCurrentDirectory();
                //string path = Path.Combine("https://digitalsignpdf.azurewebsites.net/", "wwwroot/Files"); 

                string newPdfFilePath1 = Path.Combine(path, "new" + DateTime.Now.Ticks + ".pdf");

                //var streampfx = data.PfxFile.OpenReadStream();

                signPdfFile(data.PdfFile, newPdfFilePath1, stream, GetPfx.Password, data.reason, data.location, data);

                byte[] fileBytes = System.IO.File.ReadAllBytes(newPdfFilePath1);
                var dataStream = new MemoryStream(fileBytes);
                // return fileBytes;

                HttpContext.Response.ContentType = "application/pdf";
                FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(newPdfFilePath1), "application/pdf")
                {
                    FileDownloadName = "SignedFile" + DateTime.Now + ".pdf"
                };

                return result;


            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                var line = frame.GetFileLineNumber();
                return StatusCode(500, "Internal server errors:" + ex.ToString() + " sdsd " + Directory.GetCurrentDirectory());
            }
        }


        public static void signPdfFile(IFormFile pdffile, string destinationPath, Stream privateKeyStream, string keyPassword, string reason, string location, Sign data)
        {
            Org.BouncyCastle.Pkcs.Pkcs12Store pk12 = new Org.BouncyCastle.Pkcs.Pkcs12Store(privateKeyStream, keyPassword.ToCharArray());
            privateKeyStream.Dispose();

            //then Iterate throught certificate entries to find the private key entry
            string alias = null;
            foreach (string tAlias in pk12.Aliases)
            {
                if (pk12.IsKeyEntry(tAlias))
                {
                    alias = tAlias;
                    break;
                }
            }
            var pk = pk12.GetKey(alias).Key;

            //var streampfx = pfxFile.OpenReadStream();
            //byte[] bytes = new byte[pfxFile.Length];
            //streampfx.Read(bytes, 0, (int)pfxFile.Length);

            //X509Certificate2 cert1 = new X509Certificate2(bytes, keyPassword, X509KeyStorageFlags.Exportable);
            //RSA legacyProv = (RSA)cert1.PrivateKey;
            //RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            //provider.ImportParameters(legacyProv.ExportParameters(true));
            //var psdk = Org.BouncyCastle.Security.DotNetUtilities.GetRsaKeyPair(provider).Private;

            // reader and stamper
            var streampdf = pdffile.OpenReadStream();
            

            PdfReader reader = new PdfReader(streampdf);
            using (FileStream fout = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite))
            {
                PdfStamper stamper = PdfStamper.CreateSignature(reader, fout, '\0');
                // appearance
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                //appearance.Image = new iTextSharp.text.pdf.PdfImage();
                appearance.Reason = reason;
                appearance.Location = location;
                appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(data.PageX, data.PageY, data.Height, data.Width), reader.NumberOfPages, "signature");
                // digital signature
                IExternalSignature es = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
                MakeSignature.SignDetached(appearance, es, new Org.BouncyCastle.X509.X509Certificate[] { pk12.GetCertificate(alias).Certificate }, null, null, null, 0, CryptoStandard.CMS);
                stamper.Close();
            }

        }

        [HttpPost("AddPfxFile")]
        public dynamic AddPfxFile([FromForm] UserInfoDto userInfo)
        {
            try
            {
                //Byte[] bytes = System.IO.File.ReadAllBytes(userInfo.PfxFile);

                var streampfx = userInfo.PfxFile.OpenReadStream();
                byte[] bytes = new byte[userInfo.PfxFile.Length];
                streampfx.Read(bytes, 0, (int)userInfo.PfxFile.Length);

                UserInfo user = new UserInfo();
                user.Password = userInfo.Password;
                user.CreatedOn = DateTime.Now;
                user.UserId = userInfo.UserId;
                user.PfxFile = Convert.ToBase64String(bytes);

                var result = _userInfoRepository.AddSign(user);
                return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
