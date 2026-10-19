# Inventory & Sales Manager

A web app for a shop or factory to manage **products, stock, customers and sales orders**, and to print invoices.
Built with ASP.NET Core MVC (one single application, a "monolith"), SQL Server and Bootstrap.

## What it does

| Screen | What you can do |
|---|---|
| Login | Admin and Staff users. Staff cannot delete anything. |
| Dashboard | Today's sales, totals, a 7-day sales chart, low-stock warning list |
| Products | List with search and pages, add, edit, delete (Admin) |
| Categories | Add, edit, delete (Admin) |
| Customers | Search, add, edit, delete (Admin) |
| New Order | Pick a customer and products. Total is calculated and stock goes down automatically. |
| Orders | List of all orders, invoice page (Print / Save as PDF), export to Excel |

## Demo logins

- Admin: `admin@demo.com` / `Admin@123`
- Staff: `staff@demo.com` / `Staff@123`

## How to run (Visual Studio)

1. Install **Visual Studio 2022 or newer** with the **ASP.NET and web development** workload, and the **.NET 10 SDK**.
   (SQL Server LocalDB is installed with that workload.)
2. Open `InventorySales.sln`.
3. Press **F5** (or the green "https" button).
4. The first start creates the database `InventorySalesDb` and the demo data by itself. Then log in.

### If your SQL Server is not LocalDB
Open `InventorySales/appsettings.json` and change the connection string, for example for SQL Server Express:

`Server=.\\SQLEXPRESS;Database=InventorySalesDb;Trusted_Connection=True;TrustServerCertificate=True`

### If you only have .NET 8
In both `.csproj` files change `net10.0` to `net8.0`, and change `10.0.*` to `8.0.*` in the package lines. Nothing else changes.

### Run the tests
Visual Studio menu: **Test > Run All Tests** (4 tests for the order rules). Or in a terminal: `dotnet test`.

## Folder map (easy to explain)

```
Models/         The tables: Category, Product, Customer, Order, OrderItem
Data/           AppDbContext (database setup) and DbSeeder (demo data)
Repositories/   The only place that talks to the database tables
Services/       Business rules: OrderService (create order, check stock), DashboardService
Controllers/    Receive the browser request, call a repository or service, return a View
Views/          The HTML pages (Razor)
ViewModels/     Small classes that carry data to a screen
```

Request flow: **Browser -> Controller -> Service / Repository -> Database**, then the Controller returns a **View**.

## Key points to explain to HR / interviewers

- **MVC:** Model = data, View = page, Controller = the traffic manager between them.
- **Repository pattern:** controllers never write database queries. They ask a repository. One generic `Repository<T>` handles the simple tables; `ProductRepository` and `OrderRepository` add extra queries.
- **Service layer:** the rule "check stock, reduce stock, calculate total, save everything together" lives in `OrderService`, so it is easy to test. That is why the unit tests exist.
- **Entity Framework Core (Code First) + Fluent API:** tables are created from the C# classes; `AppDbContext` sets rules like decimal precision and "a product that was sold cannot be deleted".
- **Security:** ASP.NET Identity login, roles (Admin/Staff), anti-forgery tokens on forms, `[Authorize]` on every screen.
- **Monolith:** one application, one database, one deployment. Simple and right for a small business app.

## Using real EF Core Migrations (instead of EnsureCreated)

The app uses `EnsureCreatedAsync()` in `Data/DbSeeder.cs` so it runs with zero setup. To use migrations (what many companies do):

1. Delete the database `InventorySalesDb` (SQL Server Object Explorer, right-click, Delete).
2. In **Package Manager Console** (set default project to `InventorySales`): `Add-Migration InitialCreate`
3. In `Data/DbSeeder.cs` replace `EnsureCreatedAsync()` with `MigrateAsync()`.
4. Run the app. The migration creates the database.

## Free hosting ideas

- Push the code to GitHub.
- Free .NET + SQL Server hosts change their plans often (for example MonsterASP.NET or Somee). Check the current free plan, then use that host's "publish" instructions.
- Also record a 2-minute screen video of the app as a backup for recruiters.
