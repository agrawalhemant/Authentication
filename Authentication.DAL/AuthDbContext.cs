using System;
using System.Collections.Generic;
using Authentication.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.DAL;

public partial class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> addresses { get; set; }

    public virtual DbSet<EmailVerification> emailverifications { get; set; }

    public virtual DbSet<PhoneVerification> phoneverifications { get; set; }

    public virtual DbSet<User> users { get; set; }

    public virtual DbSet<UserKyc> userkycs { get; set; }

    public virtual DbSet<UserNotificationSetting> usernotificationsettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.id).HasName("addresses_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.addresstype)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Home'::character varying");
            entity.Property(e => e.city).HasMaxLength(100);
            entity.Property(e => e.createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.district).HasMaxLength(100);
            entity.Property(e => e.latitude).HasPrecision(9, 6);
            entity.Property(e => e.locality).HasMaxLength(100);
            entity.Property(e => e.longitude).HasPrecision(9, 6);
            entity.Property(e => e.pincode).HasMaxLength(10);
            entity.Property(e => e.state).HasMaxLength(100);
            entity.Property(e => e.updatedat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.user).WithMany(p => p.addresses)
                .HasForeignKey(d => d.userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("addresses_userid_fkey");
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.id).HasName("emailverifications_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.isused).HasDefaultValue(false);

            entity.HasOne(d => d.user).WithMany(p => p.emailverifications)
                .HasForeignKey(d => d.userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("emailverifications_userid_fkey");
        });

        modelBuilder.Entity<PhoneVerification>(entity =>
        {
            entity.HasKey(e => e.id).HasName("phoneverifications_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.isused).HasDefaultValue(false);
            entity.Property(e => e.otp).HasMaxLength(10);

            entity.HasOne(d => d.user).WithMany(p => p.phoneverifications)
                .HasForeignKey(d => d.userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("phoneverifications_userid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.phonenumber, "users_phonenumber_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.firstname).HasMaxLength(100);
            entity.Property(e => e.isactive).HasDefaultValue(true);
            entity.Property(e => e.isemailverified).HasDefaultValue(false);
            entity.Property(e => e.isphoneverified).HasDefaultValue(false);
            entity.Property(e => e.lastname).HasMaxLength(100);
            entity.Property(e => e.phonenumber).HasMaxLength(15);
            entity.Property(e => e.preferredlanguage)
                .HasMaxLength(10)
                .HasDefaultValueSql("'en'::character varying");
            entity.Property(e => e.role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'User'::character varying");
            entity.Property(e => e.updatedat).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<UserKyc>(entity =>
        {
            entity.HasKey(e => e.id).HasName("userkyc_pkey");

            entity.ToTable("UserKyc");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.aadharnumber).HasMaxLength(20);
            entity.Property(e => e.createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.gstin).HasMaxLength(20);
            entity.Property(e => e.kycstatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying");
            entity.Property(e => e.pannumber).HasMaxLength(10);
            entity.Property(e => e.updatedat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.user).WithMany(p => p.userkycs)
                .HasForeignKey(d => d.userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("userkyc_userid_fkey");
        });

        modelBuilder.Entity<UserNotificationSetting>(entity =>
        {
            entity.HasKey(e => e.id).HasName("usernotificationsettings_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.createdat).HasDefaultValueSql("now()");
            entity.Property(e => e.marketingemailsenabled).HasDefaultValue(true);
            entity.Property(e => e.productupdatesenabled).HasDefaultValue(true);
            entity.Property(e => e.smsalertsenabled).HasDefaultValue(true);
            entity.Property(e => e.updatedat).HasDefaultValueSql("now()");

            entity.HasOne(d => d.user).WithMany(p => p.usernotificationsettings)
                .HasForeignKey(d => d.userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("usernotificationsettings_userid_fkey");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
