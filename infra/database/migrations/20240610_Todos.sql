BEGIN TRANSACTION;
GO

IF SCHEMA_ID(N'Todos') IS NULL EXEC(N'CREATE SCHEMA [Todos];');
GO

CREATE TABLE [Todos].[todos] (
    [Id] nvarchar(450) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Completed] BIT NOT NULL,
    [WorkerId] nvarchar(100) NULL,
    CONSTRAINT [PK_todos] PRIMARY KEY ([Id])
);
GO

COMMIT;
GO

