BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Dob');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Dob] datetime2 NULL;
GO

UPDATE [App_Sequence] SET [CreatedDate] = '2023-06-26T09:36:28.8001939+07:00'
WHERE [Id] = 'efb6f443-337c-4f7e-afa9-328bec063f21';
SELECT @@ROWCOUNT;

GO

UPDATE [App_SequenceLine] SET [CreatedDate] = '2023-06-26T09:36:28.8024891+07:00'
WHERE [Id] = '59bfc647-ef93-4af4-aaf0-4c49272a975b';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'55b23bd7-c0ca-4df5-ba39-a3f67f0a8ae0'
WHERE [Id] = N'3e1ce2a6-e835-41ff-ab54-11dc1e60e839';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'58b41150-3136-4457-a350-89362ef39dca'
WHERE [Id] = N'672db3b8-c436-49bd-8172-bdb6ad6d6148';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'41ea1550-3e6a-43bf-8b12-bce94aabaa86'
WHERE [Id] = N'db29c853-03ea-4328-9553-83676192aeed';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230626023630_20230626', N'5.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Gender');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Gender] int NULL;
GO

UPDATE [App_Sequence] SET [CreatedDate] = '2023-06-26T09:39:48.8022058+07:00'
WHERE [Id] = 'efb6f443-337c-4f7e-afa9-328bec063f21';
SELECT @@ROWCOUNT;

GO

UPDATE [App_SequenceLine] SET [CreatedDate] = '2023-06-26T09:39:48.8043639+07:00'
WHERE [Id] = '59bfc647-ef93-4af4-aaf0-4c49272a975b';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'9ce11396-11c8-4d30-b665-6d95e82c9cab'
WHERE [Id] = N'3e1ce2a6-e835-41ff-ab54-11dc1e60e839';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'f64101c4-dcef-4925-a64f-c00ae91147d4'
WHERE [Id] = N'672db3b8-c436-49bd-8172-bdb6ad6d6148';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'4f437296-2222-48b9-82a5-81038d62ab3c'
WHERE [Id] = N'db29c853-03ea-4328-9553-83676192aeed';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230626023950_20230626-1', N'5.0.10');
GO

COMMIT;
GO

