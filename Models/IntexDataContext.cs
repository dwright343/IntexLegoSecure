//using System;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;

//namespace IntexLegoSecure.Models;

//public partial class IntexDataContext : DbContext
//{
//    public IntexDataContext()
//    {
//    }

//    public IntexDataContext(DbContextOptions<IntexDataContext> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

//    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

//    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

//    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

//    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

//    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

//    public virtual DbSet<Customer> Customers { get; set; }

//    public virtual DbSet<LineItem> LineItems { get; set; }

//    public virtual DbSet<Order> Orders { get; set; }

//    public virtual DbSet<Product> Products { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<AspNetRole>(entity =>
//        {
//            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
//                .IsUnique()
//                .HasFilter("([NormalizedName] IS NOT NULL)");

//            entity.Property(e => e.Name).HasMaxLength(256);
//            entity.Property(e => e.NormalizedName).HasMaxLength(256);
//        });

//        modelBuilder.Entity<AspNetRoleClaim>(entity =>
//        {
//            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

//            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
//        });

//        modelBuilder.Entity<AspNetUser>(entity =>
//        {
//            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

//            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
//                .IsUnique()
//                .HasFilter("([NormalizedUserName] IS NOT NULL)");

//            entity.Property(e => e.Email).HasMaxLength(256);
//            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
//            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
//            entity.Property(e => e.UserName).HasMaxLength(256);

//            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
//                .UsingEntity<Dictionary<string, object>>(
//                    "AspNetUserRole",
//                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
//                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
//                    j =>
//                    {
//                        j.HasKey("UserId", "RoleId");
//                        j.ToTable("AspNetUserRoles");
//                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
//                    });
//        });

//        modelBuilder.Entity<AspNetUserClaim>(entity =>
//        {
//            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

//            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
//        });

//        modelBuilder.Entity<AspNetUserLogin>(entity =>
//        {
//            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

//            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

//            entity.Property(e => e.LoginProvider).HasMaxLength(128);
//            entity.Property(e => e.ProviderKey).HasMaxLength(128);

//            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
//        });

//        modelBuilder.Entity<AspNetUserToken>(entity =>
//        {
//            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

//            entity.Property(e => e.LoginProvider).HasMaxLength(128);
//            entity.Property(e => e.Name).HasMaxLength(128);

//            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
//        });

//        modelBuilder.Entity<Customer>(entity =>
//        {
//            entity.Property(e => e.CustomerId)
//                .ValueGeneratedNever()
//                .HasColumnName("customerId");
//            entity.Property(e => e.Age).HasColumnName("age");
//            entity.Property(e => e.BirthDate).HasColumnName("birthDate");
//            entity.Property(e => e.CountryOfResidence)
//                .HasMaxLength(50)
//                .HasColumnName("countryOfResidence");
//            entity.Property(e => e.FirstName)
//                .HasMaxLength(50)
//                .HasColumnName("firstName");
//            entity.Property(e => e.Gender)
//                .HasMaxLength(50)
//                .HasColumnName("gender");
//            entity.Property(e => e.LastName)
//                .HasMaxLength(50)
//                .HasColumnName("lastName");
//        });

//        modelBuilder.Entity<LineItem>(entity =>
//        {
//            entity.HasNoKey();
//        });

//        modelBuilder.Entity<Order>(entity =>
//        {
//            entity.HasKey(e => e.TransactionId);

//            entity.Property(e => e.TransactionId).ValueGeneratedNever();
//            entity.Property(e => e.Bank).HasMaxLength(50);
//            entity.Property(e => e.CardType).HasMaxLength(50);
//            entity.Property(e => e.DayOfWeek).HasMaxLength(50);
//            entity.Property(e => e.EntryMode).HasMaxLength(50);
//            entity.Property(e => e.ShippingAddress).HasMaxLength(50);
//            entity.Property(e => e.ShippingCity).HasMaxLength(50);
//            entity.Property(e => e.ShippingCountry).HasMaxLength(50);
//            entity.Property(e => e.ShippingState).HasMaxLength(50);
//            entity.Property(e => e.TransactionCountry).HasMaxLength(50);
//            entity.Property(e => e.TransactionType).HasMaxLength(50);
//        });

//        modelBuilder.Entity<Product>(entity =>
//        {
//            entity.Property(e => e.ProductId)
//                .ValueGeneratedNever()
//                .HasColumnName("productId");
//            entity.Property(e => e.Category)
//                .HasMaxLength(50)
//                .HasColumnName("category");
//            entity.Property(e => e.Description)
//                .HasMaxLength(2800)
//                .HasColumnName("description");
//            entity.Property(e => e.ImgLink)
//                .HasMaxLength(150)
//                .HasColumnName("imgLink");
//            entity.Property(e => e.Name)
//                .HasMaxLength(100)
//                .HasColumnName("name");
//            entity.Property(e => e.NumParts).HasColumnName("numParts");
//            entity.Property(e => e.Price).HasColumnName("price");
//            entity.Property(e => e.PrimaryColor)
//                .HasMaxLength(50)
//                .HasColumnName("primaryColor");
//            entity.Property(e => e.SecondaryColor)
//                .HasMaxLength(50)
//                .HasColumnName("secondaryColor");
//            entity.Property(e => e.Year).HasColumnName("year");
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}
