﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AppPortal.Models;
using AppPortal.Models.Finance;

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

        public virtual DbSet<FixedAsset> FixedAsset { get; set; }
        public virtual DbSet<CapFundingRequest> CapFundingRequests { get; set; }
        public virtual DbSet<FundingRequestAttachments> FundingRequestAttachments { get; set; }
        public virtual DbSet<StaggeredCost> StaggeredCosts { get; set; }
        public virtual DbSet<AttachedQuote> AttachedQuote { get; set; }
        public virtual DbSet<QuoteAttachments> QuoteAttachments { get; set; }
        public virtual DbSet<OrgChart> OrgChart { get; set; }
        public virtual DbSet<Manager> Manager { get; set; }
        public virtual DbSet<Division> Division { get; set; }
        public virtual DbSet<DivLead> DivLead { get; set; }
        public virtual DbSet<Analyst> Analyst { get; set; }


        public DbSet<MunisVw_EmployeeMaster> MunisVw_EmployeeMaster { get; set; }
        public DbSet<MunisVw_EmployeeAnnual> MunisVw_EmployeeAnnual { get; set; }
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


            modelBuilder.Entity<FixedAsset>(entity =>
            {
                entity.ToTable("fixedAsset");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AssetDept)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AssetDesc)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.AssetNum)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateOfRequest)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DisposalReason).HasMaxLength(50);

                entity.Property(e => e.MailedFrom).HasMaxLength(50);

                entity.Property(e => e.TransactType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TransferTo).HasMaxLength(50);
            });


            modelBuilder.Entity<Vw_DivisionMaster>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("id");
            });
        }
    }
}
