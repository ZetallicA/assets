-- Fix Equipment Location Assignments
-- This script identifies and fixes equipment records that have inconsistent location data

-- First, let's see what's wrong with the data
SELECT 
    e.Id,
    e.OATH_Tag,
    e.CurrentLocationId,
    l.Name as CurrentLocationName,
    e.CurrentFloorPlanId,
    fp.FloorNumber,
    e.CurrentDeskId,
    d.DeskNumber
FROM Equipment e
LEFT JOIN Locations l ON e.CurrentLocationId = l.Id
LEFT JOIN FloorPlans fp ON e.CurrentFloorPlanId = fp.Id  
LEFT JOIN Desks d ON e.CurrentDeskId = d.Id
WHERE e.OATH_Tag IN ('1073', '2050')  -- The equipment we're testing
ORDER BY e.OATH_Tag;

-- Check if Queens location exists and get its ID
SELECT Id, Name FROM Locations WHERE Name LIKE '%Queens%' OR Name LIKE '%LIC%';

-- Check what floor plans exist for Queens
SELECT 
    fp.Id, 
    fp.FloorNumber, 
    fp.FloorName, 
    fp.IsActive,
    l.Name as LocationName
FROM FloorPlans fp
JOIN Locations l ON fp.LocationId = l.Id
WHERE l.Name LIKE '%Queens%' OR l.Name LIKE '%LIC%'
ORDER BY fp.FloorNumber;

-- Fix equipment that should be assigned to Queens but has NULL/0 location
-- First, get the correct Queens location ID
DECLARE @QueensLocationId INT = (SELECT TOP 1 Id FROM Locations WHERE Name LIKE '%Queens%' OR Name LIKE '%LIC%');

-- Update equipment records that show Queens in UI but have incorrect database values
UPDATE Equipment 
SET CurrentLocationId = @QueensLocationId
WHERE OATH_Tag IN ('1073', '2050') 
AND (CurrentLocationId IS NULL OR CurrentLocationId = 0);

-- Verify the fix
SELECT 
    e.Id,
    e.OATH_Tag,
    e.CurrentLocationId,
    l.Name as CurrentLocationName,
    e.CurrentFloorPlanId,
    e.CurrentDeskId
FROM Equipment e
LEFT JOIN Locations l ON e.CurrentLocationId = l.Id
WHERE e.OATH_Tag IN ('1073', '2050')
ORDER BY e.OATH_Tag;
