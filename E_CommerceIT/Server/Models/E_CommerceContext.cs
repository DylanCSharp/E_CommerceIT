using System;
using E_CommerceIT.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace E_CommerceIT.Server.Models
{
    public partial class E_CommerceContext : DbContext
    {
        public E_CommerceContext()
        {
        }

        public E_CommerceContext(DbContextOptions<E_CommerceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<OrderHistory> OrderHistories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=ecommerceit.database.windows.net;Initial Catalog=E_Commerce;Persist Security Info=True;User ID=adminecommerce;Password=Dylancox123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("CATEGORY", "ECM");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.CategoryIcon)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("categoryIcon");

                entity.Property(e => e.CategoryType)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("categoryType");

                entity.Property(e => e.CategoryUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("categoryUrl");
            });

            modelBuilder.Entity<OrderHistory>(entity =>
            {
                entity.HasKey(e => e.OrderNum)
                    .HasName("PK__ORDER_HI__599964288677E24E");

                entity.ToTable("ORDER_HISTORY", "ECM");

                entity.Property(e => e.OrderNum).HasColumnName("orderNum");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("orderDate");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.OrderHistories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__ORDER_HIS__categ__6E01572D");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderHistories)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ORDER_HIS__produ__6B24EA82");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OrderHistories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ORDER_HIS__userI__6C190EBB");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCTS", "ECM");

                entity.HasIndex(e => e.ProductName, "UQ__PRODUCTS__0A9CBBE059B63E86")
                    .IsUnique();

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.ProductCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("productCreated");

                entity.Property(e => e.ProductDescription)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("productDescription");

                entity.Property(e => e.ProductImage)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("productImage");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("productName");

                entity.Property(e => e.ProductPrice)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("productPrice");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PRODUCTS__catego__68487DD7");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("USERS", "ECM");

                entity.HasIndex(e => e.UserEmail, "UQ__USERS__D54ADF5595CBFEB1")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.IsCustomer).HasColumnName("isCustomer");

                entity.Property(e => e.UserCreatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("userCreatedOn")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("userEmail");

                entity.Property(e => e.UserFirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("userFirstName");

                entity.Property(e => e.UserLastName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("userLastName");

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("userPassword")
                    .IsFixedLength(true);

                entity.Property(e => e.UserSalt).HasColumnName("userSalt");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
