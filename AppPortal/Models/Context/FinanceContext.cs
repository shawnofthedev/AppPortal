using AppPortal.Models.Finance;
using Microsoft.EntityFrameworkCore;

namespace AppPortal.Models
{
    public partial class FinanceContext : DbContext
    {
        public FinanceContext()
        {
        }

        public FinanceContext(DbContextOptions<FinanceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CapFundingRequest> CapFundingRequests { get; set; }
        public virtual DbSet<FundingRequestAttachments> FundingRequestAttachments { get; set; }
        public virtual DbSet<StaggeredCost> StaggeredCosts { get; set; }
        public virtual DbSet<AttachedQuote> AttachedQuote { get; set; }
        public virtual DbSet<QuoteAttachments> QuoteAttachments { get; set; }
        public virtual DbSet<Approval> Approval { get; set; }

        //public DbSet<MunisVw_EmployeeMaster> MunisVw_EmployeeMaster { get; set; }
        //public DbSet<MunisVw_EmployeeAnnual> MunisVw_EmployeeAnnual { get; set; }
        public DbSet<Vw_DivisionMaster> Vw_DivisionMaster { get; set; }
        public DbSet<MunisVw_fa_master> MunisVw_Fa_Master { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<FundingRequestAttachments>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<StaggeredCost>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AttachedQuote>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Vw_DivisionMaster>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("id");
            });
        }
    }
}
