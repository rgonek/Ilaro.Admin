IF (exists (select * from sysdatabases where name='IlaroTestDb'))
BEGIN
	USE [IlaroTestDb]
	-- disable referential integrity
	EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL' 
	 
	EXEC sp_MSForEachTable 'DELETE FROM ?' 

	-- enable referential integrity again 
	EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL' 
	
END