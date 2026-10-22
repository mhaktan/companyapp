using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CompanyApp.EntityFrameworkCore;
using CompanyApp.EntityFrameworkCore.Seed;
using CompanyApp.Entities;

namespace CompanyApp.Web.Host
{
    /// <summary>
    /// Background service that runs migration + seed once at startup
    /// without blocking the HTTP pipeline.
    /// </summary>
    public class MigrationHostedService : IHostedService
    {
        private readonly IConfiguration _config;

        public MigrationHostedService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connStr = _config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrEmpty(connStr)) return;

            // Run in background to avoid blocking host startup (prevents EF tooling timeout)
            _ = Task.Run(async () =>
            {
                // Small delay to ensure host is fully started before DB operations
                await Task.Delay(1000, cancellationToken);
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(connStr);

                    using (var db = new CompanyAppDbContext(optionsBuilder.Options))
                    {
                        // EnsureCreated bos veritabaninda en guncel semayi kurar ve true doner.
                        // Dolu veritabaninda hicbir sey yapmaz — o durumda bekleyen migration'lar
                        // SchemaMigrations.Apply icinde sirayla calistirilir.
                        var freshlyCreated = db.Database.EnsureCreated();
                        Console.WriteLine("[Migration] Migration tanimi yok — sema EnsureCreated ile yonetiliyor.");
                        Console.WriteLine("[Migration] Database is up to date.");

                        // Seed sample data — wrapped in its own try so a failure here doesn't block RBAC seed below.
                        try
                        {
                    if (!db.Companies.Any())
                    {
                        db.Companies.AddRange(
                    new Company { Id = 1, Name = "Firma 1", TaxNumber = "ABC-001", TaxOffice = "Sample Item 1", Address = "123 Main St, New York", Phone = "+1-555-0101", Email = "alice@example.com", ContactPerson = "Sample Item 1" },
                    new Company { Id = 2, Name = "Firma 2", TaxNumber = "XYZ-002", TaxOffice = "Sample Item 2", Address = "456 Oak Ave, London", Phone = "+1-555-0102", Email = "bob@example.com", ContactPerson = "Sample Item 2" }
                        );
                    }
                    if (!db.Products.Any())
                    {
                        db.Products.AddRange(
                    new Product { Id = 3, Name = "Ürün 1", Code = "ABC-001", Unit = "Sample Item 1", UnitPrice = 99.99m },
                    new Product { Id = 4, Name = "Ürün 2", Code = "XYZ-002", Unit = "Sample Item 2", UnitPrice = 149.50m }
                        );
                    }
                    if (!db.Orders.Any())
                    {
                        db.Orders.AddRange(
                    new Order { Id = 5, OrderNo = "ABC-001", OrderDate = new DateTime(2024, 3, 15), DeliveryDate = new DateTime(2024, 3, 15), DiscountRate = 99.99m, VatRate = 99.99m, Subtotal = 99.99m, DiscountAmount = 99.99m, VatAmount = 99.99m, GrandTotal = 99.99m, Notes = "Lorem ipsum dolor sit amet", Status = (OrderStatus)0, CompanyId = 1 },
                    new Order { Id = 6, OrderNo = "XYZ-002", OrderDate = new DateTime(2024, 6, 20), DeliveryDate = new DateTime(2024, 6, 20), DiscountRate = 149.50m, VatRate = 149.50m, Subtotal = 149.50m, DiscountAmount = 149.50m, VatAmount = 149.50m, GrandTotal = 149.50m, Notes = "Consectetur adipiscing elit", Status = (OrderStatus)0, CompanyId = 2 }
                        );
                    }
                    if (!db.OrderLines.Any())
                    {
                        db.OrderLines.AddRange(
                    new OrderLine { Id = 7, Description = "Lorem ipsum dolor sit amet", Quantity = 99.99m, Unit = "Sample Item 1", UnitPrice = 99.99m, DiscountRate = 99.99m, VatRate = 99.99m, TotalPrice = 99.99m, LineOrder = 42, OrderId = 5, ProductId = 3 },
                    new OrderLine { Id = 8, Description = "Consectetur adipiscing elit", Quantity = 149.50m, Unit = "Sample Item 2", UnitPrice = 149.50m, DiscountRate = 149.50m, VatRate = 149.50m, TotalPrice = 149.50m, LineOrder = 17, OrderId = 6, ProductId = 4 }
                        );
                    }
                            db.SaveChanges();
                            Console.WriteLine("[Seed] Sample data created.");
                        }
                        catch (Exception sampleEx)
                        {
                            Console.WriteLine($"[Seed] Sample data skipped: {sampleEx.GetType().Name}: {sampleEx.Message}");
                            // Carry on — RBAC seed must still run so admin/123qwe is usable.
                        }
                        // Sync identity sequences to MAX(Id). Seeded rows carry explicit Ids which do NOT
                        // advance Postgres identity sequences → nextval collides with a seed row and the
                        // first few inserts fail with a duplicate-key 500. Runs every startup; idempotent.
                        try
                        {
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Companies\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Companies\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Products\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Products\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Orders\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Orders\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"OrderLines\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"OrderLines\") + 1, false);");
                            Console.WriteLine("[Seed] Identity sequences synced.");
                        }
                        catch (Exception seqEx)
                        {
                            Console.WriteLine($"[Seed] Sequence sync skipped: {seqEx.GetType().Name}: {seqEx.Message}");
                        }
                    }
                    // RBAC seed (Admin/User roles + permissions + admin user) runs through ABP DI
                    // so PermissionRegistry can be injected. SeedHelper is idempotent.
                    SeedHelper.SeedHostDb(Abp.Dependency.IocManager.Instance);
                    Console.WriteLine("[Seed] RBAC seed complete (Admin role + admin user).");
                }
                catch (Exception ex)
                {
                    // Full diagnostic — surface the real cause so silent seed failures are debuggable.
                    Console.WriteLine($"[Migration] FAILED: {ex.GetType().Name}: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Migration] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Console.WriteLine("[Migration] StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("[Migration] App continues without migration — admin user will not exist.");
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Program
    {
        // Runtime entry: WebHost is required because ABP Startup returns IServiceProvider.
        public static void Main(string[] args)
        {
            // Npgsql 7+ requires UTC DateTimes — enable legacy behavior for ABP compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        // Design-time entry for EF Core tools (dotnet ef migrations).
        // Without this, EF tools wait 5 minutes for IHost build (resolver default timeout)
        // and then SIGTERM any running dotnet process — killing live dev servers.
        // We expose a minimal IHost that EF tools resolve in milliseconds; the actual
        // DbContext is built by IDesignTimeDbContextFactory in the EntityFrameworkCore project.
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
    }
}
