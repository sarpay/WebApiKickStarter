CREATE PROCEDURE [dbo].[getShoppers]

	@GenderIX TINYINT,	-- OPTIONAL
	@OptIn BIT			-- OPTIONAL
		
AS

BEGIN

	DECLARE @SqlString NVARCHAR(4000) -- OR VARCHAR(8000) these are max lengths

	SET @SqlString = N'' +
		N'SELECT ' +
			N'[A].[ID], ' +
			N'[A].[Email], ' +
			N'[S].[Name], ' +
			N'[G].[Text] [Gender], ' +
			N'[S].[OptIn], ' +
			N'[A].[RegDate] ' +
		N'FROM ' +
			N'[Shoppers] [S] ' +
			N'INNER JOIN [Accounts] [A] ON [A].[ID] = [S].[AccountID] ' +
			N'LEFT JOIN [Genders] [G] ON [G].[IX] = [S].[GenderIX] ' + -- LEFT JOIN SINCE [GENDER] MAY BE NULL
		N'WHERE ' +
			N'1=1'
			IF (@GenderIX IS NOT NULL)
			BEGIN
				SET @SqlString = @SqlString + N' ' +
				N'AND [S].[GenderIX]=' + CAST(@GenderIX AS CHAR(1))
			END
			IF (@OptIn IS NOT NULL)
			BEGIN
				SET @SqlString = @SqlString + N' ' +
				N'AND [S].[OptIn]=' + CAST(@OptIn AS CHAR(1))
			END
			SET @SqlString = @SqlString + N' '

	SET @SqlString = @SqlString + 
		N'ORDER BY ' +
			N'[A].[ID] DESC'
	
	--PRINT @SqlString
	EXEC sp_executesql @SqlString
	
END

