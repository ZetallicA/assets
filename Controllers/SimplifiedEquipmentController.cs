using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using OfficeOpenXml;
using System.Text.Json;

namespace AssetManagement.Controllers
{
    public class SimplifiedEquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SimplifiedEquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SimplifiedEquipment
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber, int? pageSize)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPageSize"] = pageSize ?? 20;

            var equipment = _context.SimplifiedEquipment.Where(e => e.IsActive).AsQueryable();

            // Apply search filter
            if (!String.IsNullOrEmpty(searchString))
            {
                equipment = equipment.Where(e => 
                    (e.Asset_Tag != null && e.Asset_Tag.Contains(searchString)) ||
                    (e.Serial_Number != null && e.Serial_Number.Contains(searchString)) ||
                    (e.Service_Tag != null && e.Service_Tag.Contains(searchString)) ||
                    (e.Manufacturer != null && e.Manufacturer.Contains(searchString)) ||
                    (e.Model != null && e.Model.Contains(searchString)) ||
                    (e.Category != null && e.Category.Contains(searchString)) ||
                    (e.Net_Name != null && e.Net_Name.Contains(searchString)) ||
                    (e.Assigned_User_Name != null && e.Assigned_User_Name.Contains(searchString)) ||
                    (e.Assigned_User_Email != null && e.Assigned_User_Email.Contains(searchString)) ||
                    (e.Department != null && e.Department.Contains(searchString)) ||
                    (e.Unit != null && e.Unit.Contains(searchString)) ||
                    (e.Location != null && e.Location.Contains(searchString)) ||
                    (e.Floor != null && e.Floor.Contains(searchString)) ||
                    (e.Desk != null && e.Desk.Contains(searchString)) ||
                    (e.Status != null && e.Status.Contains(searchString)) ||
                    (e.IP_Address != null && e.IP_Address.Contains(searchString)) ||
                    (e.MAC_Address != null && e.MAC_Address.Contains(searchString)) ||
                    (e.Phone_Number != null && e.Phone_Number.Contains(searchString)) ||
                    (e.Notes != null && e.Notes.Contains(searchString))
                );
            }

            // Apply sorting
            equipment = sortOrder switch
            {
                "asset_tag_desc" => equipment.OrderByDescending(e => e.Asset_Tag),
                "serial_number" => equipment.OrderBy(e => e.Serial_Number),
                "serial_number_desc" => equipment.OrderByDescending(e => e.Serial_Number),
                "manufacturer" => equipment.OrderBy(e => e.Manufacturer),
                "manufacturer_desc" => equipment.OrderByDescending(e => e.Manufacturer),
                "model" => equipment.OrderBy(e => e.Model),
                "model_desc" => equipment.OrderByDescending(e => e.Model),
                "category" => equipment.OrderBy(e => e.Category),
                "category_desc" => equipment.OrderByDescending(e => e.Category),
                "assigned_user" => equipment.OrderBy(e => e.Assigned_User_Name),
                "assigned_user_desc" => equipment.OrderByDescending(e => e.Assigned_User_Name),
                "department" => equipment.OrderBy(e => e.Department),
                "department_desc" => equipment.OrderByDescending(e => e.Department),
                "location" => equipment.OrderBy(e => e.Location),
                "location_desc" => equipment.OrderByDescending(e => e.Location),
                "floor" => equipment.OrderBy(e => e.Floor),
                "floor_desc" => equipment.OrderByDescending(e => e.Floor),
                "status" => equipment.OrderBy(e => e.Status),
                "status_desc" => equipment.OrderByDescending(e => e.Status),
                "net_name" => equipment.OrderBy(e => e.Net_Name),
                "net_name_desc" => equipment.OrderByDescending(e => e.Net_Name),
                "created_at" => equipment.OrderBy(e => e.CreatedAt),
                "created_at_desc" => equipment.OrderByDescending(e => e.CreatedAt),
                _ => equipment.OrderBy(e => e.Asset_Tag)
            };

            // Get unique values for filters (only from active records)
            ViewData["CategoryOptions"] = await _context.SimplifiedEquipment.Where(e => e.IsActive).Select(e => e.Category).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c).ToListAsync();
            ViewData["StatusOptions"] = await _context.SimplifiedEquipment.Where(e => e.IsActive).Select(e => e.Status).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToListAsync();
            ViewData["DepartmentOptions"] = await _context.SimplifiedEquipment.Where(e => e.IsActive).Select(e => e.Department).Where(d => !string.IsNullOrEmpty(d)).Distinct().OrderBy(d => d).ToListAsync();
            ViewData["LocationOptions"] = await _context.SimplifiedEquipment.Where(e => e.IsActive).Select(e => e.Location).Where(l => !string.IsNullOrEmpty(l)).Distinct().OrderBy(l => l).ToListAsync();
            ViewData["FloorOptions"] = await _context.SimplifiedEquipment.Where(e => e.IsActive).Select(e => e.Floor).Where(f => !string.IsNullOrEmpty(f)).Distinct().OrderBy(f => f).ToListAsync();
            ViewData["ManufacturerOptions"] = await _context.SimplifiedEquipment.Where(e => e.IsActive).Select(e => e.Manufacturer).Where(m => !string.IsNullOrEmpty(m)).Distinct().OrderBy(m => m).ToListAsync();

            // Pagination
            int pageSizeValue = pageSize ?? 20;
            int pageNumberValue = pageNumber ?? 1;

            var totalCount = await equipment.CountAsync();
            var items = await equipment
                .Skip((pageNumberValue - 1) * pageSizeValue)
                .Take(pageSizeValue)
                .ToListAsync();

            var paginatedList = new PaginatedList<SimplifiedEquipment>(items, totalCount, pageNumberValue, pageSizeValue);

            return View(paginatedList);
        }

        // GET: SimplifiedEquipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.SimplifiedEquipment
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: SimplifiedEquipment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SimplifiedEquipment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Asset_Tag,Serial_Number,Service_Tag,Manufacturer,Model,Category,Net_Name,Assigned_User_Name,Assigned_User_Email,Department,Unit,Location,Floor,Desk,Status,IP_Address,MAC_Address,Wall_Port,Switch_Name,Switch_Port,Phone_Number,Extension,IMEI,SIM_Card_Number,OS_Version,License1,License2,License3,License4,License5,Purchase_Price,Purchase_Order_Number,Vendor,Vendor_Info,Purchase_Date,Warranty_Start_Date,Warranty_End_Date,Notes")] SimplifiedEquipment equipment)
        {
            if (ModelState.IsValid)
            {
                // Check if equipment with this Asset Tag already exists (including inactive)
                var existingEquipment = await _context.SimplifiedEquipment
                    .FirstOrDefaultAsync(e => e.Asset_Tag == equipment.Asset_Tag);

                if (existingEquipment != null)
                {
                    if (existingEquipment.IsActive)
                    {
                        // Active record exists - show error
                        ModelState.AddModelError("Asset_Tag", $"Equipment with Asset Tag '{equipment.Asset_Tag}' already exists.");
                        return View(equipment);
                    }
                    else
                    {
                        // Inactive record exists - reactivate it and update with new data
                        existingEquipment.IsActive = true;
                        existingEquipment.UpdatedAt = DateTime.UtcNow;
                        existingEquipment.UpdatedBy = User.Identity?.Name ?? "System";
                        
                        // Update all fields with new data
                        existingEquipment.Serial_Number = equipment.Serial_Number;
                        existingEquipment.Service_Tag = equipment.Service_Tag;
                        existingEquipment.Manufacturer = equipment.Manufacturer;
                        existingEquipment.Model = equipment.Model;
                        existingEquipment.Category = equipment.Category;
                        existingEquipment.Net_Name = equipment.Net_Name;
                        existingEquipment.Assigned_User_Name = equipment.Assigned_User_Name;
                        existingEquipment.Assigned_User_Email = equipment.Assigned_User_Email;
                        existingEquipment.Department = equipment.Department;
                        existingEquipment.Unit = equipment.Unit;
                        existingEquipment.Location = equipment.Location;
                        existingEquipment.Floor = equipment.Floor;
                        existingEquipment.Desk = equipment.Desk;
                        existingEquipment.Status = equipment.Status;
                        existingEquipment.IP_Address = equipment.IP_Address;
                        existingEquipment.MAC_Address = equipment.MAC_Address;
                        existingEquipment.Wall_Port = equipment.Wall_Port;
                        existingEquipment.Switch_Name = equipment.Switch_Name;
                        existingEquipment.Switch_Port = equipment.Switch_Port;
                        existingEquipment.Phone_Number = equipment.Phone_Number;
                        existingEquipment.Extension = equipment.Extension;
                        existingEquipment.IMEI = equipment.IMEI;
                        existingEquipment.SIM_Card_Number = equipment.SIM_Card_Number;
                        existingEquipment.OS_Version = equipment.OS_Version;
                        existingEquipment.License1 = equipment.License1;
                        existingEquipment.License2 = equipment.License2;
                        existingEquipment.License3 = equipment.License3;
                        existingEquipment.License4 = equipment.License4;
                        existingEquipment.License5 = equipment.License5;
                        existingEquipment.Purchase_Price = equipment.Purchase_Price;
                        existingEquipment.Purchase_Order_Number = equipment.Purchase_Order_Number;
                        existingEquipment.Vendor = equipment.Vendor;
                        existingEquipment.Vendor_Info = equipment.Vendor_Info;
                        existingEquipment.Purchase_Date = equipment.Purchase_Date;
                        existingEquipment.Warranty_Start_Date = equipment.Warranty_Start_Date;
                        existingEquipment.Warranty_End_Date = equipment.Warranty_End_Date;
                        existingEquipment.Notes = equipment.Notes;

                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }

                // No existing record - create new one
                equipment.CreatedAt = DateTime.UtcNow;
                equipment.CreatedBy = User.Identity?.Name ?? "System";
                equipment.IsActive = true;

                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipment);
        }

        // GET: SimplifiedEquipment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.SimplifiedEquipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            return View(equipment);
        }

        // POST: SimplifiedEquipment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Asset_Tag,Serial_Number,Service_Tag,Manufacturer,Model,Category,Net_Name,Assigned_User_Name,Assigned_User_Email,Department,Unit,Location,Floor,Desk,Status,IP_Address,MAC_Address,Wall_Port,Switch_Name,Switch_Port,Phone_Number,Extension,IMEI,SIM_Card_Number,OS_Version,License1,License2,License3,License4,License5,Purchase_Price,Purchase_Order_Number,Vendor,Vendor_Info,Purchase_Date,Warranty_Start_Date,Warranty_End_Date,Notes,CreatedAt,CreatedBy,IsActive,IsCheckedOut,CheckedOutDate,CheckedOutBy,ExpectedReturnDate,QRCode,Barcode")] SimplifiedEquipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    equipment.UpdatedAt = DateTime.UtcNow;
                    equipment.UpdatedBy = User.Identity?.Name ?? "System";
                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SimplifiedEquipmentExists(equipment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(equipment);
        }

        // GET: SimplifiedEquipment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.SimplifiedEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // POST: SimplifiedEquipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.SimplifiedEquipment.FindAsync(id);
            if (equipment != null)
            {
                equipment.IsActive = false;
                equipment.UpdatedAt = DateTime.UtcNow;
                equipment.UpdatedBy = User.Identity?.Name ?? "System";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: SimplifiedEquipment/UpdateField
        [HttpPost]
        public async Task<IActionResult> UpdateField([FromBody] InlineEditRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.EquipmentId))
                {
                    return Json(new InlineEditResponse { Success = false, Message = "Invalid request" });
                }

                // Find equipment by ID
                SimplifiedEquipment? equipment = null;
                if (int.TryParse(request.EquipmentId, out int equipmentId))
                {
                    equipment = await _context.SimplifiedEquipment.FirstOrDefaultAsync(e => e.Id == equipmentId);
                }

                if (equipment == null)
                {
                    return Json(new InlineEditResponse { Success = false, Message = "Equipment not found" });
                }

                // Update the field using reflection
                var property = typeof(SimplifiedEquipment).GetProperty(request.FieldName ?? "");
                if (property != null && property.CanWrite)
                {
                    var value = request.Value?.ToString();
                    
                    // Handle different property types
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(equipment, value);
                    }
                    else if (property.PropertyType == typeof(decimal?) && decimal.TryParse(value, out decimal decimalValue))
                    {
                        property.SetValue(equipment, decimalValue);
                    }
                    else if (property.PropertyType == typeof(DateTime?) && DateTime.TryParse(value, out DateTime dateValue))
                    {
                        property.SetValue(equipment, dateValue);
                    }
                    else if (property.PropertyType == typeof(bool) && bool.TryParse(value, out bool boolValue))
                    {
                        property.SetValue(equipment, boolValue);
                    }
                    else if (property.PropertyType == typeof(int?) && int.TryParse(value, out int intValue))
                    {
                        property.SetValue(equipment, intValue);
                    }

                    equipment.UpdatedAt = DateTime.UtcNow;
                    equipment.UpdatedBy = User.Identity?.Name ?? "System";

                    await _context.SaveChangesAsync();

                    return Json(new InlineEditResponse { Success = true, Message = "Field updated successfully" });
                }
                else
                {
                    return Json(new InlineEditResponse { Success = false, Message = $"Field '{request.FieldName}' is not editable" });
                }
            }
            catch (Exception ex)
            {
                return Json(new InlineEditResponse { Success = false, Message = $"Error updating field: {ex.Message}" });
            }
        }

        // GET: SimplifiedEquipment/Export
        public async Task<IActionResult> Export()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var equipmentList = await _context.SimplifiedEquipment
                .Where(e => e.IsActive)
                .OrderBy(e => e.Asset_Tag)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Equipment");

                // Add headers
                var headers = new[]
                {
                    "Asset Tag", "Serial Number", "Service Tag", "Manufacturer", "Model", "Category",
                    "Net Name", "Assigned User Name", "Assigned User Email", "Department", "Unit",
                    "Location", "Floor", "Desk", "Status", "IP Address", "MAC Address",
                    "Wall Port", "Switch Name", "Switch Port", "Phone Number", "Extension", "IMEI",
                    "SIM Card Number", "OS Version", "License1", "License2", "License3", "License4", "License5",
                    "Purchase Price", "Purchase Order Number", "Vendor", "Vendor Info", "Purchase Date", "Warranty Start Date",
                    "Warranty End Date", "Notes", "Created At", "Created By"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Add data
                for (int row = 0; row < equipmentList.Count; row++)
                {
                    var item = equipmentList[row];
                    int col = 1;

                    worksheet.Cells[row + 2, col++].Value = item.Asset_Tag;
                    worksheet.Cells[row + 2, col++].Value = item.Serial_Number;
                    worksheet.Cells[row + 2, col++].Value = item.Service_Tag;
                    worksheet.Cells[row + 2, col++].Value = item.Manufacturer;
                    worksheet.Cells[row + 2, col++].Value = item.Model;
                    worksheet.Cells[row + 2, col++].Value = item.Category;
                    worksheet.Cells[row + 2, col++].Value = item.Net_Name;
                    worksheet.Cells[row + 2, col++].Value = item.Assigned_User_Name;
                    worksheet.Cells[row + 2, col++].Value = item.Assigned_User_Email;
                    worksheet.Cells[row + 2, col++].Value = item.Department;
                    worksheet.Cells[row + 2, col++].Value = item.Unit;
                    worksheet.Cells[row + 2, col++].Value = item.Location;
                    worksheet.Cells[row + 2, col++].Value = item.Floor;
                    worksheet.Cells[row + 2, col++].Value = item.Desk;
                    worksheet.Cells[row + 2, col++].Value = item.Status;
                    worksheet.Cells[row + 2, col++].Value = item.IP_Address;
                    worksheet.Cells[row + 2, col++].Value = item.MAC_Address;
                    worksheet.Cells[row + 2, col++].Value = item.Wall_Port;
                    worksheet.Cells[row + 2, col++].Value = item.Switch_Name;
                    worksheet.Cells[row + 2, col++].Value = item.Switch_Port;
                    worksheet.Cells[row + 2, col++].Value = item.Phone_Number;
                    worksheet.Cells[row + 2, col++].Value = item.Extension;
                    worksheet.Cells[row + 2, col++].Value = item.IMEI;
                    worksheet.Cells[row + 2, col++].Value = item.SIM_Card_Number;
                    worksheet.Cells[row + 2, col++].Value = item.OS_Version;
                    worksheet.Cells[row + 2, col++].Value = item.License1;
                    worksheet.Cells[row + 2, col++].Value = item.License2;
                    worksheet.Cells[row + 2, col++].Value = item.License3;
                    worksheet.Cells[row + 2, col++].Value = item.License4;
                    worksheet.Cells[row + 2, col++].Value = item.License5;
                    worksheet.Cells[row + 2, col++].Value = item.Purchase_Price;
                    worksheet.Cells[row + 2, col++].Value = item.Purchase_Order_Number;
                    worksheet.Cells[row + 2, col++].Value = item.Vendor;
                    worksheet.Cells[row + 2, col++].Value = item.Vendor_Info;
                    worksheet.Cells[row + 2, col++].Value = item.Purchase_Date?.ToString("MM/dd/yyyy");
                    worksheet.Cells[row + 2, col++].Value = item.Warranty_Start_Date?.ToString("MM/dd/yyyy");
                    worksheet.Cells[row + 2, col++].Value = item.Warranty_End_Date?.ToString("MM/dd/yyyy");
                    worksheet.Cells[row + 2, col++].Value = item.Notes;
                    worksheet.Cells[row + 2, col++].Value = item.CreatedAt.ToString("MM/dd/yyyy HH:mm");
                    worksheet.Cells[row + 2, col++].Value = item.CreatedBy;
                }

                worksheet.Cells.AutoFitColumns();

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var filename = $"SimplifiedEquipment_Export_{timestamp}.xlsx";
                var content = package.GetAsByteArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
        }

        // GET: SimplifiedEquipment/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: SimplifiedEquipment/Import
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to import.";
                return RedirectToAction(nameof(Import));
            }

            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            {
                TempData["Error"] = "Please select an Excel file (.xlsx or .xls).";
                return RedirectToAction(nameof(Import));
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using var stream = file.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    TempData["Error"] = "No worksheet found in the Excel file.";
                    return RedirectToAction(nameof(Import));
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                if (rowCount < 2)
                {
                    TempData["Error"] = "The Excel file must contain at least a header row and one data row.";
                    return RedirectToAction(nameof(Import));
                }

                var importedCount = 0;
                var updatedCount = 0;
                var errors = new List<string>();

                // Start from row 2 (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var assetTag = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(assetTag))
                        {
                            errors.Add($"Row {row}: Asset Tag is required");
                            continue;
                        }

                        // Check if equipment already exists
                        var existingEquipment = await _context.SimplifiedEquipment
                            .FirstOrDefaultAsync(e => e.Asset_Tag == assetTag);

                        if (existingEquipment != null)
                        {
                            // Update existing equipment
                            UpdateEquipmentFromExcel(existingEquipment, worksheet, row);
                            existingEquipment.UpdatedAt = DateTime.UtcNow;
                            existingEquipment.UpdatedBy = User.Identity?.Name ?? "System";
                            updatedCount++;
                        }
                        else
                        {
                            // Create new equipment
                            var newEquipment = new SimplifiedEquipment
                            {
                                Asset_Tag = assetTag,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = User.Identity?.Name ?? "System",
                                IsActive = true
                            };

                            UpdateEquipmentFromExcel(newEquipment, worksheet, row);
                            _context.SimplifiedEquipment.Add(newEquipment);
                            importedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                var message = $"Import completed. {importedCount} new records imported, {updatedCount} records updated.";
                if (errors.Any())
                {
                    message += $" {errors.Count} errors occurred.";
                    TempData["ImportErrors"] = string.Join("; ", errors.Take(10)); // Show first 10 errors
                }

                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Import failed: {ex.Message}";
                return RedirectToAction(nameof(Import));
            }
        }

        private void UpdateEquipmentFromExcel(SimplifiedEquipment equipment, ExcelWorksheet worksheet, int row)
        {
            equipment.Serial_Number = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "";
            equipment.Service_Tag = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
            equipment.Manufacturer = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
            equipment.Model = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
            equipment.Category = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
            equipment.Net_Name = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
            equipment.Assigned_User_Name = worksheet.Cells[row, 8].Value?.ToString()?.Trim();
            equipment.Assigned_User_Email = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
            equipment.Department = worksheet.Cells[row, 10].Value?.ToString()?.Trim();
            equipment.Unit = worksheet.Cells[row, 11].Value?.ToString()?.Trim();
            equipment.Location = worksheet.Cells[row, 12].Value?.ToString()?.Trim();
            equipment.Floor = worksheet.Cells[row, 13].Value?.ToString()?.Trim();
            equipment.Desk = worksheet.Cells[row, 14].Value?.ToString()?.Trim();
            equipment.Status = worksheet.Cells[row, 15].Value?.ToString()?.Trim();
            equipment.IP_Address = worksheet.Cells[row, 16].Value?.ToString()?.Trim();
            equipment.MAC_Address = worksheet.Cells[row, 17].Value?.ToString()?.Trim();
            equipment.Wall_Port = worksheet.Cells[row, 18].Value?.ToString()?.Trim();
            equipment.Switch_Name = worksheet.Cells[row, 19].Value?.ToString()?.Trim();
            equipment.Switch_Port = worksheet.Cells[row, 20].Value?.ToString()?.Trim();
            equipment.Phone_Number = worksheet.Cells[row, 21].Value?.ToString()?.Trim();
            equipment.Extension = worksheet.Cells[row, 22].Value?.ToString()?.Trim();
            equipment.IMEI = worksheet.Cells[row, 23].Value?.ToString()?.Trim();
            equipment.SIM_Card_Number = worksheet.Cells[row, 24].Value?.ToString()?.Trim();
            equipment.OS_Version = worksheet.Cells[row, 25].Value?.ToString()?.Trim();
            equipment.License1 = worksheet.Cells[row, 26].Value?.ToString()?.Trim();
            equipment.License2 = worksheet.Cells[row, 27].Value?.ToString()?.Trim();
            equipment.License3 = worksheet.Cells[row, 28].Value?.ToString()?.Trim();
            equipment.License4 = worksheet.Cells[row, 29].Value?.ToString()?.Trim();
            equipment.License5 = worksheet.Cells[row, 30].Value?.ToString()?.Trim();

            // Handle decimal values
            if (decimal.TryParse(worksheet.Cells[row, 31].Value?.ToString(), out decimal purchasePrice))
                equipment.Purchase_Price = purchasePrice;

            equipment.Purchase_Order_Number = worksheet.Cells[row, 32].Value?.ToString()?.Trim();
            equipment.Vendor = worksheet.Cells[row, 33].Value?.ToString()?.Trim();
            equipment.Vendor_Info = worksheet.Cells[row, 34].Value?.ToString()?.Trim();

            // Handle date values
            if (DateTime.TryParse(worksheet.Cells[row, 35].Value?.ToString(), out DateTime purchaseDate))
                equipment.Purchase_Date = purchaseDate;

            if (DateTime.TryParse(worksheet.Cells[row, 36].Value?.ToString(), out DateTime warrantyStart))
                equipment.Warranty_Start_Date = warrantyStart;

            if (DateTime.TryParse(worksheet.Cells[row, 37].Value?.ToString(), out DateTime warrantyEnd))
                equipment.Warranty_End_Date = warrantyEnd;

            equipment.Notes = worksheet.Cells[row, 38].Value?.ToString()?.Trim();
        }

        private bool SimplifiedEquipmentExists(int id)
        {
            return _context.SimplifiedEquipment.Any(e => e.Id == id);
        }
    }

}
