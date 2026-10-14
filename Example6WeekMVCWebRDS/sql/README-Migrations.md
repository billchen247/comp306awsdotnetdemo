This project targets a pre-provisioned SQL Server database with the `TodoItems` table.

If you prefer using EF Core migrations instead of running the provided SQL script `create_tododb.sql`, run the following locally:

1. Install the EF tools if needed: `dotnet tool install --global dotnet-ef`
2. From the project directory run:
   - `dotnet ef migrations add InitialCreate`
   - `dotnet ef database update`

Alternatively you can run `sql/create_tododb.sql` against your SQL Server / RDS instance to create the `TodoItems` table.

Note: Applying migrations at app startup is enabled by default in `Data/DbInitializer.cs` which calls `context.Database.Migrate()`.
