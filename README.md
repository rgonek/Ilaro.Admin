# Ilaro.Admin

**A code-first admin panel generator for ASP.NET MVC, inspired by [Django's admin site](https://docs.djangoproject.com/en/dev/ref/contrib/admin/).**

Point it at your POCO classes and get a fully functional admin UI — CRUD operations, filtering, search, file uploads, image handling, change tracking, and more — without writing a single view or controller.

> **Note:** This project is no longer actively maintained. It was built in 2014 as a personal/open-source project and is preserved here as a portfolio piece. The architecture and patterns remain a solid reference for convention-over-configuration library design in .NET.

## Key Features

- **Automatic CRUD** — full Create, Read, Update, Delete from POCO classes with zero boilerplate
- **Dual configuration** — attribute-based or fluent API, your choice:
  ```csharp
  // Attributes
  [Verbose(GroupName = "Product")]
  public class Product
  {
      [Required, StringLength(40)]
      public string ProductName { get; set; }

      [File(".jpg", ".png"), ImageSettings(200, 200)]
      public string Photo { get; set; }
  }

  // Fluent
  public class ProductConfig : EntityConfiguration<Product>
  {
      public ProductConfig()
      {
          PropertiesGroup("Main", x => x.ProductName, x => x.UnitPrice);
          Property(x => x.ProductName, p => { p.Required(); p.StringLength(40); });
      }
  }
  ```
- **Advanced filtering & search** — string, numeric, date range, boolean, enum, and foreign entity filters
- **Relationship handling** — one-to-many dropdowns, many-to-many dual listbox editors, automatic FK detection
- **File & image uploads** — type/size validation, automatic thumbnail generation, multiple size variants
- **Change tracking** — audit trail recording user, timestamp, and operation type for every change
- **Soft delete** — mark records as deleted without physical removal, with restore capability
- **Concurrency control** — optimistic locking to prevent lost updates in multi-user scenarios
- **Role-based authorization** — pluggable `AuthorizeAttribute` support
- **Single-DLL deployment** — views pre-compiled via RazorGenerator, static assets embedded in the assembly

## Architecture

```
src/
  Ilaro.Admin.Core     Core logic: entities, services, validation, data access
  Ilaro.Admin          ASP.NET MVC Area with controllers, views, and frontend assets
  Ilaro.Admin.Unity    Unity DI container integration
  Ilaro.Admin.Ninject  Ninject DI container integration
  Ilaro.Admin.Autofac  Autofac DI container integration
samples/
  Ilaro.Admin.Sample   Northwind database demo app
tests/
  Ilaro.Admin.Tests    Unit tests (xUnit + FakeItEasy)
```

**Design decisions worth noting:**

- **Interface-driven service layer** — `IEntityService`, `IRecordsService`, `IValidatingEntities`, `IHandlingFiles`, etc. All business logic is behind interfaces, making the codebase fully testable and DI-friendly.
- **Areas-based isolation** — the admin panel lives in its own ASP.NET MVC Area, preventing any namespace or routing collisions with the host application.
- **Convention over configuration** — automatic primary key detection, foreign key inference, sensible defaults for every property type. You configure only what deviates from convention.
- **Modified Massive ORM** — stripped down to read-only operations for tighter control over SQL generation, with custom dynamic SQL building for filters and sorting.
- **Template system** — pluggable display, editor, and filter templates per data type (DateTime pickers, markdown editors, WYSIWYG, numeric spinners, image previews, etc.).
- **Multi-container DI support** — separate NuGet packages for Unity, Ninject, and Autofac so consumers aren't forced into a specific container.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET Framework 4.5, ASP.NET MVC 4 |
| Data access | Modified Massive (micro-ORM), dynamic SQL generation |
| DI containers | Unity, Ninject, Autofac (separate packages) |
| Frontend | Bootstrap 3, jQuery, Chosen, Summernote, Bootstrap-DateTimePicker, Dual Listbox |
| Images | ImageResizer |
| View engine | RazorGenerator (pre-compiled views) |
| Testing | xUnit, FakeItEasy |
| Build | Cake, AppVeyor CI |
| Package | NuGet |

## Quick Start

Install via NuGet (with your preferred DI container):
```
Install-Package Ilaro.Admin.Unity
Install-Package Ilaro.Admin.Ninject
Install-Package Ilaro.Admin.Autofac
```

Register in `Global.asax`:
```csharp
// 1. Register routes (before default routes)
AdminInitialise.RegisterRoutes(RouteTable.Routes, prefix: "Admin");
AdminInitialise.RegisterResourceRoutes(RouteTable.Routes);

// 2. Register your entities
Entity<Customer>.Add();
Entity<Product>.Add();

// 3. Set up authorization
Admin.Authorize = new AuthorizeAttribute { Roles = "Admin" };

// 4. Initialize (connection string name, optional if you have only one)
Admin.Initialise("NorthwindEntities");
```

Navigate to `~/Admin` and your admin panel is ready.

For detailed configuration options, see the wiki: [Entity configuration](https://github.com/rgonek/Ilaro.Admin/wiki/Entity-configuration) | [Property configuration](https://github.com/rgonek/Ilaro.Admin/wiki/Property-configuration)

## License

[MIT](LICENSE) — Robert Gonek
