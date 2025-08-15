-- Fix Duplicate Floor Plans
-- This script identifies and removes duplicate floor plans

-- First, let's see what duplicates exist
SELECT 
    LocationId,
    FloorNumber,
    FloorName,
    COUNT(*) as DuplicateCount,
    STRING_AGG(CAST(Id as VARCHAR), ', ') as FloorPlanIds
FROM FloorPlans 
WHERE IsActive = 1
GROUP BY LocationId, FloorNumber, FloorName
HAVING COUNT(*) > 1
ORDER BY LocationId, FloorNumber;

-- For each duplicate group, keep the one with the most desks and delete the others
-- This will mark the duplicates as inactive (soft delete)

-- Example for Queens 3rd Floor duplicates:
-- Keep the one with the most desks, mark others as inactive

UPDATE FloorPlans 
SET IsActive = 0, UpdatedAt = GETUTCDATE()
WHERE Id IN (
    SELECT fp.Id
    FROM FloorPlans fp
    INNER JOIN (
        SELECT LocationId, FloorNumber, FloorName
        FROM FloorPlans 
        WHERE IsActive = 1
        GROUP BY LocationId, FloorNumber, FloorName
        HAVING COUNT(*) > 1
    ) duplicates ON fp.LocationId = duplicates.LocationId 
                   AND fp.FloorNumber = duplicates.FloorNumber 
                   AND fp.FloorName = duplicates.FloorName
    WHERE fp.Id NOT IN (
        SELECT TOP 1 fp2.Id
        FROM FloorPlans fp2
        INNER JOIN (
            SELECT LocationId, FloorNumber, FloorName
            FROM FloorPlans 
            WHERE IsActive = 1
            GROUP BY LocationId, FloorNumber, FloorName
            HAVING COUNT(*) > 1
        ) duplicates2 ON fp2.LocationId = duplicates2.LocationId 
                       AND fp2.FloorNumber = duplicates2.FloorNumber 
                       AND fp2.FloorName = duplicates2.FloorName
        ORDER BY (SELECT COUNT(*) FROM Desks WHERE FloorPlanId = fp2.Id AND IsActive = 1) DESC,
                 fp2.CreatedAt ASC
    )
);

-- Verify the fix
SELECT 
    fp.Id,
    l.Name as LocationName,
    fp.FloorNumber,
    fp.FloorName,
    fp.IsActive,
    (SELECT COUNT(*) FROM Desks WHERE FloorPlanId = fp.Id AND IsActive = 1) as DeskCount
FROM FloorPlans fp
INNER JOIN Locations l ON fp.LocationId = l.Id
WHERE l.Name LIKE '%Queens%' AND fp.FloorNumber = '3rd Floor'
ORDER BY fp.IsActive DESC, fp.CreatedAt;
