-- Add Storage floor plans to all existing locations
-- Run this script to add Storage floors for equipment storage areas

-- Add Storage floor for Manhattan location (66 John St)
INSERT INTO [AssetManagement].[dbo].[FloorPlans] (LocationId, FloorNumber, FloorName, Description, IsActive, CreatedAt)
SELECT 
    Id, 
    'Storage', 
    'Storage Area', 
    'Equipment storage and inventory area', 
    1, 
    GETUTCDATE()
FROM [AssetManagement].[dbo].[Locations] 
WHERE Name LIKE '%Manhattan%' AND Name LIKE '%66 John%'
AND NOT EXISTS (SELECT 1 FROM [AssetManagement].[dbo].[FloorPlans] WHERE LocationId = [Locations].Id AND FloorNumber = 'Storage');

-- Add Storage floor for Brooklyn location (9 Bond St)
INSERT INTO [AssetManagement].[dbo].[FloorPlans] (LocationId, FloorNumber, FloorName, Description, IsActive, CreatedAt)
SELECT 
    Id, 
    'Storage', 
    'Storage Area', 
    'Equipment storage and inventory area', 
    1, 
    GETUTCDATE()
FROM [AssetManagement].[dbo].[Locations] 
WHERE Name LIKE '%Brooklyn%' AND Name LIKE '%9 Bond%'
AND NOT EXISTS (SELECT 1 FROM [AssetManagement].[dbo].[FloorPlans] WHERE LocationId = [Locations].Id AND FloorNumber = 'Storage');

-- Add Storage floor for Queens location (LIC)
INSERT INTO [AssetManagement].[dbo].[FloorPlans] (LocationId, FloorNumber, FloorName, Description, IsActive, CreatedAt)
SELECT 
    Id, 
    'Storage', 
    'Storage Area', 
    'Equipment storage and inventory area', 
    1, 
    GETUTCDATE()
FROM [AssetManagement].[dbo].[Locations] 
WHERE Name LIKE '%LIC%'
AND NOT EXISTS (SELECT 1 FROM [AssetManagement].[dbo].[FloorPlans] WHERE LocationId = [Locations].Id AND FloorNumber = 'Storage');

-- Add Storage floor for Bronx location
INSERT INTO [AssetManagement].[dbo].[FloorPlans] (LocationId, FloorNumber, FloorName, Description, IsActive, CreatedAt)
SELECT 
    Id, 
    'Storage', 
    'Storage Area', 
    'Equipment storage and inventory area', 
    1, 
    GETUTCDATE()
FROM [AssetManagement].[dbo].[Locations] 
WHERE Name LIKE '%Bronx%'
AND NOT EXISTS (SELECT 1 FROM [AssetManagement].[dbo].[FloorPlans] WHERE LocationId = [Locations].Id AND FloorNumber = 'Storage');

-- Add Storage floor for Staten Island location
INSERT INTO [AssetManagement].[dbo].[FloorPlans] (LocationId, FloorNumber, FloorName, Description, IsActive, CreatedAt)
SELECT 
    Id, 
    'Storage', 
    'Storage Area', 
    'Equipment storage and inventory area', 
    1, 
    GETUTCDATE()
FROM [AssetManagement].[dbo].[Locations] 
WHERE Name LIKE '%Staten Island%'
AND NOT EXISTS (SELECT 1 FROM [AssetManagement].[dbo].[FloorPlans] WHERE LocationId = [Locations].Id AND FloorNumber = 'Storage');

-- Verify the results
SELECT 
    l.Name as LocationName,
    fp.FloorNumber,
    fp.FloorName,
    fp.Description
FROM [AssetManagement].[dbo].[FloorPlans] fp
JOIN [AssetManagement].[dbo].[Locations] l ON fp.LocationId = l.Id
WHERE fp.FloorNumber = 'Storage'
ORDER BY l.Name;
