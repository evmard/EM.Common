using Crm.DbUpdater.Core.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System;

namespace Crm.DbUpdater.Core
{
    public class PatchDbContext : DbContext
    {
        internal PatchDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public DbSet<MigrationInfo> Migrations { get; set; }
        public DbSet<AppliedPatch> Patches { get; set; }
    }

    public class PatchDbFactory : IDbContextFactory<PatchDbContext>
    {
        private readonly string _connectionName = Utils.ConfigUtils.GetString("ConnectionName", "PatchDb");

        public PatchDbContext Create()
        {
            return new PatchDbContext(_connectionName);
        }

        public PatchDbContext Create(string nameOrConnectionString)
        {
            return new PatchDbContext(nameOrConnectionString);
        }
    }
}
