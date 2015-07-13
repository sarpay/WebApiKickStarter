CREATE TABLE [dbo].[UserTickets] (
    [UserID]      INT       NOT NULL,
    [Ticket]      CHAR (36) NOT NULL,
    [CreatedOn]   DATETIME  NOT NULL,
    [ExpiresOn]   DATETIME  NOT NULL,
    [SignInCount] INT       NOT NULL, 
    CONSTRAINT [PK_UserTickets] PRIMARY KEY ([UserID])
);

