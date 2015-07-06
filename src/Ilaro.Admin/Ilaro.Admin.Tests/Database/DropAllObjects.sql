USE master
GO
if exists (select * from sysdatabases where name='IlaroTestDb')
		drop database IlaroTestDb
GO