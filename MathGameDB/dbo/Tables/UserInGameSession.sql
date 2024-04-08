CREATE TABLE [dbo].[UserInGameSession]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Uid] UNIQUEIDENTIFIER NOT NULL,
	[CreatedOn] DateTime NOT NULL,
	[DeletedOn] DateTime NULL,
	[Score] INT NOT NULL DEFAULT(0),
	[UserFk] NVARCHAR(450) NOT NULL,
	[GameSessionFk] INT NOT NULL,

	CONSTRAINT FK_UserInSession_AspNetUsers FOREIGN KEY (UserFk)
		REFERENCES [dbo].[AspNetUsers](Id),


	CONSTRAINT FK_UserInSession_GameSession FOREIGN KEY (GameSessionFk)
		REFERENCES [dbo].[GameSession](Id)
 )
