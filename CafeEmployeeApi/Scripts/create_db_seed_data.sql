IF DB_ID('cafeemployeeapp_db') IS NULL
BEGIN
    CREATE DATABASE cafeemployeeapp_db;
END;
GO

USE [cafeemployeeapp_db]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 15/4/2025 1:01:49 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cafes]    Script Date: 15/4/2025 1:01:49 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cafes](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](10) NOT NULL,
	[Description] [nvarchar](256) NOT NULL,
	[Logo] [varbinary](max) NULL,
	[Location] [nvarchar](100) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[ETag] [timestamp] NOT NULL,
 CONSTRAINT [PK_Cafes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 15/4/2025 1:01:49 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](450) NOT NULL,
	[PhoneNumber] [nvarchar](8) NOT NULL,
	[Gender] [bit] NOT NULL,
	[CafeId] [uniqueidentifier] NULL,
	[StartDate] [datetime2](7) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
	[ETag] [timestamp] NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250409122710_InitDb', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250409123555_Update', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250409155936_RemoveEmploymentHistory', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250410104007_AlterEmployeeCafeIdFK', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250410144504_AddUniqueConstraints', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250410150044_UpdateUniqueConstraint', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250410152812_AddCreatedAndUpdateDates', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250410153033_DropLastModifiedDate', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250410154759_AddUpdateDate', N'9.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250411080942_AddEtagRowVersion', N'9.0.4')
GO
INSERT [dbo].[Cafes] ([Id], [Name], [Description], [Logo], [Location], [CreatedDate], [UpdatedDate]) VALUES (N'fd3035b8-2dd2-40aa-a4ca-77fa8c489daf', N'Cafe 3', N'Best cafe in town - 3', NULL, N'Singapore', CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2), CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2))
INSERT [dbo].[Cafes] ([Id], [Name], [Description], [Logo], [Location], [CreatedDate], [UpdatedDate]) VALUES (N'0cf93ebb-3918-4310-9033-8601ba0a3519', N'Cafe 5', N'Best cafe in town - 5', NULL, N'Singapore', CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2), CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2))
INSERT [dbo].[Cafes] ([Id], [Name], [Description], [Logo], [Location], [CreatedDate], [UpdatedDate]) VALUES (N'a5dac490-ddbc-4e77-bf49-8e6d7acfb968', N'Cafe 2', N'Best cafe in town - 2', NULL, N'Philippines', CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2), CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2))
INSERT [dbo].[Cafes] ([Id], [Name], [Description], [Logo], [Location], [CreatedDate], [UpdatedDate]) VALUES (N'ead52bb8-e0ea-4a01-8610-bab6f58aca1d', N'Cafe 4', N'Best cafe in town - 4', NULL, N'Singapore', CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2), CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2))
INSERT [dbo].[Cafes] ([Id], [Name], [Description], [Logo], [Location], [CreatedDate], [UpdatedDate]) VALUES (N'ab8a7d62-dcd9-47ca-a84c-bf7d89807846', N'Cafe 1', N'Best cafe in town - 1', NULL, N'US', CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2), CAST(N'2025-04-15T04:55:58.4833333' AS DateTime2))
GO
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UI25CkeuHT6Ye', N'Employee 2', N'employee2@cafe.com', N'82345672', 0, N'a5dac490-ddbc-4e77-bf49-8e6d7acfb968', CAST(N'2025-04-05T04:55:58.5617201' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UI48aEbiDGbxO', N'Employee 5', N'employee5@cafe.com', N'82345675', 1, N'ab8a7d62-dcd9-47ca-a84c-bf7d89807846', CAST(N'2025-04-05T04:55:58.5640128' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UICsyE6w8flC4', N'Employee Unassigned', N'employee3@cafe.com', N'92345673', 0, NULL, NULL, CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UIiEpCMguSDDQ', N'Employee 1', N'employee1@cafe.com', N'82345671', 0, N'fd3035b8-2dd2-40aa-a4ca-77fa8c489daf', CAST(N'2025-04-07T04:55:58.5371328' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UIOZbIEeG6Qie', N'Employee 4', N'employee4@cafe.com', N'82345674', 1, N'0cf93ebb-3918-4310-9033-8601ba0a3519', CAST(N'2025-04-06T04:55:58.5634369' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UIpD2lbtYz1je', N'Employee Unassigned', N'employee1@cafe.com', N'92345671', 0, NULL, NULL, CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UIQaA0qVCV7jL', N'Employee Unassigned', N'employee2@cafe.com', N'92345672', 0, NULL, NULL, CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UIRxor17kNqMs', N'Employee Unassigned', N'employee4@cafe.com', N'92345674', 1, NULL, NULL, CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UIyfwXcVxoecx', N'Employee Unassigned', N'employee5@cafe.com', N'92345675', 0, NULL, NULL, CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
INSERT [dbo].[Employees] ([Id], [Name], [Email], [PhoneNumber], [Gender], [CafeId], [StartDate], [CreatedDate], [UpdatedDate]) VALUES (N'UIYtfLHkJAQT3', N'Employee 3', N'employee3@cafe.com', N'82345673', 0, N'ab8a7d62-dcd9-47ca-a84c-bf7d89807846', CAST(N'2025-04-07T04:55:58.5628086' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2), CAST(N'2025-04-15T04:55:58.5966667' AS DateTime2))
GO
ALTER TABLE [dbo].[Cafes] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Cafes] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Cafes] ADD  DEFAULT (getutcdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT (getutcdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Employees] ADD  DEFAULT (getutcdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Cafes_CafeId] FOREIGN KEY([CafeId])
REFERENCES [dbo].[Cafes] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Cafes_CafeId]
GO
