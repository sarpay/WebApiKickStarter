CREATE PROCEDURE [dbo].[getPurchases]

	@GenderIX TINYINT,	-- OPTIONAL
	@GoodID INT			-- OPTIONAL
		
AS

BEGIN

	DECLARE @SqlString NVARCHAR(4000) -- OR VARCHAR(8000) these are max lengths

	SET @SqlString = N'' +
		N'SELECT ' +
			N'[A].[ID], ' +
			N'[A].[Email] [ShopperEmail], ' +
			N'[S].[Name] [ShopperName], ' +
			N'[G].[Text] [ShopperGender], ' +
			N'SUM([Goo].[Price]) [TotalPurchase], ' +
			N'[A].[RegDate] ' +
		N'FROM ' +
			N'[Accounts] [A] ' +
			N'INNER JOIN [Shoppers] [S] ON [S].[AccountID] = [A].[ID] ' +
			N'INNER JOIN [Purchases] [P] ON [P].[AccountID] = [A].[ID] ' +
			N'INNER JOIN [Goods] [Goo] ON [Goo].[ID] = [P].[GoodID] ' +
			N'LEFT JOIN [Genders] [G] ON [G].[IX] = [S].[GenderIX] ' + -- LEFT JOIN SINCE [GENDER IX] MAY BE NULL
		N'WHERE ' +
			N'1=1'
			IF (@GenderIX IS NOT NULL)
			BEGIN
				SET @SqlString = @SqlString + N' ' +
				N'AND [G].[IX]=' + CAST(@GenderIX AS CHAR(1))
			END
			IF (@GoodID IS NOT NULL)
			BEGIN
				SET @SqlString = @SqlString + N' ' +
				N'AND [Goo].[ID]=' + CAST(@GoodID AS NCHAR(32))
			END
		SET @SqlString = @SqlString + N' ' +
		N'GROUP BY [A].[ID], [A].[Email], [S].[Name], [G].[Text], [A].[RegDate] '

	SET @SqlString = @SqlString + 
		N'ORDER BY ' +
			N'[TotalPurchase] DESC'
	
	--PRINT @SqlString
	EXEC sp_executesql @SqlString
	
END