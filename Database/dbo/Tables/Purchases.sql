CREATE TABLE [dbo].[Purchases] (
    [ID]        INT IDENTITY (1, 1) NOT NULL,
    [AccountID] INT NOT NULL,
    [GoodID]    INT NOT NULL,
    CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED ([ID] ASC)
);

