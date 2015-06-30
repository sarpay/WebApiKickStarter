CREATE PROCEDURE [dbo].[getShopper]

	@AccountID INT
		
AS

BEGIN

	SELECT 
		[A].[ID],
		[A].[Email],
		[S].[Name],
		[G].[Text] [Gender],
		[S].[OptIn],
		[A].[RegDate]

	FROM [Shoppers] [S]
		INNER JOIN [Accounts] [A] ON [A].[ID] = [S].[AccountID]
		LEFT JOIN [Genders] [G] ON [G].[IX] = [S].[GenderIX] -- LEFT JOIN SINCE [GENDER] MAY BE NULL

	WHERE
		[A].[ID] = @AccountID
		
END

