using System;
using Microsoft.EntityFrameworkCore;

namespace ProgressiveWebAppsDemo.Web.Data
{
    public class ExpensesContext : DbContext
    {
        public DbSet<Movement> Movements { get; set; }

        public DbSet<Category> Categories { get; set; }

        public ExpensesContext(DbContextOptions<ExpensesContext> options)
            : base(options)
        {
        }

        public void EnsureSeedData()
        {
            this.Database.ExecuteSqlCommand(@"
                SET IDENTITY_INSERT Categories ON;
                MERGE Categories as Target USING (VALUES 
                        (1, 'Default'),
                        (2, 'Food'),
                        (3, 'Entertainment'),
                        (4, 'Health'),
                        (5, 'Transport')) AS Source (Id, Name)
                    ON Target.Id = Source.Id
                    WHEN MATCHED THEN
                        UPDATE SET Name = Source.Name
                    WHEN NOT MATCHED BY Target THEN
                        INSERT (Id, Name) VALUES (Source.Id, Source.Name);
                SET IDENTITY_INSERT Categories OFF
            ");
        }
    }
}
