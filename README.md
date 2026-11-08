# Inventory & Sales Manager – ASP.NET Core MVC, SQL Server, Bootstrap

A web app for a shop or factory to manage **products, stock, customers and sales orders**, and to print invoices.
Built with ASP.NET Core MVC (one single application, a "monolith"), SQL Server and Bootstrap.

A web app for a shop or small factory to manage products, stock, customers and sales orders in one place.

Live Preview;- https://inventory-sales-app.tryasp.net/

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



# Inventory & Sales Manager

A web app for a small shop or factory to keep track of products, stock, customers and sales orders, and to print an invoice for each sale.

I built it to practise how a real business system fits together, from login to invoice, instead of making another list-and-form demo. The part I care about most: when you save an order, the total is calculated and the stock goes down in the same step, and if anything fails nothing is saved.

**Live demo:** [paste link]
(It runs on a free plan, so the first visit can take up to a minute to wake up.)

| Role | Email | Password |
|---|---|---|
| Admin | admin@demo.com | Admin@123 |
| Staff | staff@demo.com | Staff@123 |

## Screenshots

**Dashboard.** Today's sales, totals, the last 7 days as a chart, and a list of products running low.
![Dashboard](docs/screenshots/dashboard.png)

**New order.** Pick a customer and the products. The total and the new stock are worked out when you save.
![New order](docs/screenshots/new-order.png)

**Invoice.** Each order has an invoice page you can print or save as PDF.
![Invoice](docs/screenshots/invoice.png)

**Products.** Search by product or category, with pages for long lists.
![Products](docs/screenshots/products.png)

## Who can do what

| | Admin | Staff |
|---|---|---|
| View dashboard, products, categories, customers, orders | Yes | Yes |
| Add and edit products, categories, customers | Yes | Yes |
| Create orders, print invoices, export to Excel | Yes | Yes |
| Delete products, categories, customers | Yes | No |

Orders cannot be deleted by anyone. A sale is a record, so I did not want it to disappear. The same idea protects data: a category that still has products, a customer who has orders, or a product that was sold cannot be deleted.

The roles are checked on the server, not only hidden in the menu, so Staff cannot delete by typing the address.

## Screens

- **Login:** email and password, using ASP.NET Core Identity.
- **Dashboard:** sales today, totals, a 7-day sales chart, and products with fewer than 10 in stock.
- **Products:** list, search, add, edit, delete (Admin).
- **Categories:** add, edit, delete (Admin).
- **Customers:** search, add, edit, delete (Admin).
- **New order:** choose a customer, fill in the product rows you need (empty rows are ignored).
- **Orders:** all orders, newest first, with an invoice page and an Excel export.

## How an order is saved

All the rules are in one class, `Services/OrderService.cs`, not in the controller:

1. Keep only the filled rows, and merge the same product if it appears twice.
2. Check that the customer exists.
3. Check that there is enough stock for every row. If not, show a clear message and save nothing.
4. Build the order, using the product price at that moment, and reduce the stock.
5. Save the order and the new stock together in one go.

The price is copied into the order, so old invoices don't change when a product price changes later.

## Tech stack

- ASP.NET Core MVC on .NET 10, C#
- Entity Framework Core with SQL Server (Code First, Fluent API)
- ASP.NET Core Identity for login and roles
- Bootstrap 5 for the layout, Chart.js for the sales chart
- ClosedXML for the Excel export
- xUnit tests for the order rules

