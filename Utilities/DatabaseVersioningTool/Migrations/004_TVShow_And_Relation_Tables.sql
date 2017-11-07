CREATE TABLE [dbo].[tblTvShow] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name]		   NVARCHAR (200) NOT NULL,
	[Overview]		   NVARCHAR (2500),
	[First_Air_Date]		   DATE,
	[Episode_Run_Time]  BIGINT,
	[TMDB_ID]		   BIGINT,
	[IMDB_ID] 		   NVARCHAR (200),
	[TVDB_ID]		   BIGINT,
	[TMDB_Poster_Path] NVARCHAR (200) ,
	[TMDB_Backdrop_Path] NVARCHAR (200)
)
CREATE TABLE [dbo].[tblTvShowSeason] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Number]	   BIGINT NOT NULL,
    [Name]		   NVARCHAR (200) NOT NULL,
	[Overview]		   NVARCHAR (1000) NOT NULL,
	[PosterPath]		   NVARCHAR (1000) NOT NULL,
	[First_Air_Date]		   NVARCHAR (20),
	[TMDB_ID]  int
)
CREATE TABLE [dbo].[tblTvShowEpisode] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Number]	   BIGINT NOT NULL,
    [Name]		   NVARCHAR (200) NOT NULL,
	[Overview]		   NVARCHAR (1000) NOT NULL,
	[StillPath]		   NVARCHAR (1000) NOT NULL,
	[First_Air_Date]		   NVARCHAR (20),
	[IMDB_ID] 		   NVARCHAR (200) NOT NULL,
	[TMDB_ID]  BIGINT
)
;
