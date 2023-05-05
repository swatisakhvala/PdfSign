using PDFDigitalSignatureApi.Model;

namespace PDFDigitalSignatureApi.Repository
{
    public interface IUserInfoRepository
    {
        public UserInfo AddSign(UserInfo userInfo);

        UserInfo GetSign(string userid);
    }
}
