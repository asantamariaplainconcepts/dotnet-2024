BEGIN TRANSACTION;
GO

IF SCHEMA_ID(N'Workers') IS NULL EXEC(N'CREATE SCHEMA [Workers];');
GO

CREATE TABLE [Workers].[workers] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_workers] PRIMARY KEY ([Id])
);
GO

COMMIT;
GO

