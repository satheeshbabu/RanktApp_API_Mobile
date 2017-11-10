CREATE TABLE [dbo].[tblGenreMovie] (
    [ID]	   BIGINT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
    [Source_Cat]		   BIGINT NOT NULL,
    [Source_Name]		   NVARCHAR (200) NOT NULL,
    [Source_Id]		   BIGINT
)
CREATE TABLE [dbo].[tblGenreTvShow] (
    [ID]	   BIGINT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
    [Source_Cat]		   BIGINT NOT NULL,
    [Source_Name]		   NVARCHAR (200) NOT NULL,
    [Source_Id]		   BIGINT
)
CREATE TABLE [dbo].[tblMovie] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name]		   NVARCHAR (200) NOT NULL,
	[Overview]		   NVARCHAR (1000) NOT NULL,
	[Release_Date]		   DATE,
	[Run_Time]				int,
	[TMDB_ID]		   BIGINT,
	[IMDB_ID] 		   NVARCHAR (200) NOT NULL,
	[TMDB_Poster_Path] NVARCHAR (200),
	[TMDB_Backdrop_Path] NVARCHAR (200),
	[Date_Updated]		DATETIME NOT NULL
)
CREATE TABLE [dbo].[tblRelation](
	[ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Cat_From]  BIGINT  NOT NULL,
	[Entity_From] BIGINT NOT NULL,
	[Cat_To] BIGINT         NOT NULL,
	[Entity_To] BIGINT         NOT NULL,
	[Rel_Status] BIGINT NOT NULL,
	[Created_Date] Date
)
;