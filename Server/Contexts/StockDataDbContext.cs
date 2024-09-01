using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Server.Contexts;

public partial class StockDataDbContext : DbContext
{
    public StockDataDbContext()
    {
    }

    public StockDataDbContext(DbContextOptions<StockDataDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Stockdatum> Stockdata { get; set; }

    public virtual DbSet<Stockuser> Stockusers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stockdatum>(entity =>
        {
            entity.HasKey(e => e.Stockdataid).HasName("stockdata_pkey");

            entity.ToTable("stockdata");

            entity.Property(e => e.Stockdataid).HasColumnName("stockdataid");
            entity.Property(e => e.ClosePrice)
                .HasPrecision(10, 2)
                .HasColumnName("close_price");
            entity.Property(e => e.HighPrice)
                .HasPrecision(10, 2)
                .HasColumnName("high_price");
            entity.Property(e => e.LowPrice)
                .HasPrecision(10, 2)
                .HasColumnName("low_price");
            entity.Property(e => e.OpenPrice)
                .HasPrecision(10, 2)
                .HasColumnName("open_price");
            entity.Property(e => e.TickerSymbol)
                .HasMaxLength(10)
                .HasColumnName("ticker_symbol");
            entity.Property(e => e.TradeDate).HasColumnName("trade_date");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Stockdata)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("stockdata_userid_fkey");
        });

        modelBuilder.Entity<Stockuser>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("stockuser_pkey");

            entity.ToTable("stockuser");

            entity.HasIndex(e => e.Email, "stockuser_email_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_created");
            entity.Property(e => e.DateLastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_last_login");
            entity.Property(e => e.DateRetired)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_retired");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.UmsUserid).HasColumnName("ums_userid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
