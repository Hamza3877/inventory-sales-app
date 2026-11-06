# Inventory & Sales Manager – ASP.NET Core MVC, SQL Server, Bootstrap

A web app for a shop or factory to manage **products, stock, customers and sales orders**, and to print invoices.
Built with ASP.NET Core MVC (one single application, a "monolith"), SQL Server and Bootstrap.

A web app for a shop or small factory to manage products, stock, customers and sales orders in one place.

What it does
• Dashboard: today's sales, totals, a 7-day sales chart and a low-stock warning list
• Products, Categories and Customers: add, edit, search, and pages for long lists
• New order: choose a customer and products. The total is calculated and the stock goes down automatically, and the order is refused if there is not enough stock
• Invoice page with Print / Save as PDF, and export of all orders to Excel

Roles
• Admin: full access, including delete
• Staff: can create and edit products, customers and orders, but cannot delete anything
Login uses ASP.NET Core Identity, and the roles are checked on the server.

How it is built
ASP.NET Core MVC (.NET 10), C#, Entity Framework Core, SQL Server, ASP.NET Core Identity, Bootstrap 5, ClosedXML (Excel export), xUnit.
Structure: Controller → Service → Repository → Database. The order rules (check stock, reduce stock, calculate total, save together) live in one service class, so I could unit test them without a database.

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

## Screenshots

![Dashboard](docs/screenshots/1.png)
![Products](docs/screenshots/2.png)


