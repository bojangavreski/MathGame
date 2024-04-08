CREATE TABLE [dbo].[GameSession]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Uid] UNIQUEIDENTIFIER NOT NULL,
	[CreatedOn] DateTime NOT NULL,
	[DeletedOn] DateTime NULL
)
