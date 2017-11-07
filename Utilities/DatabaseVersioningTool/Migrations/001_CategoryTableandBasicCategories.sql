CREATE TABLE [dbo].[tblCategory] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name]		   NVARCHAR (200) NOT NULL,
    [SimpleName]        NVARCHAR (256) NOT NULL,
    [Status]       NVARCHAR (100) NOT NULL
)

CREATE TABLE [dbo].[tblCategoryRel] (
    [ID]	   BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Parent_Cat]		   int NOT NULL,
    [Child_Cat]        int NOT NULL,
    [Status]       NVARCHAR (100) NOT NULL
)

