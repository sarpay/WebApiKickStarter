CREATE PROCEDURE [dbo].[newPurchase]

	@AccountID INT,
	@GoodID INT,

	@NewID INT OUTPUT
		
AS

BEGIN

	INSERT INTO
		[Purchases]
	
	(
		[AccountID],
		[GoodID]
	)	
	
	VALUES
	
	(
		@AccountID,
		@GoodID
	)

	SELECT @NewID = SCOPE_IDENTITY()
		
END




