# Provisioning TodoDb on AWS RDS

This project includes `sql/create_tododb.sql` which creates the `TodoDb` database, `dbo.TodoItems` table and inserts seed data. For AWS RDS (SQL Server) you typically provision the database instance through AWS, then run this script against the RDS endpoint.

Two common ways to run the SQL script:

1) Use sqlcmd (recommended for automation)

- Install sqlcmd (part of SQL Server command-line tools) or use the tools available on your CI runner.
- Use the provided PowerShell helper script `scripts/run-sqlcmd-rds.ps1`:

  PowerShell example (interactive):
  ```powershell
  cd <repo-root>
  ./scripts/run-sqlcmd-rds.ps1 -Server "your-rds-endpoint" -User "dbadmin" -File "sql/create_tododb.sql"
  ```

  The script will prompt for the password. It uses `-N -C` flags to enable encryption and trust server certificate which are commonly needed for RDS endpoints.

- Or call sqlcmd directly:
  ```powershell
  sqlcmd -S "your-rds-endpoint,1433" -U dbadmin -P "YourPassword" -i sql/create_tododb.sql -N -C
  ```

2) Use SSMS / Azure Data Studio

- Connect to your RDS endpoint in SSMS/Azure Data Studio using the server address and credentials.
- Open `sql/create_tododb.sql` and execute the script.

Notes and Best Practices
- The script checks for existence and will not overwrite existing data. It's safe to re-run but you should review in production.
- For production deployments, consider using infrastructure-as-code (CloudFormation / Terraform) to create the RDS instance and CI/CD to apply schema migrations (EF Migrations or SQL migration tooling).
- If you prefer EF Migrations, create a migration locally and run `dotnet ef database update` from a trusted deployment environment.

Security
- Never store plaintext DB passwords in source code or commit them to the repository.
- Use Secrets Manager (AWS) or environment variables in your CI/CD pipeline to provide secure credentials.

If you want, I can:
- Add an example GitHub Actions workflow that runs the SQL script against RDS (using secrets), or
- Replace `EnsureCreated()` with `db.Database.Migrate()` and add instructions to create/apply migrations.
