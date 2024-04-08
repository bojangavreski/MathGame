CREATE TABLE [dbo].[MathExpression]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Uid] UNIQUEIDENTIFIER NOT NULL,
	[CreatedOn] DateTime NOT NULL,
	[DeletedOn] DateTime NULL,
	[QuestionType] INT NOT NULL DEFAULT(1),
	[GameSessionFk] INT NOT NULL,
	[DisplayedExpression] NVARCHAR(15) NOT NULL,
	[ActualResult] DECIMAL(8,2) NOT NULL,
	[DisplayedResult] DECIMAL(8,2) NULL,

	CONSTRAINT FK_MathExpression_GameSession FOREIGN KEY (GameSessionFk)
		REFERENCES [dbo].[GameSession](Id)
)
