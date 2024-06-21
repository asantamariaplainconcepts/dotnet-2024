DECLARE @workerCount INT;
SET @workerCount = (SELECT COUNT(*) FROM [Workers].[workers]);

IF @workerCount = 0
BEGIN
INSERT INTO [Workers].[workers] (Id, Name)
VALUES ('1', 'Ines'), ('2', 'Luisa'), ('3', 'Martina');
END