-- Fix duplicate floor plan issue
-- This script deactivates the duplicate Queens floor plan (ID: 142)

-- First, let's see what we have
SELECT 
    Id,
    LocationId,
    FloorNumber,
    FloorName,
    IsActive,
    CreatedAt
FROM FloorPlans 
WHERE LocationId = (SELECT Id FROM Locations WHERE Name = 'Queens' AND IsActive = 1)
ORDER BY Id;

-- Deactivate the duplicate floor plan (ID: 142)
UPDATE FloorPlans 
SET IsActive = 0, 
    UpdatedAt = GETUTCDATE()
WHERE Id = 142;

-- Verify the fix
SELECT 
    Id,
    LocationId,
    FloorNumber,
    FloorName,
    IsActive,
    CreatedAt
FROM FloorPlans 
WHERE LocationId = (SELECT Id FROM Locations WHERE Name = 'Queens' AND IsActive = 1)
ORDER BY Id;
