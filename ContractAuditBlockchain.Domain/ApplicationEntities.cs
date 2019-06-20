using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContractAuditBlockchain.Domain
{
    public partial class ApplicationEntities : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, IdentityUserClaim>, IApplicationDbContext
    {
        public ApplicationEntities()
            : base("name=ApplicationEntities")
        {
            Database.SetInitializer(new DomainInitialiser());
        }

        public IDbSet<Admin> Admins { get; set; }
        public IDbSet<Client> Clients { get; set; }
        public IDbSet<Contract> Contracts { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            OnIdentityModelCreating(mb);

            //// User -> Added By
            //mb.Entity<ApplicationUser>()
            //    .HasMany(e => e.UsersAdded)
            //    .WithRequired(e => e.AddedByUser)
            //    .HasForeignKey(e => e.AddedByUserId);

            // Admins are linked 1:1 to an Application User, but
            // not all Application Users are Admins.
            mb.Entity<Admin>()
              .HasRequired(s => s.ApplicationUser)
              .WithOptional(au => au.Admin);

            // Clients are linked 1:1 to an Application User, but
            // not all Application Users are Clients.
            mb.Entity<Client>()
              .HasRequired(s => s.ApplicationUser)
              .WithOptional(au => au.Client);

            #region Contracts

            // Admins
            mb.Entity<Contract>()
              .HasRequired(c => c.Provider)
              .WithMany(p => p.Contracts);

            // Clients
            mb.Entity<Contract>()
              .HasRequired(c => c.Client)
              .WithMany(c => c.Contracts);

            #endregion Contracts

        }

        protected override bool ShouldValidateEntity(DbEntityEntry entityEntry)
        {
            return base.ShouldValidateEntity(entityEntry);
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            return base.ValidateEntity(entityEntry, items);
        }

        private void OnIdentityModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            // Needed to ensure subclasses share the same table
            var user = modelBuilder.Entity<ApplicationUser>()
                .ToTable("AspNetUsers");
            user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId).WillCascadeOnDelete(true);
            user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId).WillCascadeOnDelete(true);
            user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId).WillCascadeOnDelete(true);
            user.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

            // CONSIDER: u.Email is Required if set on options?
            user.Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Entity<ApplicationUserRole>()
                .HasKey(r => new { r.ID })
                .ToTable("AspNetUserRoles");

            modelBuilder.Entity<ApplicationUserLogin>()
                .HasKey(l => new { l.ID })
                .ToTable("AspNetUserLogins");

            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("AspNetUserClaims");

            var role = modelBuilder.Entity<ApplicationRole>()
                .ToTable("AspNetRoles");
            role.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId).WillCascadeOnDelete(true);
        }
    }
}
