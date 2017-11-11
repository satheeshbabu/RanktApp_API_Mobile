CREATE TABLE [dbo].[tblUser] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username]		   NVARCHAR (200) NOT NULL,
	[Password]		   NVARCHAR (200) NOT NULL,
	[Email_Address]		NVARCHAR (200) NOT NULL,	
	[Date_Created]		DATETIME NOT NULL,
	[Date_Updated]		DATETIME NOT NULL,
	[Email_Verified]	BIT,
	[Last_Login]		DATETIME
)
CREATE TABLE [dbo].[tblAuthToken] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Token]	   NVARCHAR (200) NOT NULL,
	[User_Id]			BIGINT NOT NULL,
    [Date_Created]		DATETIME NOT NULL,
	[Date_Last_Used]	DATETIME NOT NULL,
	[Date_Expire]		DATETIME NOT NULL
)