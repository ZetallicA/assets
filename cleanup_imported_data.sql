-- Cleanup script for imported data
-- Run this to clear all imported equipment data

BEGIN TRANSACTION;

-- Delete Technology Configurations first (foreign key dependency)
DELETE FROM [AssetManagement].[dbo].[TechnologyConfigurations];

-- Delete Equipment records
DELETE FROM [AssetManagement].[dbo].[Equipment];

-- Delete People records (if any were imported)
DELETE FROM [AssetManagement].[dbo].[People];

-- Delete EntraUsers (if any were imported via People template)
DELETE FROM [AssetManagement].[dbo].[EntraUsers];

-- Reset identity columns (optional - starts IDs back at 1)
DBCC CHECKIDENT ('[AssetManagement].[dbo].[Equipment]', RESEED, 0);
DBCC CHECKIDENT ('[AssetManagement].[dbo].[TechnologyConfigurations]', RESEED, 0);
DBCC CHECKIDENT ('[AssetManagement].[dbo].[People]', RESEED, 0);
DBCC CHECKIDENT ('[AssetManagement].[dbo].[EntraUsers]', RESEED, 0);

-- Verify cleanup
SELECT 'Equipment' as TableName, COUNT(*) as RecordCount FROM [AssetManagement].[dbo].[Equipment]
UNION ALL
SELECT 'TechnologyConfigurations', COUNT(*) FROM [AssetManagement].[dbo].[TechnologyConfigurations]
UNION ALL
SELECT 'People', COUNT(*) FROM [AssetManagement].[dbo].[People]
UNION ALL
SELECT 'EntraUsers', COUNT(*) FROM [AssetManagement].[dbo].[EntraUsers];

COMMIT TRANSACTION;
