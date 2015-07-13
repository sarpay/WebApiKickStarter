CREATE PROCEDURE [dbo].[newAccount]

	@Email VARCHAR(255),
	@Password CHAR(60),

	@NewID INT = NULL OUTPUT
		
AS

BEGIN

	INSERT INTO
		[Accounts]
	
	(
		[Email],
		[Password]
	)	
	
	VALUES
	
	(
		LOWER(@Email),
		@Password
	)

	SELECT @NewID = SCOPE_IDENTITY()
		
END




