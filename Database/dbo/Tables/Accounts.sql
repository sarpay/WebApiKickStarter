CREATE TABLE [dbo].[Accounts] (
    [ID]       INT           IDENTITY (1, 1) NOT NULL,
    [Email]    VARCHAR (255) NOT NULL,
    [Password] NVARCHAR (50) NOT NULL,
    [RegDate]  SMALLDATETIME CONSTRAINT [DF_Accounts_RegDate] DEFAULT (sysdatetime()) NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Accounts_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);

