using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Owls_BE.Models
{
    public partial class Owls_BookContext : DbContext
    {
        public Owls_BookContext()
        {
        }

        public Owls_BookContext(DbContextOptions<Owls_BookContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<BookCrawl> BookCrawls { get; set; } = null!;
        public virtual DbSet<BookImage> BookImages { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Delivery> Deliveries { get; set; } = null!;
        public virtual DbSet<Discount> Discounts { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<UserLogin> UserLogins { get; set; } = null!;
        public virtual DbSet<Voucher> Vouchers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.LoginId)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("login_id");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("phone_number")
                    .IsFixedLength();

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasColumnName("role");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.Login)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.LoginId)
                    .HasConstraintName("FK__Admins__login_id__2EA5EC27");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.Author)
                    .HasMaxLength(500)
                    .HasColumnName("author");

                entity.Property(e => e.BookTitle)
                    .HasMaxLength(500)
                    .HasColumnName("book_title");

                entity.Property(e => e.BookView).HasColumnName("book_view");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Format)
                    .HasMaxLength(50)
                    .HasColumnName("format");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.PageCount).HasColumnName("page_count");

                entity.Property(e => e.PublishedYear).HasColumnName("published_year");

                entity.Property(e => e.Publisher)
                    .HasMaxLength(500)
                    .HasColumnName("publisher");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SalePrice).HasColumnName("sale_price");

                entity.Property(e => e.Summary).HasColumnName("summary");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Books__category___59063A47");
            });

            modelBuilder.Entity<BookCrawl>(entity =>
            {
                entity.ToTable("BookCrawl");

                entity.Property(e => e.BookName).HasMaxLength(500);

                entity.Property(e => e.BookType).HasMaxLength(100);

                entity.Property(e => e.BookUrl).HasMaxLength(1000);

                entity.Property(e => e.DateCrawl).HasColumnType("datetime");

                entity.Property(e => e.Price).HasMaxLength(100);

                entity.Property(e => e.Website).HasMaxLength(100);
            });

            modelBuilder.Entity<BookImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__BookImag__DC9AC955B03267E2");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.ImageName)
                    .HasMaxLength(300)
                    .HasColumnName("image_name");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookImages)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK__BookImage__book___52593CB8");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK__Cart__book_id__6FE99F9F");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Cart__customer_i__70DDC3D8");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(200)
                    .HasColumnName("category_name");

                entity.Property(e => e.CategoryTag)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("category_tag");

                entity.Property(e => e.ParentCategory).HasColumnName("parent_category");

                entity.HasOne(d => d.ParentCategoryNavigation)
                    .WithMany(p => p.InverseParentCategoryNavigation)
                    .HasForeignKey(d => d.ParentCategory)
                    .HasConstraintName("FK__Categorie__paren__5812160E");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Address)
                    .HasMaxLength(1000)
                    .HasColumnName("address");

                entity.Property(e => e.City)
                    .HasMaxLength(1000)
                    .HasColumnName("city");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100)
                    .HasColumnName("customer_name");

                entity.Property(e => e.District)
                    .HasMaxLength(1000)
                    .HasColumnName("district");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.LoginId)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("login_id");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("phone_number")
                    .IsFixedLength();

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.Property(e => e.Ward)
                    .HasMaxLength(1000)
                    .HasColumnName("ward");

                entity.HasOne(d => d.Login)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.LoginId)
                    .HasConstraintName("FK__Customers__login__2DB1C7EE");
            });

            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.ToTable("Delivery");

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.District).HasMaxLength(100);
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.Property(e => e.DiscountId).HasColumnName("discount_id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.DiscountName)
                    .HasMaxLength(100)
                    .HasColumnName("discount_name");

                entity.Property(e => e.DiscountPercent).HasColumnName("discount_percent");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.HasMany(d => d.Books)
                    .WithMany(p => p.Discounts)
                    .UsingEntity<Dictionary<string, object>>(
                        "DiscountsBook",
                        l => l.HasOne<Book>().WithMany().HasForeignKey("BookId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Discounts__book___5165187F"),
                        r => r.HasOne<Discount>().WithMany().HasForeignKey("DiscountId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Discounts__disco__5070F446"),
                        j =>
                        {
                            j.HasKey("DiscountId", "BookId").HasName("PK__Discount__B92E4F57EB4CAE2A");

                            j.ToTable("Discounts_Books");

                            j.IndexerProperty<int>("DiscountId").HasColumnName("discount_id");

                            j.IndexerProperty<int>("BookId").HasColumnName("book_id");
                        });
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("order_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.DeliveryAddress)
                    .HasMaxLength(1000)
                    .HasColumnName("delivery_address");

                entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");

                entity.Property(e => e.IsPaid).HasColumnName("is_paid");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("payment_method");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(10)
                    .HasColumnName("phonenumber")
                    .IsFixedLength();

                entity.Property(e => e.ShippingFee).HasColumnName("shipping_fee");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.Total).HasColumnName("total");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Orders__customer__4E88ABD4");

                entity.HasOne(d => d.Delivery)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DeliveryId)
                    .HasConstraintName("FK__Orders__delivery__2CBDA3B5");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.VoucherId)
                    .HasConstraintName("FK__Orders__voucher___4F7CD00D");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.BookId, e.OrderId })
                    .HasName("PK__OrderDet__CD688CC36FC25822");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("order_id");

                entity.Property(e => e.DiscountPercent).HasColumnName("discount_percent");

                entity.Property(e => e.IsRated).HasColumnName("is_rated");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SalePrice).HasColumnName("sale_price");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__book___5629CD9C");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__order__571DF1D5");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.ReviewId).HasColumnName("review_id");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.Comment)
                    .HasMaxLength(500)
                    .HasColumnName("comment");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("create_at");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.RatingPoint).HasColumnName("rating_point");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK__Reviews__book_id__534D60F1");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Reviews__custome__59FA5E80");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.ToTable("UserLogin");

                entity.Property(e => e.Id)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("id");

                entity.Property(e => e.Refreshtoken)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("refreshtoken");

                entity.Property(e => e.Role)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("role");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Code)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.MinOrderValue).HasColumnName("min_order_value");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
