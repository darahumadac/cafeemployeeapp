IF DB_ID('cafeemployeeapp_db') IS NULL
BEGIN
    CREATE DATABASE cafeemployeeapp_db;
END;
GO

USE cafeemployeeapp_db;
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409122710_InitDb'
)
BEGIN
    CREATE TABLE [Cafes] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(10) NOT NULL,
        [Description] nvarchar(256) NOT NULL,
        [Logo] varbinary(max) NULL,
        [Location] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Cafes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409122710_InitDb'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(8) NOT NULL,
        [Gender] bit NOT NULL,
        [CafeId] uniqueidentifier NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Cafes_CafeId] FOREIGN KEY ([CafeId]) REFERENCES [Cafes] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409122710_InitDb'
)
BEGIN
    CREATE TABLE [EmploymentHistory] (
        [EmployeeId] nvarchar(450) NOT NULL,
        [CafeId] uniqueidentifier NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NULL,
        CONSTRAINT [PK_EmploymentHistory] PRIMARY KEY ([EmployeeId], [CafeId]),
        CONSTRAINT [FK_EmploymentHistory_Cafes_CafeId] FOREIGN KEY ([CafeId]) REFERENCES [Cafes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EmploymentHistory_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409122710_InitDb'
)
BEGIN
    CREATE INDEX [IX_Employees_CafeId] ON [Employees] ([CafeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409122710_InitDb'
)
BEGIN
    CREATE INDEX [IX_EmploymentHistory_CafeId] ON [EmploymentHistory] ([CafeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409122710_InitDb'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250409122710_InitDb', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409123555_Update'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cafes]') AND [c].[name] = N'Id');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Cafes] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [Cafes] ADD DEFAULT (NEWID()) FOR [Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409123555_Update'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250409123555_Update', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409155936_RemoveEmploymentHistory'
)
BEGIN
    DROP TABLE [EmploymentHistory];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409155936_RemoveEmploymentHistory'
)
BEGIN
    ALTER TABLE [Employees] ADD [StartDate] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250409155936_RemoveEmploymentHistory'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250409155936_RemoveEmploymentHistory', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410104007_AlterEmployeeCafeIdFK'
)
BEGIN
    ALTER TABLE [Employees] DROP CONSTRAINT [FK_Employees_Cafes_CafeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410104007_AlterEmployeeCafeIdFK'
)
BEGIN
    ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Cafes_CafeId] FOREIGN KEY ([CafeId]) REFERENCES [Cafes] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410104007_AlterEmployeeCafeIdFK'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250410104007_AlterEmployeeCafeIdFK', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410144504_AddUniqueConstraints'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'Email');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Employees] ALTER COLUMN [Email] nvarchar(450) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410144504_AddUniqueConstraints'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cafes]') AND [c].[name] = N'Location');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Cafes] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Cafes] ALTER COLUMN [Location] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410144504_AddUniqueConstraints'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_Name_Email_PhoneNumber_Gender] ON [Employees] ([Name], [Email], [PhoneNumber], [Gender]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410144504_AddUniqueConstraints'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cafes_Name_Location] ON [Cafes] ([Name], [Location]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410144504_AddUniqueConstraints'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250410144504_AddUniqueConstraints', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410150044_UpdateUniqueConstraint'
)
BEGIN
    DROP INDEX [IX_Employees_Name_Email_PhoneNumber_Gender] ON [Employees];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410150044_UpdateUniqueConstraint'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_Name_Email_PhoneNumber] ON [Employees] ([Name], [Email], [PhoneNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410150044_UpdateUniqueConstraint'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250410150044_UpdateUniqueConstraint', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410152812_AddCreatedAndUpdateDates'
)
BEGIN
    ALTER TABLE [Employees] ADD [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410152812_AddCreatedAndUpdateDates'
)
BEGIN
    ALTER TABLE [Employees] ADD [LastModified] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410152812_AddCreatedAndUpdateDates'
)
BEGIN
    ALTER TABLE [Cafes] ADD [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410152812_AddCreatedAndUpdateDates'
)
BEGIN
    ALTER TABLE [Cafes] ADD [LastModified] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410152812_AddCreatedAndUpdateDates'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250410152812_AddCreatedAndUpdateDates', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410153033_DropLastModifiedDate'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'LastModified');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Employees] DROP COLUMN [LastModified];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410153033_DropLastModifiedDate'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cafes]') AND [c].[name] = N'LastModified');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Cafes] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Cafes] DROP COLUMN [LastModified];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410153033_DropLastModifiedDate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250410153033_DropLastModifiedDate', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410154759_AddUpdateDate'
)
BEGIN
    ALTER TABLE [Employees] ADD [UpdatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410154759_AddUpdateDate'
)
BEGIN
    ALTER TABLE [Cafes] ADD [UpdatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250410154759_AddUpdateDate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250410154759_AddUpdateDate', N'9.0.4');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250411080942_AddEtagRowVersion'
)
BEGIN
    ALTER TABLE [Employees] ADD [ETag] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250411080942_AddEtagRowVersion'
)
BEGIN
    ALTER TABLE [Cafes] ADD [ETag] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250411080942_AddEtagRowVersion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250411080942_AddEtagRowVersion', N'9.0.4');
END;

COMMIT;
GO


