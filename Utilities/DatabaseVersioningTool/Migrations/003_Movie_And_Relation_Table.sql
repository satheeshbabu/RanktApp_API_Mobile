CREATE TABLE [dbo].[tblMovieMedia](
	[ID]  BIGINT         NOT NULL PRIMARY KEY,
	[TMDB_Poster_Path] NVARCHAR (200) NOT NULL,
	[TMDB_Backdrop_Path] NVARCHAR (200) NOT NULL	
)
CREATE TABLE [dbo].[tblMovieCollection] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Source]    BIGINT NOT NULL, 
	[Source_Id]    BIGINT NOT NULL, 
    [Name]		   NVARCHAR (1000) NOT NULL,
	[Overview]		   NVARCHAR (4000) NOT NULL,
	[Poster]		   NVARCHAR (200),
	[Backdrop]  NVARCHAR (200)
);
CREATE TABLE [dbo].[tblMediaList] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[List_Type_Cat] BIGINT NOT NULL,
	[Source]    BIGINT NOT NULL, 
	[Source_Id]    NVARCHAR (1000) NOT NULL, 
    [Name]		   NVARCHAR (1000) NOT NULL,
	[Overview]		   NVARCHAR (4000) ,
	[Poster]		   NVARCHAR (200),
	[Backdrop]  NVARCHAR (200)
);
