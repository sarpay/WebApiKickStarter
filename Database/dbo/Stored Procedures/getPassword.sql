CREATE PROCEDURE [dbo].[getPassword]
	
	@Username VARCHAR(255),
	
	@UserID INT OUTPUT,
	@Password CHAR(60) OUTPUT
	
AS

BEGIN
	
	SELECT 
		@UserID = [ID],
		@Password = [Password]
	FROM 
		[Accounts]
	WHERE 
		[Email] = @Username
	
END