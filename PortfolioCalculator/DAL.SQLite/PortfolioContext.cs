using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Contracts;

using SQLite.CodeFirst;

namespace DAL.SQLite
{
	public class PortfolioContext : DbContext
	{
		public DbSet<Portfolio> Portfolios { get; set; }
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Position> Positions { get; set; }
		public DbSet<Security> Securities { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<CategoryValue> CategoryValues { get; set; }
		public DbSet<CategoryWeight> CategoryWeights { get; set; }

		public PortfolioContext()
			: base("PortfolioContext")
		{
			
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			//modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			var sqliteInitializer = new SqliteCreateDatabaseIfNotExists<PortfolioContext>(modelBuilder);
			Database.SetInitializer(sqliteInitializer);
		}
	}
}
