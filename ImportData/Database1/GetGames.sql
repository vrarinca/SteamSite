CREATE PROCEDURE InsertGamesFromList
	@gamesList as dbo.Game READONLY	
AS
BEGIN
	INSERT INTO Game (Appid, Name)
	SELECT Appid, Name
	FROM @gamesList;
END