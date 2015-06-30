CREATE PROCEDURE [dbo].[getShopperPurchases]

	@AccountID INT
		
AS

BEGIN

	SELECT 
		[G].[Name],
		[G].[Description],
		[G].[Price]

	FROM [Purchases] [P]
		INNER JOIN [Goods] [G] ON [G].[ID] = [P].[GoodID]

	WHERE
		[P].[AccountID] = @AccountID

	ORDER BY 
		[P].[ID] DESC
		
END
