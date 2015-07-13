CREATE PROCEDURE [dbo].[signIn]
	
	@Username VARCHAR(255),
	@Password CHAR(60),
	@Ticket CHAR(36) OUTPUT
	
AS

BEGIN

	DECLARE @UserID INT
	SET @UserID = 0
	SET @Ticket = '0'
	
	-- check for sign-in credentials
	SELECT @UserID = [ID]
	FROM [Accounts]
	WHERE Email=@Username AND [Password]=@Password
	
	-- create ticket only if sign-in is successful
	IF (@UserID > 0)
	BEGIN
	
		-- specify ticket validation window
		-- updated every time user interacts [getCheckAuth]
		DECLARE @CreatedOn DATETIME
		DECLARE @ExpiresOn DATETIME
		SET @CreatedOn = GETDATE()
		SET @ExpiresOn = DATEADD(HOUR,1,@CreatedOn)
		
		-- attempt to create new ticket
		DECLARE @GUID UNIQUEIDENTIFIER
		SET @GUID = NEWID()
		
		-- check if this ticket already exists
		DECLARE @TicketExists BIT
		SET @TicketExists = 0
		SELECT @TicketExists = COUNT([UserID])
		FROM [UserTickets]
		WHERE [Ticket] = @GUID
		
		-- loop and create new ticket until it finds a unique one
		WHILE (@TicketExists = 1)
		BEGIN
			SET @GUID = NEWID()
			SELECT @TicketExists = COUNT([UserID])
			FROM [UserTickets]
			WHERE [Ticket] = @GUID
		END
		
		-- convert GUID to string and assign to output ticket variable
		SET @Ticket = CAST(@GUID AS CHAR(36))
		
		-- has this user ever signed in and acquired a ticket before?
		DECLARE @UserExists BIT
		SELECT @UserExists = COUNT([UserID])
		FROM [UserTickets]
		WHERE [UserID] = @UserID
		
		-- assign new ticket to existing user
		IF (@UserExists = 1)
		BEGIN
			UPDATE 
				[UserTickets]
			SET 
				[Ticket] = @Ticket, 
				[CreatedOn] = @CreatedOn, 
				[ExpiresOn] = @ExpiresOn, 
				[SignInCount] = [SignInCount] + 1
			WHERE 
				[UserID] = @UserID
		END
		
		-- add new user and ticket
		IF (@UserExists = 0)
		BEGIN
			INSERT INTO [UserTickets] 
				([UserID], [Ticket], [CreatedOn], [ExpiresOn], [SignInCount])
			VALUES
				(@UserID, @Ticket, @CreatedOn, @ExpiresOn, 1)
		END
		
	END --IF (@UserID > 0)
	
END