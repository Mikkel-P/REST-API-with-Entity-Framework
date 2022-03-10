using Microsoft.EntityFrameworkCore;
using API_ElevWiki.Models;
using Microsoft.AspNetCore.Http;

namespace API_ElevWiki.DataContext
{
    public class EFContext : DbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options) { }

        // Adds models to the Entity Framework context.
        #region Student
        public DbSet<Student> Students { get; set; }
        #endregion

        #region Abscence
        public DbSet<AbscenceRecord> Abscences { get; set; }
        #endregion

        #region Activity
        public DbSet<ActivityRecord> Actives { get; set; }
        #endregion

        #region LoginHolder
        public DbSet<LoginHolder> LoginHolders { get; set; }
        #endregion

        #region NfcCard
        public DbSet<NfcCard> NfcCards { get; set; }
        #endregion

        #region TimeRecord
        public DbSet<TimeRecord> TimeRecords { get; set; }
        #endregion
    }
}
