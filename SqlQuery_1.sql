CREATE TABLE [dbo].[Companies] (
    [Id] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [PostCode] [nvarchar](max),
    [Image] [nvarchar](max),
    [Address] [nvarchar](max),
    [City] [nvarchar](max),
    [Phone] [nvarchar](max),
    [Connections] [int] NOT NULL,
    CONSTRAINT [PK_dbo.Companies] PRIMARY KEY ([Id])
)

ALTER TABLE [dbo].[Companies] ADD [CVR] [int] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[Companies] ADD [Web] [nvarchar](max)
ALTER TABLE [dbo].[Companies] ADD [Email] [nvarchar](max)
ALTER TABLE [dbo].[Companies] ADD [PersonName] [nvarchar](max)
ALTER TABLE [dbo].[Companies] ADD [Chairman] [nvarchar](max)
ALTER TABLE [dbo].[Companies] ADD [IndustryCode] [int] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[Companies] ADD [IndustryText] [nvarchar](max)
ALTER TABLE [dbo].[Companies] ADD [AdvertisingProtection] [bit] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[Companies] ADD [Owner] [nvarchar](max)