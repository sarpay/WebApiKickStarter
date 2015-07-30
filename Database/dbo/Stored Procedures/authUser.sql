CREATE PROCEDURE [dbo].[authUser]
	
	@UserID INT,
	@Ticket CHAR(36),
	@Authenticated BIT = 0 OUTPUT
	
AS

BEGIN
	
	IF EXISTS (
		SELECT 
			[UserID]
		FROM 
			[UserTickets]
		WHERE 
			[UserID] = @UserID
			AND [Ticket]=@Ticket
			AND [ExpiresOn] > GETDATE()
	)
	BEGIN
	  SET @Authenticated = 1
	END

	-- reset expiration
	-- add 1 more hour to validation window
	-- first set at login - [signIn]
	IF (@Authenticated = 1)
	BEGIN
		DECLARE @ExpiresOn DATETIME
		SET @ExpiresOn = DATEADD(HOUR,1,GETDATE())
		UPDATE
			[UserTickets]
		SET
			[ExpiresOn] = @ExpiresOn
		WHERE
			[Ticket]=@Ticket 
	END
	
END