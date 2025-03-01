using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace proforma.Models
{
    public partial class ProformaContext : DbContext
    {
        private readonly IConfiguration? _configuration;

        public ProformaContext(DbContextOptions<ProformaContext> options, IConfiguration? configuration = null)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleBarcode> ArticleBarcodes { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<SystemConfiguration> Configurations { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryDetail> InventoryDetail { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationClient> OrganizationClients { get; set; }
        public virtual DbSet<PasswordReset> PasswordResets { get; set; }
        public virtual DbSet<PersonalAccessToken> PersonalAccessTokens { get; set; }
        public virtual DbSet<PrintingTemplate> PrintingTemplates { get; set; }
        public virtual DbSet<Proforma> Proformas { get; set; }
        public virtual DbSet<ProformaDetail> ProformaDetails { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Usersession> Usersessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration?.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
                }
                optionsBuilder.UseMySQL(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Article ---
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("articles");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Active)
                      .IsRequired()
                      .HasDefaultValueSql("'1'")
                      .HasColumnName("active");
                entity.Property(e => e.Cost)
                      .HasPrecision(10)
                      .HasColumnName("cost");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.Property(e => e.Description)
                      .HasMaxLength(255)
                      .HasColumnName("description");
                entity.Property(e => e.NetProfit)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("net_profit");
                entity.Property(e => e.ProfitPercentage)
                      .HasPrecision(5)
                      .HasColumnName("profit_percentage");
                entity.Property(e => e.SourceInfo)
                      .HasMaxLength(100)
                      .HasColumnName("source_info");
                entity.Property(e => e.Subtotal)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("subtotal");
                entity.Property(e => e.TaxNet)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("tax_net");
                entity.Property(e => e.TaxPercentage)
                      .HasPrecision(5)
                      .HasColumnName("tax_percentage");
                entity.Property(e => e.TaxPercentual)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("tax_percentual");
                entity.Property(e => e.Total)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("total");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
            });

            // --- ArticleBarcode ---
            modelBuilder.Entity<ArticleBarcode>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("article_barcodes");
                entity.HasIndex(e => e.ArticleId, "article_id");
                entity.HasIndex(e => e.Barcode, "barcode").IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ArticleId).HasColumnName("article_id");
                entity.Property(e => e.Barcode)
                      .HasMaxLength(100)
                      .HasColumnName("barcode");
                entity.HasOne(d => d.Article)
                      .WithMany(p => p.ArticleBarcodes)
                      .HasForeignKey(d => d.ArticleId)
                      .HasConstraintName("article_barcodes_ibfk_1");
            });

            // --- Client ---
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("clients");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Active)
                      .IsRequired()
                      .HasDefaultValueSql("'1'")
                      .HasColumnName("active");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.Property(e => e.Email)
                      .HasMaxLength(100)
                      .HasColumnName("email");
                entity.Property(e => e.Identification)
                      .HasMaxLength(50)
                      .HasColumnName("identification");
                entity.Property(e => e.Lastname)
                      .HasMaxLength(100)
                      .HasColumnName("lastname");
                entity.Property(e => e.Name)
                      .HasMaxLength(100)
                      .HasColumnName("name");
                entity.Property(e => e.Phone)
                      .HasMaxLength(50)
                      .HasColumnName("phone");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
            });

            // --- SystemConfiguration ---
            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("configurations");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Key)
                      .HasMaxLength(100)
                      .HasColumnName("key");
                entity.Property(e => e.Value)
                      .HasMaxLength(255)
                      .HasColumnName("value");
                entity.Property(e => e.Description)
                      .HasColumnType("text")
                      .HasColumnName("description");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.HasOne(d => d.User)
                      .WithMany(p => p.SystemConfigurations)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Inventory ---
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("inventories");
                entity.HasIndex(e => e.OrganizationId, "inventories_org_id_fk").IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
            });

            // --- InventoryDetail ---
            modelBuilder.Entity<InventoryDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("inventory_details");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
                entity.Property(e => e.ArticleId).HasColumnName("article_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.Notes)
                      .HasColumnType("text")
                      .HasColumnName("notes");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.HasOne(d => d.Inventory)
                      .WithMany(p => p.InventoryDetails)
                      .HasForeignKey(d => d.InventoryId)
                      .HasConstraintName("inventory_details_inventory_id_fk");
                entity.HasOne(d => d.Article)
                      .WithMany(p => p.InventoryDetails)
                      .HasForeignKey(d => d.ArticleId)
                      .HasConstraintName("inventory_details_article_id_fk");
            });

            // --- Organization ---
            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("organizations");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Address)
                      .HasColumnType("text")
                      .HasColumnName("address");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.Property(e => e.Email)
                      .HasMaxLength(100)
                      .HasColumnName("email");
                entity.Property(e => e.ManagerIdentification)
                      .HasMaxLength(50)
                      .HasColumnName("manager_identification");
                entity.Property(e => e.Name)
                      .HasMaxLength(255)
                      .HasColumnName("name");
                entity.Property(e => e.Phone)
                      .HasMaxLength(50)
                      .HasColumnName("phone");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.Property(e => e.UserId)
                      .HasColumnName("user_id")
                      .IsRequired();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Organizations)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // --- OrganizationClient ---
            modelBuilder.Entity<OrganizationClient>(entity =>
            {
                entity.HasKey(e => new { e.ClientId, e.OrganizationId }).HasName("PRIMARY");
                entity.ToTable("organization_client");
                entity.Property(e => e.ClientId).HasColumnName("client_id");
                entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
                entity.HasOne(oc => oc.Client)
                      .WithMany(c => c.OrganizationClients)
                      .HasForeignKey(oc => oc.ClientId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(oc => oc.Organization)
                      .WithMany(o => o.OrganizationClients)
                      .HasForeignKey(oc => oc.OrganizationId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
            });

            // --- PasswordReset ---
            modelBuilder.Entity<PasswordReset>(entity =>
            {
                entity.HasKey(e => new { e.Email, e.Token });
                entity.ToTable("password_resets");
                entity.HasIndex(e => e.Email, "email");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.Email)
                      .HasMaxLength(100)
                      .HasColumnName("email");
                entity.Property(e => e.Token)
                      .HasMaxLength(255)
                      .HasColumnName("token");
            });

            // --- PersonalAccessToken ---
            modelBuilder.Entity<PersonalAccessToken>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("personal_access_tokens");
                entity.HasIndex(e => new { e.TokenableType, e.TokenableId }, "personal_access_tokens_tokenable_index");
                entity.HasIndex(e => e.Token, "token").IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Abilities)
                      .HasColumnType("text")
                      .HasColumnName("abilities");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.LastUsedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("last_used_at");
                entity.Property(e => e.Name)
                      .HasMaxLength(255)
                      .HasColumnName("name");
                entity.Property(e => e.Token)
                      .HasMaxLength(64)
                      .HasColumnName("token");
                entity.Property(e => e.TokenableId).HasColumnName("tokenable_id");
                entity.Property(e => e.TokenableType).HasColumnName("tokenable_type");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
            });

            // --- PrintingTemplate ---
            modelBuilder.Entity<PrintingTemplate>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("printing_templates");
                entity.HasIndex(e => e.UserId, "printing_templates_user_id_fk");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ColorMode)
                      .HasMaxLength(50)
                      .HasColumnName("color_mode");
                entity.Property(e => e.Copies)
                      .HasDefaultValueSql("'1'")
                      .HasColumnName("copies");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.Property(e => e.FooterHtml)
                      .HasColumnType("text")
                      .HasColumnName("footer_html");
                entity.Property(e => e.HeaderHtml)
                      .HasColumnType("text")
                      .HasColumnName("header_html");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.PageOrientation)
                      .HasMaxLength(50)
                      .HasColumnName("page_orientation");
                entity.Property(e => e.PaperSize)
                      .HasMaxLength(50)
                      .HasDefaultValueSql("'A4'")
                      .HasColumnName("paper_size");
                entity.Property(e => e.TemplateName)
                      .HasMaxLength(100)
                      .HasColumnName("template_name");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.HasOne(d => d.User)
                      .WithMany(p => p.PrintingTemplates)
                      .HasForeignKey(d => d.UserId)
                      .HasConstraintName("printing_templates_user_id_fk");
            });

            // --- Proforma ---
            modelBuilder.Entity<Proforma>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("proformas");
                entity.HasIndex(e => e.ClientId, "proformas_client_id_fk");
                entity.HasIndex(e => e.OrganizationId, "proformas_org_id_fk");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ClientId).HasColumnName("client_id");
                entity.Property(e => e.Comment)
                      .HasColumnType("text")
                      .HasColumnName("comment");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.Property(e => e.DiscountTotal)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("discount_total");
                entity.Property(e => e.InvoiceNumber)
                      .HasMaxLength(50)
                      .HasColumnName("invoice_number");
                entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
                entity.Property(e => e.Subtotal)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("subtotal");
                entity.Property(e => e.Taxes)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("taxes");
                entity.Property(e => e.Total)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("total");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.HasOne(d => d.Client)
                      .WithMany(p => p.Proformas)
                      .HasForeignKey(d => d.ClientId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("proformas_client_id_fk");
                entity.HasOne(d => d.Organization)
                      .WithMany(p => p.Proformas)
                      .HasForeignKey(d => d.OrganizationId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("proformas_org_id_fk");
            });

            // --- ProformaDetail ---
            modelBuilder.Entity<ProformaDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("proforma_details");
                entity.HasIndex(e => e.ProformaId, "proforma_details_proforma_id_fk");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ArticleCode)
                      .HasMaxLength(100)
                      .HasColumnName("article_barcode");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.Property(e => e.Discount)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("discount");
                entity.Property(e => e.ItemComment)
                      .HasColumnType("text")
                      .HasColumnName("item_comment");
                entity.Property(e => e.LineTotal)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("line_total");
                entity.Property(e => e.ProformaId).HasColumnName("proforma_id");
                entity.Property(e => e.Quantity)
                      .HasDefaultValueSql("'1'")
                      .HasColumnName("quantity");
                entity.Property(e => e.UnitPrice)
                      .HasPrecision(10)
                      .HasColumnName("unit_price");
                entity.Property(e => e.UnitTax)
                      .HasPrecision(10)
                      .HasDefaultValueSql("'0.00'")
                      .HasColumnName("unit_tax");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.HasOne(d => d.Proforma)
                      .WithMany(p => p.ProformaDetails)
                      .HasForeignKey(d => d.ProformaId)
                      .HasConstraintName("proforma_details_proforma_id_fk");
            });

            // --- User ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("users");
                entity.HasIndex(e => e.Email, "email").IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Active)
                      .IsRequired()
                      .HasDefaultValueSql("'1'")
                      .HasColumnName("active");
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("created_at");
                entity.Property(e => e.DeletedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("deleted_at");
                entity.Property(e => e.Email)
                      .HasMaxLength(100)
                      .HasColumnName("email");
                entity.Property(e => e.Name)
                      .HasMaxLength(100)
                      .HasColumnName("name");
                entity.Property(e => e.Password)
                      .HasMaxLength(255)
                      .HasColumnName("password");
                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp")
                      .HasColumnName("updated_at");
                entity.HasMany(u => u.Organizations)
                      .WithOne(o => o.User)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(u => u.SystemConfigurations)
                      .WithOne(sc => sc.User)
                      .HasForeignKey(sc => sc.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(u => u.PrintingTemplates)
                      .WithOne(pt => pt.User)
                      .HasForeignKey(pt => pt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Usersession ---
            modelBuilder.Entity<Usersession>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("UserSessions"); // Coincide con la tabla en SQL

                entity.HasIndex(e => e.UserId, "UserId");

                entity.Property(e => e.Id)
                      .HasColumnName("Id")
                      .HasColumnType("bigint unsigned")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                      .HasColumnName("UserId")
                      .HasColumnType("bigint unsigned")
                      .IsRequired();

                entity.Property(e => e.RefreshToken)
                      .HasColumnName("RefreshToken")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Fingerprint)
                      .HasColumnName("Fingerprint")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.ExpiryDate)
                      .HasColumnName("ExpiryDate")
                      .HasColumnType("datetime")
                      .IsRequired();

                entity.Property(e => e.RevokedAt)
                      .HasColumnName("RevokedAt")
                      .HasColumnType("datetime")
                      .IsRequired(false);

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .HasColumnType("timestamp")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .IsRequired();

                entity.HasOne(d => d.User)
                      .WithMany(p => p.Usersessions)
                      .HasForeignKey(d => d.UserId)
                      .HasConstraintName("UserSessions_ibfk_1")
                      .OnDelete(DeleteBehavior.Cascade);
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
