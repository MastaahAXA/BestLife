using Microsoft.EntityFrameworkCore;
using BestLife.Models;
using System;

namespace BestLife.Data
{
    public class BestLifeDbContext : DbContext
    {
        public BestLifeDbContext(DbContextOptions<BestLifeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Registration> Registration { get; set; } = default!;
        public DbSet<Members> Members { get; set; } = default!;
        public DbSet<Growfund> Growfund { get; set; } = default!;
        public DbSet<MemberPayment> MemberPayments { get; set; } = default!;
        public DbSet<MemberEarnings> MemberEarnings { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table mappings
            modelBuilder.Entity<Registration>().ToTable("Registration");
            modelBuilder.Entity<Members>().ToTable("Members");
            modelBuilder.Entity<Growfund>().ToTable("Growfund");
            modelBuilder.Entity<MemberPayment>().ToTable("MemberPayments");
            modelBuilder.Entity<MemberEarnings>().ToTable("MemberEarnings");

            // MemberPayment relationships and precision
            modelBuilder.Entity<MemberPayment>()
                .HasOne(mp => mp.Member)
                .WithMany()
                .HasForeignKey(mp => mp.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberPayment>()
                .HasOne(mp => mp.Growfund)
                .WithOne()
                .HasForeignKey<MemberPayment>(mp => mp.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberPayment>().Property(mp => mp.AmountPaid).HasPrecision(18, 2);
            modelBuilder.Entity<MemberPayment>().Property(mp => mp.Cashback).HasPrecision(18, 2);
            modelBuilder.Entity<MemberPayment>().Property(mp => mp.Voucher).HasPrecision(18, 2);
            modelBuilder.Entity<MemberPayment>().Property(mp => mp.Savings).HasPrecision(18, 2);

            // MemberEarnings relationships and precision
            modelBuilder.Entity<MemberEarnings>()
                .HasOne(e => e.Member)
                .WithMany()
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberEarnings>().Property(e => e.Cashback).HasPrecision(18, 2);
            modelBuilder.Entity<MemberEarnings>().Property(e => e.Voucher).HasPrecision(18, 2);
            modelBuilder.Entity<MemberEarnings>().Property(e => e.Savings).HasPrecision(18, 2);

            // Registration role default
            modelBuilder.Entity<Registration>()
                .Property(r => r.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Member");
        }
    }
}
