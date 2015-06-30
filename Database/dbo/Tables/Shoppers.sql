CREATE TABLE [dbo].[Shoppers] (
    [AccountID] INT            NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [GenderIX]  TINYINT        NULL,
    [OptIn]     BIT            NOT NULL,
    CONSTRAINT [PK_Shoppers] PRIMARY KEY CLUSTERED ([AccountID] ASC)
);

