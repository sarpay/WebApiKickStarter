CREATE PROCEDURE [dbo].[newShopper]

	@AccountID INT,
	@Name NVARCHAR(100),
	@GenderIX TINYINT,
	@OptIn BIT
		
AS

BEGIN

	INSERT INTO
		[Shoppers]
	
	(
		[AccountID],
		[Name],
		[GenderIX],
		[OptIn]
	)	
	
	VALUES
	
	(
		@AccountID,
		UPPER(@Name),
		@GenderIX,
		@OptIn
	)
		
END




