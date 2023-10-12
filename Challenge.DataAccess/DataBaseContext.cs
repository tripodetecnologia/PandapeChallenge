using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Challenge.DataAccess.Model;

namespace Challenge.DataAccess
{
    public class DataBaseContext : DbContext
    {
 
        protected readonly IConfiguration _configuration;

        #region Cosntructor
        public DataBaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {            
            options.UseSqlServer(_configuration.GetConnectionString("DataBaseCon"));
        }

        #region DB SET
        public virtual DbSet<Candidates> Candidates { get; set; }
        public virtual DbSet<CandidateExperiences> CandidateExperiences { get; set; }        

        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        #endregion        
    }
}