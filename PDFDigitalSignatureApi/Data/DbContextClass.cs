using Microsoft.EntityFrameworkCore;
using PDFDigitalSignatureApi.Model;
using System.Collections.Generic;

namespace PDFDigitalSignatureApi.Data
{
    public class DbContextClass : DbContext
    {
        protected readonly IConfiguration configuration;

        public DbContextClass(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnnection"));
        }
        public DbSet<UserInfo> UserInfo { get; set; }
    }
}
