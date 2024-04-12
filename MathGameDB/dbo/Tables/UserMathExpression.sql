CREATE TABLE [dbo].[UserMathExpression]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Uid] UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [DeletedOn] DATETIME NULL,
    [UserInGameSessionFk] INT NOT NULL, 
    [MathExpressionFk] INT NOT NULL,

    CONSTRAINT FK_UserMathExpression_UserInGameSession FOREIGN KEY (UserInGameSessionFk)
    REFERENCES [dbo].[UserInGameSession](Id),

    CONSTRAINT FK_UserMathExpression_MathExpression FOREIGN KEY (MathExpressionFk)
    REFERENCES [dbo].[MathExpression](Id),
)
