-- SQL Script to rename confusing database columns to match user-friendly template headers
-- Run this to make database columns more intuitive

USE [AssetManagement];
GO

BEGIN TRANSACTION;

-- Rename Computer_Name to NetName (more intuitive for hostnames)
EXEC sp_rename 'dbo.Equipment.Computer_Name', 'NetName', 'COLUMN';

-- Rename IP_Address to IPAddress (remove underscore for consistency)
EXEC sp_rename 'dbo.Equipment.IP_Address', 'IPAddress', 'COLUMN';

-- Rename Phone_Number to PhoneNumber (remove underscore for consistency)
EXEC sp_rename 'dbo.Equipment.Phone_Number', 'PhoneNumber', 'COLUMN';

-- Rename Assigned_User_Name to AssignedUserName (remove underscores)
EXEC sp_rename 'dbo.Equipment.Assigned_User_Name', 'AssignedUserName', 'COLUMN';

-- Rename Assigned_User_Email to AssignedUserEmail (remove underscores)
EXEC sp_rename 'dbo.Equipment.Assigned_User_Email', 'AssignedUserEmail', 'COLUMN';

-- Rename Assigned_User_ID to AssignedUserID (remove underscores)
EXEC sp_rename 'dbo.Equipment.Assigned_User_ID', 'AssignedUserID', 'COLUMN';

-- Rename Serial_Number to SerialNumber (remove underscore)
EXEC sp_rename 'dbo.Equipment.Serial_Number', 'SerialNumber', 'COLUMN';

-- Rename Asset_Tag to AssetTag (remove underscore)
EXEC sp_rename 'dbo.Equipment.Asset_Tag', 'AssetTag', 'COLUMN';

-- Rename OATH_Tag to OATHTag (remove underscore)
EXEC sp_rename 'dbo.Equipment.OATH_Tag', 'OATHTag', 'COLUMN';

-- Rename Purchase_Cost to PurchaseCost (remove underscore)
EXEC sp_rename 'dbo.Equipment.Purchase_Cost', 'PurchaseCost', 'COLUMN';

-- Rename Purchase_Date to PurchaseDate (remove underscore)
EXEC sp_rename 'dbo.Equipment.Purchase_Date', 'PurchaseDate', 'COLUMN';

-- Rename Warranty_Expiry to WarrantyExpiry (remove underscore)
EXEC sp_rename 'dbo.Equipment.Warranty_Expiry', 'WarrantyExpiry', 'COLUMN';

-- Rename Current_Location_Notes to CurrentLocationNotes (remove underscores)
EXEC sp_rename 'dbo.Equipment.Current_Location_Notes', 'CurrentLocationNotes', 'COLUMN';

-- Rename Current_Value to CurrentValue (remove underscore)
EXEC sp_rename 'dbo.Equipment.Current_Value', 'CurrentValue', 'COLUMN';

-- Rename Last_Maintenance_Date to LastMaintenanceDate (remove underscores)
EXEC sp_rename 'dbo.Equipment.Last_Maintenance_Date', 'LastMaintenanceDate2', 'COLUMN';

-- Rename OS_Version to OSVersion (remove underscore)
EXEC sp_rename 'dbo.Equipment.OS_Version', 'OSVersion', 'COLUMN';

-- Verify changes
SELECT TOP 5 
    OATHTag, SerialNumber, NetName, IPAddress, PhoneNumber,
    AssignedUserName, AssignedUserEmail
FROM Equipment;

COMMIT TRANSACTION;

PRINT 'Database schema updated successfully!';
PRINT 'NetName now stores hostnames/computer names';
PRINT 'All underscores removed for consistency';
