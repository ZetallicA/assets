# Search Functionality Test Guide

## Testing Equipment Search

The Equipment page at `http://localhost:5009/Equipment` should have full search functionality.

### What Should Work:

1. **Search Box**: Large search input field at the top of the page
2. **Auto-suggestions**: As you type (2+ characters), suggestions should appear
3. **Search Fields**: 
   - OATH Tag
   - Serial Number  
   - Net Name (Computer Name)
   - User Name
   - User Email
   - Model
   - Manufacturer
   - Department
   - Facility

### Test Steps:

1. **Basic Search**:
   - Go to `http://localhost:5009/Equipment`
   - Type "RE-DO" in the search box
   - Press Enter or click the search button
   - Should show equipment with OATH tags containing "RE-DO"

2. **Auto-suggestions**:
   - Type "RE" in the search box
   - Wait 300ms
   - Should see dropdown with suggestions
   - Click on a suggestion to search

3. **Sorting**:
   - Click column headers to sort
   - OATH Tag, Net Name, Model, Status, Location, Assigned To, Category

### If Search Doesn't Work:

1. **Check Browser Console** (F12):
   - Look for JavaScript errors
   - Check Network tab for failed API calls

2. **Check Application Logs**:
   - Look for errors in the terminal where `dotnet run` is running

3. **Verify Database**:
   - Ensure equipment data exists in the database
   - Check that searchable fields have data

### Common Issues:

- **No suggestions**: Check if `GetSearchSuggestions` action is working
- **Search returns nothing**: Verify search terms match actual data
- **JavaScript errors**: Check browser console for errors
