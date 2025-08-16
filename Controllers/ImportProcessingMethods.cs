using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AssetManagement.Controllers
{
    public partial class ImportController
    {
        private async Task<IActionResult> ProcessEquipmentRegistration(ExcelWorksheet worksheet, int locationId)
        {
            // Clear previous flagged records - this will be handled by the controller
            
            var imported = 0;
            var errors = new List<string>();
            
            // Create header mapping from user-friendly names to database fields
            var headerMapping = CreateHeaderMapping(worksheet);
            
            // Get the "Registered" status for new equipment
            var registeredStatus = await _context.AssetStatuses
                .FirstOrDefaultAsync(s => s.Name == "Registered");
            
            var location = await _context.Locations.FindAsync(locationId);

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                    var oathTag = GetCellValue(worksheet, row, headerMapping, "OATH Tag");
                    var serialNumber = GetCellValue(worksheet, row, headerMapping, "Serial Number");

                    if (string.IsNullOrEmpty(oathTag) || string.IsNullOrEmpty(serialNumber))
                        continue;

                    // Check if equipment already exists by OATH Tag
                    var existingEquipment = await _context.Equipment
                        .FirstOrDefaultAsync(e => e.OATH_Tag == oathTag);

                    if (existingEquipment != null)
                    {
                        var errorMsg = $"Row {row}: Equipment with OATH Tag '{oathTag}' already exists - skipping";
                        errors.Add(errorMsg);
                        
                        // Add to flagged records for review
                        var flaggedRecord = new FlaggedRecord
                        {
                            RowNumber = row,
                            OathTag = oathTag,
                            SerialNumber = serialNumber,
                            Model = GetCellValue(worksheet, row, headerMapping, "Model"),
                            Manufacturer = GetCellValue(worksheet, row, headerMapping, "Manufacturer"),
                            Category = GetCellValue(worksheet, row, headerMapping, "Category"),
                            Unit = GetCellValue(worksheet, row, headerMapping, "Unit"),
                            Issue = "Duplicate OATH Tag",
                            ErrorMessage = errorMsg,
                            ImportType = "equipment",
                            OriginalRowData = GetOriginalRowData(worksheet, row, headerMapping)
                        };
                        ImportController.AddFlaggedRecord(flaggedRecord);
                        continue;
                    }

                    // Check for duplicate serial number (warning, not error)
                    var duplicateSerial = await _context.Equipment
                        .FirstOrDefaultAsync(e => e.Serial_Number == serialNumber && !string.IsNullOrEmpty(serialNumber));
                    
                    if (duplicateSerial != null)
                    {
                        errors.Add($"Row {row}: Warning - Serial Number '{serialNumber}' already exists for OATH Tag '{duplicateSerial.OATH_Tag}'");
                    }

                    var equipment = new Equipment
                    {
                        OATH_Tag = oathTag,
                        Serial_Number = serialNumber,
                        Model = GetCellValue(worksheet, row, headerMapping, "Model"),
                        Manufacturer = GetCellValue(worksheet, row, headerMapping, "Manufacturer"),
                        CurrentStatusId = registeredStatus?.Id,
                        CurrentLocationId = locationId,
                        Notes = GetCellValue(worksheet, row, headerMapping, "Notes"),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    // Parse dates and costs using header mapping
                    if (DateTime.TryParse(GetCellValue(worksheet, row, headerMapping, "Purchase Date"), out DateTime purchaseDate))
                        equipment.Purchase_Date = purchaseDate;

                    if (decimal.TryParse(GetCellValue(worksheet, row, headerMapping, "Purchase Cost")?.Replace("$", ""), out decimal cost))
                        equipment.Purchase_Cost = cost;

                    if (DateTime.TryParse(GetCellValue(worksheet, row, headerMapping, "Warranty Expiry"), out DateTime warrantyExpiry))
                        equipment.Warranty_Expiry = warrantyExpiry;

                    // Find or create category
                    var categoryName = GetCellValue(worksheet, row, headerMapping, "Category");
                    if (!string.IsNullOrEmpty(categoryName))
                    {
                        var category = await _context.AssetCategories
                            .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
                        equipment.AssetCategoryId = category?.Id;
                    }

                    // Set Unit (Department)
                    var unitName = GetCellValue(worksheet, row, headerMapping, "Unit");
                    if (!string.IsNullOrEmpty(unitName))
                    {
                        equipment.Department = unitName;
                    }

                    _context.Equipment.Add(equipment);
                    imported++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            if (errors.Any())
            {
                TempData["ImportWarning"] = $"Imported {imported} items with {errors.Count} errors: {string.Join("; ", errors.Take(5))}";
            }
            else
            {
                TempData["ImportSuccess"] = $"Successfully imported {imported} equipment items for registration!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ProcessConfigurationImport(ExcelWorksheet worksheet)
        {
            var configured = 0;
            var errors = new List<string>();

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                    var oathTag = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(oathTag)) continue;

                    var equipment = await _context.Equipment
                        .Include(e => e.TechnologyConfiguration)
                        .FirstOrDefaultAsync(e => e.OATH_Tag == oathTag);

                    if (equipment == null)
                    {
                        errors.Add($"Row {row}: Equipment with OATH Tag '{oathTag}' not found");
                        continue;
                    }

                    // Create or update technology configuration
                    if (equipment.TechnologyConfiguration == null)
                    {
                        equipment.TechnologyConfiguration = new TechnologyConfiguration
                        {
                            EquipmentId = equipment.Id
                        };
                        _context.TechnologyConfigurations.Add(equipment.TechnologyConfiguration);
                    }

                    var config = equipment.TechnologyConfiguration;
                    equipment.Computer_Name = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                    equipment.IP_Address = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                    config.MACAddress = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                    config.WallPort = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                    config.SwitchName = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                    config.SwitchPort = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
                    equipment.Phone_Number = worksheet.Cells[row, 8].Value?.ToString()?.Trim();
                    config.Extension = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
                    config.ConfigurationNotes = worksheet.Cells[row, 13].Value?.ToString()?.Trim();
                    config.LastUpdated = DateTime.UtcNow;

                    // Update location and desk assignment
                    var locationName = worksheet.Cells[row, 10].Value?.ToString()?.Trim();
                    var floorNumber = worksheet.Cells[row, 11].Value?.ToString()?.Trim();
                    var deskNumber = worksheet.Cells[row, 12].Value?.ToString()?.Trim();

                    if (!string.IsNullOrEmpty(locationName) && !string.IsNullOrEmpty(floorNumber) && !string.IsNullOrEmpty(deskNumber))
                    {
                        var location = await _context.Locations
                            .FirstOrDefaultAsync(l => l.Name.Contains(locationName));
                        
                        if (location != null)
                        {
                            var floorPlan = await _context.FloorPlans
                                .FirstOrDefaultAsync(fp => fp.LocationId == location.Id && fp.FloorNumber == floorNumber);
                            
                            if (floorPlan != null)
                            {
                                var desk = await _context.Desks
                                    .FirstOrDefaultAsync(d => d.FloorPlanId == floorPlan.Id && d.DeskNumber == deskNumber);
                                
                                equipment.CurrentLocationId = location.Id;
                                equipment.CurrentFloorPlanId = floorPlan.Id;
                                equipment.CurrentDeskId = desk?.Id;
                            }
                        }
                    }

                    // Update status to "Configured"
                    var configuredStatus = await _context.AssetStatuses
                        .FirstOrDefaultAsync(s => s.Name == "Configured");
                    if (configuredStatus != null)
                    {
                        equipment.CurrentStatusId = configuredStatus.Id;
                    }

                    equipment.UpdatedAt = DateTime.UtcNow;
                    configured++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            if (errors.Any())
            {
                TempData["ImportWarning"] = $"Configured {configured} items with {errors.Count} errors: {string.Join("; ", errors.Take(5))}";
            }
            else
            {
                TempData["ImportSuccess"] = $"Successfully configured {configured} equipment items!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ProcessPeopleImport(ExcelWorksheet worksheet)
        {
            var imported = 0;
            var errors = new List<string>();

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                    var objectId = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                    var displayName = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                    var userPrincipalName = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(objectId) || string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(userPrincipalName))
                        continue;

                    // Check if user already exists
                    var existingUser = await _context.EntraUsers
                        .FirstOrDefaultAsync(u => u.ObjectId == objectId || u.UserPrincipalName == userPrincipalName);

                    if (existingUser != null)
                    {
                        // Update existing user
                        existingUser.DisplayName = displayName;
                        existingUser.Mail = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                        existingUser.Department = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                        existingUser.JobTitle = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                        existingUser.OfficeLocation = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
                        existingUser.EmployeeId = worksheet.Cells[row, 8].Value?.ToString()?.Trim();
                        existingUser.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create new user
                        var user = new EntraUser
                        {
                            ObjectId = objectId,
                            DisplayName = displayName,
                            UserPrincipalName = userPrincipalName,
                            Mail = worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                            Department = worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                            JobTitle = worksheet.Cells[row, 6].Value?.ToString()?.Trim(),
                            OfficeLocation = worksheet.Cells[row, 7].Value?.ToString()?.Trim(),
                            EmployeeId = worksheet.Cells[row, 8].Value?.ToString()?.Trim(),
                            AccountEnabled = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        // Also create a Person record for supervisor tracking
                        var person = new Person
                        {
                            FullName = displayName,
                            Email = user.Mail,
                            Phone = worksheet.Cells[row, 10].Value?.ToString()?.Trim(),
                            Supervisor = worksheet.Cells[row, 9].Value?.ToString()?.Trim(),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        _context.People.Add(person);

                        _context.EntraUsers.Add(user);
                    }

                    imported++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            if (errors.Any())
            {
                TempData["ImportWarning"] = $"Imported {imported} people with {errors.Count} errors: {string.Join("; ", errors.Take(5))}";
            }
            else
            {
                TempData["ImportSuccess"] = $"Successfully imported {imported} people!";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ProcessFullInventoryImport(ExcelWorksheet worksheet, int locationId)
        {
            // Clear previous flagged records - this will be handled by the controller
            
            var imported = 0;
            var updated = 0;
            var errors = new List<string>();
            var warnings = new List<string>();
            var batchSize = 10; // Process in smaller batches
            
            // Create header mapping from user-friendly names to database fields
            var headerMapping = CreateHeaderMapping(worksheet);

            // Pre-check for duplicates within the Excel file
            var excelOathTags = new HashSet<string>();
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var oathTag = GetCellValue(worksheet, row, headerMapping, "OATH Tag");
                if (!string.IsNullOrEmpty(oathTag))
                {
                    if (!excelOathTags.Add(oathTag))
                    {
                        var errorMsg = $"Row {row}: Duplicate OATH Tag '{oathTag}' found within Excel file";
                        errors.Add(errorMsg);
                        
                        // Add to flagged records for review
                        var flaggedRecord = new FlaggedRecord
                        {
                            RowNumber = row,
                            OathTag = oathTag,
                            SerialNumber = GetCellValue(worksheet, row, headerMapping, "Serial Number"),
                            Model = GetCellValue(worksheet, row, headerMapping, "Model"),
                            Manufacturer = GetCellValue(worksheet, row, headerMapping, "Manufacturer"),
                            Category = GetCellValue(worksheet, row, headerMapping, "Category"),
                            Status = GetCellValue(worksheet, row, headerMapping, "Status"),
                            Unit = GetCellValue(worksheet, row, headerMapping, "Unit"),
                            Issue = "Duplicate OATH Tag in Excel",
                            ErrorMessage = errorMsg,
                            ImportType = "inventory",
                            OriginalRowData = GetOriginalRowData(worksheet, row, headerMapping)
                        };
                        ImportController.AddFlaggedRecord(flaggedRecord);
                    }
                }
            }

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                    var oathTag = GetCellValue(worksheet, row, headerMapping, "OATH Tag");
                    var serialNumber = GetCellValue(worksheet, row, headerMapping, "Serial Number");

                    if (string.IsNullOrEmpty(oathTag) || string.IsNullOrEmpty(serialNumber))
                        continue;

                    // Check if equipment exists, create or update
                    var equipment = await _context.Equipment
                        .Include(e => e.TechnologyConfiguration)
                        .FirstOrDefaultAsync(e => e.OATH_Tag == oathTag);

                    bool isNewEquipment = false;
                    if (equipment == null)
                    {
                        equipment = new Equipment
                        {
                            OATH_Tag = oathTag,
                            Serial_Number = serialNumber,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                        _context.Equipment.Add(equipment);
                        isNewEquipment = true;
                    }
                    else
                    {
                        // Update existing equipment's serial number if provided
                        if (!string.IsNullOrEmpty(serialNumber))
                        {
                            equipment.Serial_Number = serialNumber;
                        }
                        updated++;
                        warnings.Add($"Row {row}: Updated existing equipment with OATH Tag '{oathTag}'");
                    }

                    // Update basic equipment info
                    equipment.Model = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                    equipment.Manufacturer = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                    equipment.Department = worksheet.Cells[row, 7].Value?.ToString()?.Trim(); // Unit field
                    equipment.Notes = worksheet.Cells[row, 24].Value?.ToString()?.Trim(); // Notes moved to column 24
                    equipment.UpdatedAt = DateTime.UtcNow;

                    // Parse dates and costs
                    if (DateTime.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out DateTime purchaseDate))
                        equipment.Purchase_Date = purchaseDate;

                    if (decimal.TryParse(worksheet.Cells[row, 9].Value?.ToString()?.Replace("$", ""), out decimal cost))
                        equipment.Purchase_Cost = cost;

                    if (DateTime.TryParse(worksheet.Cells[row, 10].Value?.ToString(), out DateTime warrantyExpiry))
                        equipment.Warranty_Expiry = warrantyExpiry;

                    // Handle category, status, and assignments
                    await HandleCategoryAndStatus(equipment, worksheet, row);
                    await HandleUserAssignment(equipment, worksheet, row);
                    await HandleLocationAssignment(equipment, worksheet, row, locationId);
                    await HandleTechnologyConfiguration(equipment, worksheet, row);

                    imported++;

                    // Save in batches to avoid large transactions
                    if ((imported + updated) % batchSize == 0)
                    {
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Batch save error at row {row}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row}: {ex.Message}");
                }
            }

            // Save remaining items
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Final save error: {ex.Message}");
            }

            // Build comprehensive result message
            var resultMessage = new List<string>();
            
            if (imported > 0) 
                resultMessage.Add($"✅ {imported} new equipment added");
            if (updated > 0) 
                resultMessage.Add($"🔄 {updated} existing equipment updated");
            if (warnings.Count > 0) 
                resultMessage.Add($"⚠️ {warnings.Count} warnings");
            if (errors.Count > 0) 
                resultMessage.Add($"❌ {errors.Count} errors");

            if (errors.Any())
            {
                var errorSummary = string.Join(", ", resultMessage);
                var errorDetails = string.Join("; ", errors.Take(10)); // Show first 10 errors
                TempData["ImportWarning"] = $"{errorSummary}. Errors: {errorDetails}";
                
                if (errors.Count > 10)
                {
                    TempData["ImportError"] = $"... and {errors.Count - 10} more errors. Please review your data.";
                }
            }
            else
            {
                var successSummary = string.Join(", ", resultMessage);
                TempData["ImportSuccess"] = $"Import completed! {successSummary}";
                
                if (warnings.Count > 0)
                {
                    var warningDetails = string.Join("; ", warnings.Take(5));
                    TempData["ImportInfo"] = $"Warnings: {warningDetails}";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task HandleCategoryAndStatus(Equipment equipment, ExcelWorksheet worksheet, int row)
        {
            // Handle category
            var categoryName = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = await _context.AssetCategories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
                equipment.AssetCategoryId = category?.Id;
            }

            // Handle status
            var statusName = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(statusName))
            {
                var status = await _context.AssetStatuses
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == statusName.ToLower());
                equipment.CurrentStatusId = status?.Id;
            }
        }

        private async Task HandleUserAssignment(Equipment equipment, ExcelWorksheet worksheet, int row)
        {
            var headerMapping = CreateHeaderMapping(worksheet);
            
            var userEmail = GetCellValue(worksheet, row, headerMapping, "Assigned User Email");
            var userName = GetCellValue(worksheet, row, headerMapping, "Assigned User Name");
            
            // If we have a user name (even without email), process the assignment
            if (!string.IsNullOrEmpty(userName))
            {
                // If we have an email, try to find the user in Entra
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _context.EntraUsers
                        .FirstOrDefaultAsync(u => u.Mail == userEmail || u.UserPrincipalName == userEmail);
                    
                    if (user != null)
                    {
                        // User found in Entra - use Entra data
                        equipment.AssignedEntraUserId = user.Id;
                        equipment.Assigned_User_Email = user.Mail;
                        equipment.Assigned_User_Name = user.DisplayName;
                    }
                    else
                    {
                        // User not found in Entra - use Excel data directly
                        equipment.Assigned_User_Email = userEmail;
                        equipment.Assigned_User_Name = userName;
                    }
                }
                else
                {
                    // No email provided - use Excel name with "No email on file"
                    equipment.Assigned_User_Email = "No email on file";
                    equipment.Assigned_User_Name = userName;
                }
            }
        }

        private async Task HandleLocationAssignment(Equipment equipment, ExcelWorksheet worksheet, int row, int defaultLocationId)
        {
            var headerMapping = CreateHeaderMapping(worksheet);
            
            var locationName = GetCellValue(worksheet, row, headerMapping, "Location");
            var floorNumber = GetCellValue(worksheet, row, headerMapping, "Floor");
            var deskNumber = GetCellValue(worksheet, row, headerMapping, "Desk Number");

            Location? location = null;

            // PRIORITIZE FORM SELECTION: Use the default location from form first
            if (defaultLocationId > 0)
            {
                location = await _context.Locations.FindAsync(defaultLocationId);
            }
            
            // Only use Excel location if no form selection was made
            if (location == null && !string.IsNullOrEmpty(locationName))
            {
                location = await _context.Locations
                    .FirstOrDefaultAsync(l => l.Name.Contains(locationName));
            }

            if (location != null)
            {
                equipment.CurrentLocationId = location.Id;

                if (!string.IsNullOrEmpty(floorNumber))
                {
                    var floorPlan = await _context.FloorPlans
                        .FirstOrDefaultAsync(fp => fp.LocationId == location.Id && fp.FloorNumber == floorNumber);
                    
                    if (floorPlan != null)
                    {
                        equipment.CurrentFloorPlanId = floorPlan.Id;

                        if (!string.IsNullOrEmpty(deskNumber))
                        {
                            var desk = await _context.Desks
                                .FirstOrDefaultAsync(d => d.FloorPlanId == floorPlan.Id && d.DeskNumber == deskNumber);
                            equipment.CurrentDeskId = desk?.Id;
                        }
                    }
                }
            }
        }

        private Task HandleTechnologyConfiguration(Equipment equipment, ExcelWorksheet worksheet, int row)
        {
            var netName = worksheet.Cells[row, 16].Value?.ToString()?.Trim();
            var ipv4 = worksheet.Cells[row, 17].Value?.ToString()?.Trim();
            var mac = worksheet.Cells[row, 18].Value?.ToString()?.Trim();

            if (!string.IsNullOrEmpty(netName) || !string.IsNullOrEmpty(ipv4) || !string.IsNullOrEmpty(mac))
            {
                if (equipment.TechnologyConfiguration == null)
                {
                    equipment.TechnologyConfiguration = new TechnologyConfiguration
                    {
                        EquipmentId = equipment.Id
                    };
                    _context.TechnologyConfigurations.Add(equipment.TechnologyConfiguration);
                }

                var config = equipment.TechnologyConfiguration;
                equipment.Computer_Name = netName;  // Computer_Name maps to what was "Net Name"
                equipment.IP_Address = ipv4;       // IP_Address maps to IPv4
                config.MACAddress = mac;
                config.WallPort = worksheet.Cells[row, 19].Value?.ToString()?.Trim();
                config.SwitchName = worksheet.Cells[row, 20].Value?.ToString()?.Trim();
                config.SwitchPort = worksheet.Cells[row, 21].Value?.ToString()?.Trim();
                equipment.Phone_Number = worksheet.Cells[row, 22].Value?.ToString()?.Trim();
                config.ConfigurationNotes = worksheet.Cells[row, 23].Value?.ToString()?.Trim();
                config.LastUpdated = DateTime.UtcNow;
            }
            
            return Task.CompletedTask;
        }

        private Dictionary<string, int> CreateHeaderMapping(ExcelWorksheet worksheet)
        {
            var mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            
            // Map user-friendly headers to column positions
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(header))
                {
                    // Normalize headers to handle variations
                    var normalizedHeader = header.Replace("*", "").Replace("(", "").Replace(")", "").Trim();
                    mapping[normalizedHeader] = col;
                    
                    // Add specific mappings for common variations
                    if (normalizedHeader.Equals("OATH Tag", StringComparison.OrdinalIgnoreCase))
                        mapping["OATHTag"] = col;
                    if (normalizedHeader.Equals("Serial Number", StringComparison.OrdinalIgnoreCase))
                        mapping["SerialNumber"] = col;
                    if (normalizedHeader.Equals("Net Name Hostname", StringComparison.OrdinalIgnoreCase))
                        mapping["NetName"] = col;
                    if (normalizedHeader.Equals("IP Address", StringComparison.OrdinalIgnoreCase))
                        mapping["IPAddress"] = col;
                    if (normalizedHeader.Equals("Phone Number", StringComparison.OrdinalIgnoreCase))
                        mapping["PhoneNumber"] = col;
                    if (normalizedHeader.Equals("Purchase Date", StringComparison.OrdinalIgnoreCase))
                        mapping["PurchaseDate"] = col;
                    if (normalizedHeader.Equals("Purchase Cost", StringComparison.OrdinalIgnoreCase))
                        mapping["PurchaseCost"] = col;
                    if (normalizedHeader.Equals("Warranty Expiry", StringComparison.OrdinalIgnoreCase))
                        mapping["WarrantyExpiry"] = col;
                    if (normalizedHeader.Equals("Assigned User Email", StringComparison.OrdinalIgnoreCase))
                        mapping["AssignedUserEmail"] = col;
                    if (normalizedHeader.Equals("Assigned User Name", StringComparison.OrdinalIgnoreCase))
                        mapping["AssignedUserName"] = col;
                    if (normalizedHeader.Equals("IMEI", StringComparison.OrdinalIgnoreCase))
                        mapping["IMEI"] = col;
                    if (normalizedHeader.Equals("SIM Card Number", StringComparison.OrdinalIgnoreCase))
                        mapping["SIMCardNumber"] = col;
                    if (normalizedHeader.Equals("Unit", StringComparison.OrdinalIgnoreCase))
                        mapping["Unit"] = col;
                }
            }
            
            return mapping;
        }

        private string GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMapping, string fieldName)
        {
            if (headerMapping.TryGetValue(fieldName, out int col))
            {
                return worksheet.Cells[row, col].Value?.ToString()?.Trim() ?? "";
            }
            return "";
        }

        private Dictionary<string, object> GetOriginalRowData(ExcelWorksheet worksheet, int row, Dictionary<string, int> headerMapping)
        {
            var rowData = new Dictionary<string, object>();
            
            // Get all column headers and their values
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(header))
                {
                    var value = worksheet.Cells[row, col].Value?.ToString()?.Trim() ?? "";
                    rowData[header] = value;
                }
            }
            
            return rowData;
        }
    }
}
