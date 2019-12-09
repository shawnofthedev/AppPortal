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
        public virtual DbSet<ManagerApproval> Approval { get; set; }
        public virtual DbSet<SecretaryApproval> SecretaryApproval { get; set; }
        public virtual DbSet<AnalystApproval> AnalystApproval { get; set; }
        public virtual DbSet<FleetApproval> FleetApproval { get; set; }
        public virtual DbSet<FinalApproval> FinalApproval { get; set; }

        //These Db sets are from read only views to be used for validation from ERP system
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
