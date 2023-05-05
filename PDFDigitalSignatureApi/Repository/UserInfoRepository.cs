using PDFDigitalSignatureApi.Data;
using PDFDigitalSignatureApi.Model;

namespace PDFDigitalSignatureApi.Repository
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly DbContextClass _dbContext;
        public UserInfoRepository(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        public UserInfo AddSign(UserInfo userInfo)
        {
            var result = _dbContext.UserInfo.Add(userInfo);
            _dbContext.SaveChanges();
            return result.Entity;
        }

        public UserInfo GetSign(string userid)
        {
            var result = _dbContext.UserInfo.Where(x=>x.UserId == userid).FirstOrDefault();
            return result;
        }
    }
}
