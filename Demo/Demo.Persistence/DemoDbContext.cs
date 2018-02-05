using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Demo.Domain.Models;
using Demo.Domain.Models.Accounts;
using Demo.Domain.Models.Users;
using IFramework.EntityFramework;

namespace Demo.Persistence
{
    public class DemoDropCreateDatabaseIfModelChanges : DropCreateDatabaseIfModelChanges<DemoDbContext>
    {
        protected override void Seed(DemoDbContext context)
        {
            base.Seed(context);
            // 用于初始化数据库
            //var initSqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/demoinit.sql");
            //if (File.Exists(initSqlFile))
            {
                var sql = @" CREATE SEQUENCE[dbo].[DbSequence]
                AS[bigint]
                START WITH 0
                INCREMENT BY 2
                MINVALUE - 9223372036854775808
                MAXVALUE 9223372036854775807
                CACHE";//File.ReadAllText(initSqlFile);
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    context.Database.ExecuteSqlCommand(sql);
                }
            }
        }
    }

    public class DemoDbContext : MSDbContext, ISetUserable
    {
        static DemoDbContext()
        {
            Database.SetInitializer(new DemoDropCreateDatabaseIfModelChanges());
        }

        public DemoDbContext()
            : base("DemoDb")
        {
            //Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
        }

        public UserInfo CurrentUser { get; protected set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }

        public void SetCurrentUser(UserInfo user)
        {
            CurrentUser = user;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                        .Property(u => u.Name)
                        .HasMaxLength(200);
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Name)
                        .IsUnique();

            modelBuilder.Entity<Account>()
                        .Property(a => a.UserName)
                        .HasMaxLength(200);

            modelBuilder.Entity<Account>()
                        .Property(a => a.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Account>()
                        .HasIndex(a => new {a.AccountType, a.UserName})
                        .IsUnique();
           

            modelBuilder.ComplexType<UserInfo>();
        }

        public override int SaveChanges()
        {
            UpdateEntitiesModification();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            UpdateEntitiesModification();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected void UpdateEntitiesModification()
        {
            var user = CurrentUser ?? UserInfo.Null;
            foreach (var e in ChangeTracker.Entries())
            {
                if (e.Entity is IUpdatable updatable)
                {
                    if (e.State == EntityState.Modified)
                    {
                        updatable.UpdateModification(user);
                    }
                    else if (e.State == EntityState.Added)
                    {
                        updatable.UpdateCreation(user);
                    }
                }
            }
        }
    }
}