BEGIN TRANSACTION;
GO

ALTER TABLE [GF_Booking] ADD [AccountFullName] nvarchar(500) NULL;
GO

ALTER TABLE [GF_Booking] ADD [AccountName] nvarchar(200) NULL;
GO

ALTER TABLE [GF_Booking] ADD [BookedCardNo] nvarchar(500) NULL;
GO

ALTER TABLE [GF_Booking] ADD [MemberType] nvarchar(500) NULL;
GO

ALTER TABLE [GF_Booking] ADD [TeeTimeDisplay] nvarchar(500) NULL;
GO

UPDATE [App_Sequence] SET [CreatedDate] = '2023-04-24T08:34:45.1203912+07:00'
WHERE [Id] = 'efb6f443-337c-4f7e-afa9-328bec063f21';
SELECT @@ROWCOUNT;

GO

UPDATE [App_SequenceLine] SET [CreatedDate] = '2023-04-24T08:34:45.1225111+07:00'
WHERE [Id] = '59bfc647-ef93-4af4-aaf0-4c49272a975b';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'76ad8ed8-1330-477d-810a-69e160c61f89'
WHERE [Id] = N'3e1ce2a6-e835-41ff-ab54-11dc1e60e839';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'af99c01e-44f6-460b-9de6-360d7e95a6bd'
WHERE [Id] = N'672db3b8-c436-49bd-8172-bdb6ad6d6148';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'28f61797-24da-4283-88f8-05db416d5aca'
WHERE [Id] = N'db29c853-03ea-4328-9553-83676192aeed';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230424013447_20230424', N'5.0.10');
GO

COMMIT;
GO

