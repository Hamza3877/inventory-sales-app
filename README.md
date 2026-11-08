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

A web app for a small shop or factory to keep track of what it sells: products and stock, customers and sales orders. When an order is saved the stock goes down by itself, and the order can be printed as an invoice.

I built it as one ASP.NET Core MVC application on SQL Server. I wanted to show how a normal business app fits together (login, roles, business rules, a database that protects its own data), not only add, edit and delete pages.

**Live demo:** [paste link]
It runs on a free plan, so the first visit can take up to a minute to wake up.

| Role | Email | Password |
|---|---|---|
| Admin | admin@demo.com | Admin@123 |
| Staff | staff@demo.com | Staff@123 |

## Who can do what

| | Admin | Staff |
|---|---|---|
| See the dashboard, products, customers, orders | Yes | Yes |
| Add and edit products, categories, customers | Yes | Yes |
| Create orders and print invoices | Yes | Yes |
| Delete products, categories, customers | Yes | No |

The roles are checked on the server, so a Staff user cannot delete something by typing the address in the browser.

## Screens

**Dashboard.** The first page after login. It shows today's sales, totals, a chart of the last 7 days and a list of products that are running low.
![Dashboard](docs/screenshots/dashboard.png)

**Products.** Search by product or category, page through the list, add and edit. Only Admin sees the Delete button.
![Products](docs/screenshots/products.png)

**New order.** Choose a customer and fill in the product rows. The total is worked out automatically and the stock goes down when the order is saved.
![New order](docs/screenshots/new-order.png)

**Invoice.** Every order has an invoice page that can be printed or saved as PDF. All orders can also be exported to Excel.
![Invoice](docs/screenshots/invoice.png)

## How an order is saved

This is the part I spent the most time on, because a wrong stock number is a real business problem.

- Empty rows are ignored, and the same product on two rows is added together.
- If any product does not have enough stock, the whole order is refused with a clear message.
- The price is copied into the order, so old invoices don't change when a product price changes later.
- The order and the new stock are saved together. If something fails, nothing is saved.
- The database also blocks deleting a category that has products, a customer that has orders, or a product that was already sold.

Four unit tests cover these rules.

## Tech stack

ASP.NET Core MVC (.NET 10), C#, Entity Framework Core, SQL Server, ASP.NET Core Identity (login and roles), Bootstrap 5, ClosedXML (Excel export), xUnit.

## How the code is organised

Controller, then Service or Repository, then Database. Controllers only receive the request and return a page. The order rules live in `OrderService`. Database queries live in the repositories.

## What I would add next

Paging on the orders list, order cancellation with stock restore, and a REST API for a mobile app.
