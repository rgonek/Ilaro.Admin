-- Drop all tables
--WHILE EXISTS(SELECT [name] FROM sys.tables WHERE [type] = 'U' AND [name] NOT LIKE '%sysdiagrams%')
IF DB_NAME() IN ('master', 'msdb', 'model', 'distribution')
BEGIN
    RAISERROR('Not for use on system databases', 16, 1)
    GOTO Done
END

SET NOCOUNT ON
DECLARE @DropStatement nvarchar(4000)
DECLARE @SequenceNumber int
DECLARE @LastError int
DECLARE DropStatements CURSOR LOCAL FAST_FORWARD READ_ONLY FOR

/************************************************************
All Foreign Keys
*************************************************************/
SELECT
    1 AS SequenceNumber,
    N'ALTER TABLE ' + QUOTENAME(TABLE_SCHEMA) + N'.' + QUOTENAME(TABLE_NAME) + N' DROP CONSTRAINT ' + CONSTRAINT_NAME AS DropStatement
FROM
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE
    CONSTRAINT_TYPE = N'FOREIGN KEY'
UNION ALL
/************************************************************
All Tables
*************************************************************/
SELECT
    2 AS SequenceNumber,
    N'DROP TABLE ' + QUOTENAME(TABLE_SCHEMA) + N'.' + QUOTENAME(TABLE_NAME) AS DropStatement
FROM
    INFORMATION_SCHEMA.TABLES
WHERE
    TABLE_TYPE = N'BASE TABLE' AND OBJECTPROPERTY(OBJECT_ID(QUOTENAME(TABLE_SCHEMA) + N'.' + QUOTENAME(TABLE_NAME)), 'IsMSShipped') = 0
    AND TABLE_NAME NOT LIKE 'sysdiagrams'

OPEN DropStatements
WHILE 1 = 1
BEGIN
    FETCH NEXT FROM DropStatements INTO @SequenceNumber, @DropStatement
    IF @@FETCH_STATUS = -1 BREAK
    BEGIN
        RAISERROR('%s', 0, 1, @DropStatement) WITH NOWAIT
        EXECUTE sp_ExecuteSQL @DropStatement
        SET @LastError = @@ERROR
        IF @LastError > 0
        BEGIN
            RAISERROR('Script terminated due to unexpected error', 16, 1)
            GOTO Done
        END
    END
END
CLOSE DropStatements
DEALLOCATE DropStatements

Done:
GO