CREATE TABLE [dbo].[Goods] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (150) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Price]       SMALLMONEY     NULL,
    CONSTRAINT [PK_Goods] PRIMARY KEY CLUSTERED ([ID] ASC)
);

