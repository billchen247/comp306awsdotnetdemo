-- SQL script to create TodoDb and TodoItems table and seed demo data
-- Run this in SSMS or using sqlcmd if you prefer manual provisioning

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TodoDb')
BEGIN
    CREATE DATABASE TodoDb;
END
GO

USE TodoDb;
GO

IF OBJECT_ID('dbo.TodoItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TodoItems
    (
      Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
      Title NVARCHAR(250) NOT NULL,
      IsCompleted BIT NOT NULL DEFAULT 0,
      DueAt DATETIME2 NULL
    );
END
GO

-- Seed rows (only insert if table empty)
IF NOT EXISTS (SELECT 1 FROM dbo.TodoItems)
BEGIN
    INSERT INTO dbo.TodoItems (Title, IsCompleted, DueAt) VALUES
    (N'Buy milk', 0, DATEADD(day, 2, GETDATE())),
    (N'Write blog post', 0, DATEADD(day, 3, GETDATE())),
    (N'Pay bills', 1, NULL);
END
GO

-- Notes:
-- - The script uses IF NOT EXISTS checks so it is safe to re-run without dropping
--   existing data. Review before running in production environments.
-- - Adjust the database name or schema as needed for your environment.
