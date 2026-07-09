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
CREATE TABLE [MarketingLeadStatuses] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(50) NOT NULL,
    [Label] nvarchar(100) NOT NULL,
    [IsRecurring] bit NOT NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_MarketingLeadStatuses] PRIMARY KEY ([Id])
);

CREATE TABLE [MarketingLeadTemperatures] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(50) NOT NULL,
    [Label] nvarchar(100) NOT NULL,
    [Color] nvarchar(20) NOT NULL,
    [SoftColor] nvarchar(40) NOT NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_MarketingLeadTemperatures] PRIMARY KEY ([Id])
);

CREATE TABLE [MarketingOwners] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(80) NOT NULL,
    [Color] nvarchar(20) NOT NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_MarketingOwners] PRIMARY KEY ([Id])
);

CREATE TABLE [MarketingSources] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(120) NOT NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_MarketingSources] PRIMARY KEY ([Id])
);

CREATE TABLE [MarketingStages] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(50) NOT NULL,
    [Label] nvarchar(100) NOT NULL,
    [Color] nvarchar(20) NOT NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_MarketingStages] PRIMARY KEY ([Id])
);

CREATE TABLE [MarketingLeads] (
    [Id] int NOT NULL IDENTITY,
    [ExternalId] nvarchar(40) NOT NULL,
    [Company] nvarchar(250) NOT NULL,
    [Contact] nvarchar(250) NULL,
    [Email] nvarchar(250) NULL,
    [SourceId] int NOT NULL,
    [StatusId] int NOT NULL,
    [TemperatureId] int NOT NULL,
    [StageId] int NOT NULL,
    [Value] decimal(18,2) NOT NULL,
    [Date] datetime2 NOT NULL,
    [Note] nvarchar(max) NULL,
    [SortOrder] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_MarketingLeads] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MarketingLeads_MarketingLeadStatuses_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [MarketingLeadStatuses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MarketingLeads_MarketingLeadTemperatures_TemperatureId] FOREIGN KEY ([TemperatureId]) REFERENCES [MarketingLeadTemperatures] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MarketingLeads_MarketingSources_SourceId] FOREIGN KEY ([SourceId]) REFERENCES [MarketingSources] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MarketingLeads_MarketingStages_StageId] FOREIGN KEY ([StageId]) REFERENCES [MarketingStages] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [MarketingLeadOwners] (
    [MarketingLeadId] int NOT NULL,
    [MarketingOwnerId] int NOT NULL,
    [SortOrder] int NOT NULL,
    CONSTRAINT [PK_MarketingLeadOwners] PRIMARY KEY ([MarketingLeadId], [MarketingOwnerId]),
    CONSTRAINT [FK_MarketingLeadOwners_MarketingLeads_MarketingLeadId] FOREIGN KEY ([MarketingLeadId]) REFERENCES [MarketingLeads] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MarketingLeadOwners_MarketingOwners_MarketingOwnerId] FOREIGN KEY ([MarketingOwnerId]) REFERENCES [MarketingOwners] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsRecurring', N'Key', N'Label', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingLeadStatuses]'))
    SET IDENTITY_INSERT [MarketingLeadStatuses] ON;
INSERT INTO [MarketingLeadStatuses] ([Id], [IsRecurring], [Key], [Label], [SortOrder])
VALUES (1, CAST(1 AS bit), N'duzenli', N'Düzenli İş', 1),
(2, CAST(0 AS bit), N'dis', N'Dış İş', 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsRecurring', N'Key', N'Label', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingLeadStatuses]'))
    SET IDENTITY_INSERT [MarketingLeadStatuses] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Key', N'Label', N'SoftColor', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingLeadTemperatures]'))
    SET IDENTITY_INSERT [MarketingLeadTemperatures] ON;
INSERT INTO [MarketingLeadTemperatures] ([Id], [Color], [Key], [Label], [SoftColor], [SortOrder])
VALUES (1, N'#FF7A3C', N'sicak', N'Sıcak', N'rgba(255,122,60,.16)', 1),
(2, N'#4FA0E6', N'soguk', N'Soğuk', N'rgba(79,160,230,.16)', 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Key', N'Label', N'SoftColor', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingLeadTemperatures]'))
    SET IDENTITY_INSERT [MarketingLeadTemperatures] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Name', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingOwners]'))
    SET IDENTITY_INSERT [MarketingOwners] ON;
INSERT INTO [MarketingOwners] ([Id], [Color], [Name], [SortOrder])
VALUES (1, N'#FF7A3C', N'Ege', 1),
(2, N'#E0544E', N'Fırat', 2),
(3, N'#4FA0E6', N'Emre', 3),
(4, N'#39C07A', N'Göksel', 4);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Name', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingOwners]'))
    SET IDENTITY_INSERT [MarketingOwners] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingSources]'))
    SET IDENTITY_INSERT [MarketingSources] ON;
INSERT INTO [MarketingSources] ([Id], [Name], [SortOrder])
VALUES (1, N'Referans', 1),
(2, N'Instagram', 2),
(3, N'Google', 3),
(4, N'Web Sitesi', 4),
(5, N'LinkedIn', 5),
(6, N'Soğuk Arama', 6),
(7, N'Fuar / Etkinlik', 7);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingSources]'))
    SET IDENTITY_INSERT [MarketingSources] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Key', N'Label', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingStages]'))
    SET IDENTITY_INSERT [MarketingStages] ON;
INSERT INTO [MarketingStages] ([Id], [Color], [Key], [Label], [SortOrder])
VALUES (1, N'#9AA0A6', N'new', N'Yeni', 1),
(2, N'#4FA0E6', N'contacted', N'Görüşme', 2),
(3, N'#FF7A3C', N'proposal', N'Teklif', 3),
(4, N'#E6A93C', N'negotiation', N'Müzakere', 4),
(5, N'#39C07A', N'won', N'Satış Tamamlandı', 5),
(6, N'#E0544E', N'lost', N'Kaybedildi', 6);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Key', N'Label', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingStages]'))
    SET IDENTITY_INSERT [MarketingStages] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Company', N'Contact', N'CreatedAt', N'Date', N'Email', N'ExternalId', N'Note', N'SortOrder', N'SourceId', N'StageId', N'StatusId', N'TemperatureId', N'UpdatedAt', N'Value') AND [object_id] = OBJECT_ID(N'[MarketingLeads]'))
    SET IDENTITY_INSERT [MarketingLeads] ON;
INSERT INTO [MarketingLeads] ([Id], [Company], [Contact], [CreatedAt], [Date], [Email], [ExternalId], [Note], [SortOrder], [SourceId], [StageId], [StatusId], [TemperatureId], [UpdatedAt], [Value])
VALUES (1, N'Alaş İnşaat', N'', '2026-07-08T00:00:00.0000000', '2026-05-15T00:00:00.0000000', N'', N'c1', N'', 1, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 110000.0),
(2, N'Bilişim Garajı', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c2', N'', 2, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 102000.0),
(3, N'Pro Estetik Diş Kliniği', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c3', N'', 3, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 92500.0),
(4, N'Gerilim Enerji', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'c4', N'', 4, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 79800.0),
(5, N'Dr. Çağlar İmançer', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c5', N'', 5, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 75000.0),
(6, N'Güzelbahçe Fen Bilimleri Koleji', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'c6', N'', 6, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 75000.0),
(7, N'Dr. Muzaffer Tunç', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c7', N'', 7, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 65000.0),
(8, N'Pal Mühendislik', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'c8', N'', 8, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 62700.0),
(9, N'Boğatepe Köy Mandırası', N'', '2026-07-08T00:00:00.0000000', '2026-02-15T00:00:00.0000000', N'', N'c9', N'', 9, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 50000.0),
(10, N'Dr. Özlem Gürbüz Nazlı', N'', '2026-07-08T00:00:00.0000000', '2026-02-15T00:00:00.0000000', N'', N'c10', N'', 10, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 45600.0),
(11, N'Catchupper', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c11', N'', 11, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 45600.0),
(12, N'Ercey Design', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c12', N'', 12, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 41040.0),
(13, N'Dr. Mehmet Sucubaşı', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c13', N'', 13, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 40000.0),
(14, N'Prof. Dr. Törün Özer', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'c14', N'', 14, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 40000.0),
(15, N'Esbay OSGB', N'', '2026-07-08T00:00:00.0000000', '2026-02-15T00:00:00.0000000', N'', N'c15', N'', 15, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 40000.0),
(16, N'Dr. Sevil Tunabayoğlu', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c16', N'', 16, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 36000.0),
(17, N'Berkay & Koray Nazlı', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'c17', N'', 17, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 35000.0),
(18, N'Dr. Süleyman & Dr. Sezin', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c18', N'', 18, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 56000.0),
(19, N'Uzm. Dr. Sibel Karkaç', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c19', N'', 19, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 25000.0),
(20, N'Ossi Hair', N'', '2026-07-08T00:00:00.0000000', '2025-12-01T00:00:00.0000000', N'', N'c20', N'', 20, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 20000.0),
(21, N'Rota Home Bellona', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'c21', N'', 21, 1, 5, 1, 1, '2026-07-08T00:00:00.0000000', 55500.0),
(22, N'Fikret Mungan', N'', '2026-07-08T00:00:00.0000000', '2026-01-15T00:00:00.0000000', N'', N'd1', N'', 22, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 45600.0),
(23, N'Bombacı Zeydan', N'', '2026-07-08T00:00:00.0000000', '2026-01-15T00:00:00.0000000', N'', N'd2', N'', 23, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 30000.0),
(24, N'Özlem Gürbüz Nazlı', N'', '2026-07-08T00:00:00.0000000', '2026-02-15T00:00:00.0000000', N'', N'd3', N'', 24, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 45600.0),
(25, N'Dijital Futbol Akademi', N'', '2026-07-08T00:00:00.0000000', '2026-02-15T00:00:00.0000000', N'', N'd4', N'', 25, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 300000.0),
(26, N'Bilişim Garajı (Ekstralar)', N'', '2026-07-08T00:00:00.0000000', '2026-02-15T00:00:00.0000000', N'', N'd5', N'', 26, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 57000.0),
(27, N'İlkim Makina', N'', '2026-07-08T00:00:00.0000000', '2026-02-15T00:00:00.0000000', N'', N'd6', N'', 27, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 20000.0),
(28, N'Bombacı Zeydan', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'd7', N'', 28, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 10000.0),
(29, N'Jaggermaister', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'd8', N'', 29, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 12000.0),
(30, N'İlkim Mühendislik', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'd9', N'', 30, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 151000.0),
(31, N'Günseli Uyar', N'', '2026-07-08T00:00:00.0000000', '2026-03-15T00:00:00.0000000', N'', N'd10', N'', 31, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 10000.0),
(32, N'Gold Performans', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'd11', N'', 32, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 207000.0),
(33, N'Oben Home', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'd12', N'', 33, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 80000.0),
(34, N'Viora Coffe', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'd13', N'', 34, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 6840.0),
(35, N'Pedalanka', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'd14', N'', 35, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 15000.0),
(36, N'Bilet Point', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'd15', N'', 36, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 29000.0),
(37, N'İlkim Makina', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'd16', N'', 37, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 200000.0),
(38, N'Zişan Cin', N'', '2026-07-08T00:00:00.0000000', '2026-04-15T00:00:00.0000000', N'', N'd17', N'', 38, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 40000.0),
(39, N'Gold Performans', N'', '2026-07-08T00:00:00.0000000', '2026-05-15T00:00:00.0000000', N'', N'd18', N'', 39, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 163000.0),
(40, N'Pal Mühendislik', N'', '2026-07-08T00:00:00.0000000', '2026-05-15T00:00:00.0000000', N'', N'd19', N'', 40, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 30000.0),
(41, N'Oha Patch', N'', '2026-07-08T00:00:00.0000000', '2026-05-15T00:00:00.0000000', N'', N'd20', N'', 41, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 54000.0),
(42, N'Keep Risen', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'd21', N'', 42, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 50000.0);
INSERT INTO [MarketingLeads] ([Id], [Company], [Contact], [CreatedAt], [Date], [Email], [ExternalId], [Note], [SortOrder], [SourceId], [StageId], [StatusId], [TemperatureId], [UpdatedAt], [Value])
VALUES (43, N'Proline', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'd22', N'', 43, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 85500.0),
(44, N'Zero1', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'd23', N'', 44, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 34200.0),
(45, N'Welness', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'd24', N'', 45, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 25000.0),
(46, N'İzmir Avukat Hareketi (İZAH)', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'd25', N'', 46, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 370000.0),
(47, N'Proline (Katalog)', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'd26', N'', 47, 1, 5, 2, 1, '2026-07-08T00:00:00.0000000', 57000.0),
(48, N'C3 Teknoloji', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p1', N'', 48, 1, 4, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(49, N'Bilimsev Koleji', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p2', N'', 49, 1, 3, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(50, N'Sanat Garajı', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p3', N'', 50, 1, 2, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(51, N'İzmir Avukat Hareketi', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p4', N'', 51, 1, 4, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(52, N'Lazarus', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p5', N'', 52, 1, 3, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(53, N'Oha Yatch', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p6', N'', 53, 1, 4, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(54, N'Boatfinder', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p7', N'', 54, 1, 4, 2, 1, '2026-07-08T00:00:00.0000000', 0.0),
(55, N'İzmir Cerrahi Onkoloji', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p8', N'', 55, 1, 3, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(56, N'Wellnes', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p9', N'', 56, 1, 3, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(57, N'SANGROW', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p10', N'', 57, 1, 1, 1, 2, '2026-07-08T00:00:00.0000000', 0.0),
(58, N'Netforce', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p11', N'', 58, 1, 1, 1, 2, '2026-07-08T00:00:00.0000000', 0.0),
(59, N'Metod Koleji', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p12', N'', 59, 1, 1, 1, 2, '2026-07-08T00:00:00.0000000', 0.0),
(60, N'Proline', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p13', N'', 60, 1, 2, 1, 2, '2026-07-08T00:00:00.0000000', 0.0),
(61, N'İlkim Makina', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p14', N'', 61, 1, 2, 1, 2, '2026-07-08T00:00:00.0000000', 0.0),
(62, N'4R Mühendislik', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p15', N'', 62, 1, 2, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(63, N'Arkas Holding', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p16', N'', 63, 1, 3, 2, 1, '2026-07-08T00:00:00.0000000', 0.0),
(64, N'Cour de Lion', N'', '2026-07-08T00:00:00.0000000', '2026-06-15T00:00:00.0000000', N'', N'p17', N'', 64, 1, 2, 1, 1, '2026-07-08T00:00:00.0000000', 0.0),
(65, N'The Vets Hub', N'', '2026-07-08T00:00:00.0000000', '2026-01-20T00:00:00.0000000', N'', N'x1', N'', 65, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 20000.0),
(66, N'Zero1', N'', '2026-07-08T00:00:00.0000000', '2026-01-20T00:00:00.0000000', N'', N'x2', N'', 66, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 51300.0),
(67, N'Pekari', N'', '2026-07-08T00:00:00.0000000', '2026-01-20T00:00:00.0000000', N'', N'x3', N'', 67, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 28500.0),
(68, N'Funce Medical', N'', '2026-07-08T00:00:00.0000000', '2026-01-20T00:00:00.0000000', N'', N'x4', N'', 68, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 45000.0),
(69, N'Baskın Kocabaş', N'', '2026-07-08T00:00:00.0000000', '2026-01-20T00:00:00.0000000', N'', N'x5', N'', 69, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 35000.0),
(70, N'Tolga Bıçakcı', N'', '2026-07-08T00:00:00.0000000', '2026-01-20T00:00:00.0000000', N'', N'x6', N'', 70, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 35000.0),
(71, N'Çiler Ezgi', N'', '2026-07-08T00:00:00.0000000', '2026-02-20T00:00:00.0000000', N'', N'x7', N'', 71, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 57000.0),
(72, N'Bjorn Coffe', N'', '2026-07-08T00:00:00.0000000', '2026-02-20T00:00:00.0000000', N'', N'x8', N'', 72, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 34200.0),
(73, N'Kaan Akacun', N'', '2026-07-08T00:00:00.0000000', '2026-02-20T00:00:00.0000000', N'', N'x9', N'', 73, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 33000.0),
(74, N'Dino Pizza', N'', '2026-07-08T00:00:00.0000000', '2026-02-20T00:00:00.0000000', N'', N'x10', N'', 74, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 30000.0),
(75, N'Saygın Health', N'', '2026-07-08T00:00:00.0000000', '2026-02-20T00:00:00.0000000', N'', N'x11', N'', 75, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 35000.0),
(76, N'Ecem Cantürk Nazlı', N'', '2026-07-08T00:00:00.0000000', '2026-03-20T00:00:00.0000000', N'', N'x12', N'', 76, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 51300.0),
(77, N'Zişan Cin', N'', '2026-07-08T00:00:00.0000000', '2026-03-20T00:00:00.0000000', N'', N'x13', N'', 77, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 34200.0),
(78, N'Hakkı Kurt', N'', '2026-07-08T00:00:00.0000000', '2026-05-20T00:00:00.0000000', N'', N'x14', N'', 78, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 40000.0),
(79, N'Bilet Points', N'', '2026-07-08T00:00:00.0000000', '2026-05-20T00:00:00.0000000', N'', N'x15', N'', 79, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 80000.0),
(80, N'Venettia Pizza', N'', '2026-07-08T00:00:00.0000000', '2026-06-20T00:00:00.0000000', N'', N'x16', N'', 80, 1, 6, 1, 1, '2026-07-08T00:00:00.0000000', 25000.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Company', N'Contact', N'CreatedAt', N'Date', N'Email', N'ExternalId', N'Note', N'SortOrder', N'SourceId', N'StageId', N'StatusId', N'TemperatureId', N'UpdatedAt', N'Value') AND [object_id] = OBJECT_ID(N'[MarketingLeads]'))
    SET IDENTITY_INSERT [MarketingLeads] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MarketingLeadId', N'MarketingOwnerId', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingLeadOwners]'))
    SET IDENTITY_INSERT [MarketingLeadOwners] ON;
INSERT INTO [MarketingLeadOwners] ([MarketingLeadId], [MarketingOwnerId], [SortOrder])
VALUES (1, 1, 1),
(1, 3, 2),
(2, 1, 1),
(2, 2, 2),
(3, 2, 2),
(3, 3, 1),
(4, 2, 1),
(5, 3, 1),
(6, 4, 1),
(7, 3, 1),
(8, 1, 1),
(9, 2, 1),
(10, 4, 1),
(11, 1, 1),
(12, 3, 1),
(13, 2, 1),
(14, 2, 1),
(15, 2, 1),
(16, 3, 1),
(17, 4, 1),
(18, 3, 1),
(19, 3, 1),
(20, 4, 1),
(21, 1, 1),
(22, 2, 1),
(23, 1, 1),
(23, 2, 2),
(24, 4, 1),
(25, 2, 1),
(26, 1, 1),
(27, 2, 1),
(28, 2, 1),
(29, 3, 1),
(30, 2, 1),
(31, 1, 1),
(32, 2, 1),
(33, 3, 1),
(34, 4, 1),
(35, 2, 1),
(36, 4, 1),
(37, 2, 1),
(38, 2, 1);
INSERT INTO [MarketingLeadOwners] ([MarketingLeadId], [MarketingOwnerId], [SortOrder])
VALUES (39, 2, 1),
(40, 1, 1),
(41, 3, 1),
(42, 2, 1),
(43, 4, 1),
(44, 4, 1),
(45, 4, 1),
(46, 1, 1),
(46, 2, 2),
(47, 4, 1),
(48, 1, 1),
(49, 1, 1),
(50, 2, 1),
(51, 1, 1),
(52, 3, 1),
(53, 3, 1),
(54, 3, 1),
(55, 4, 1),
(56, 4, 1),
(57, 1, 1),
(58, 1, 1),
(59, 1, 1),
(60, 4, 1),
(61, 4, 1),
(62, 1, 1),
(63, 1, 1),
(64, 3, 1),
(65, 3, 1),
(66, 4, 1),
(67, 2, 1),
(68, 3, 1),
(69, 3, 1),
(70, 3, 1),
(71, 2, 1),
(72, 2, 1),
(73, 3, 1),
(74, 3, 1),
(75, 3, 1),
(76, 4, 1),
(77, 2, 1),
(78, 4, 1),
(79, 4, 1);
INSERT INTO [MarketingLeadOwners] ([MarketingLeadId], [MarketingOwnerId], [SortOrder])
VALUES (80, 1, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MarketingLeadId', N'MarketingOwnerId', N'SortOrder') AND [object_id] = OBJECT_ID(N'[MarketingLeadOwners]'))
    SET IDENTITY_INSERT [MarketingLeadOwners] OFF;

CREATE INDEX [IX_MarketingLeadOwners_MarketingOwnerId] ON [MarketingLeadOwners] ([MarketingOwnerId]);

CREATE UNIQUE INDEX [IX_MarketingLeads_ExternalId] ON [MarketingLeads] ([ExternalId]);

CREATE INDEX [IX_MarketingLeads_SourceId] ON [MarketingLeads] ([SourceId]);

CREATE INDEX [IX_MarketingLeads_StageId] ON [MarketingLeads] ([StageId]);

CREATE INDEX [IX_MarketingLeads_StatusId] ON [MarketingLeads] ([StatusId]);

CREATE INDEX [IX_MarketingLeads_TemperatureId] ON [MarketingLeads] ([TemperatureId]);

CREATE UNIQUE INDEX [IX_MarketingLeadStatuses_Key] ON [MarketingLeadStatuses] ([Key]);

CREATE UNIQUE INDEX [IX_MarketingLeadTemperatures_Key] ON [MarketingLeadTemperatures] ([Key]);

CREATE UNIQUE INDEX [IX_MarketingOwners_Name] ON [MarketingOwners] ([Name]);

CREATE UNIQUE INDEX [IX_MarketingSources_Name] ON [MarketingSources] ([Name]);

CREATE UNIQUE INDEX [IX_MarketingStages_Key] ON [MarketingStages] ([Key]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260708085056_AddMarketingFeature', N'10.0.5');

COMMIT;
GO

