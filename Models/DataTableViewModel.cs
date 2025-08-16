using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Models
{
    public class DataTableViewModel
    {
        public string TableId { get; set; } = "datatable";
        public Dictionary<string, DataTableColumn> Columns { get; set; } = new();
        public List<DataTableRow> Rows { get; set; } = new();
        public DataTablePagination? Pagination { get; set; }
        public string? ApiEndpoint { get; set; }
        public string? UpdateEndpoint { get; set; }
    }

    public class DataTableColumn
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public bool Sortable { get; set; } = true;
        public bool Editable { get; set; } = false;
        public string SortDirection { get; set; } = "none"; // none, asc, desc
        public string? FieldType { get; set; } // text, select, date, etc.
        public Dictionary<string, string>? Options { get; set; } // For select fields
    }

    public class DataTableRow
    {
        public string Id { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new();
        public Dictionary<string, string> DisplayData { get; set; } = new();

        public object GetValue(string key)
        {
            return Data.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public string GetDisplayValue(string key)
        {
            return DisplayData.TryGetValue(key, out var value) ? value : GetValue(key)?.ToString() ?? string.Empty;
        }
    }

    public class DataTablePagination
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int TotalPages { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
    }

    public class DataTableRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? SortColumn { get; set; }
        public string SortDirection { get; set; } = "asc";
        public string? SearchTerm { get; set; }
        public Dictionary<string, string>? Filters { get; set; }
        public List<string>? VisibleColumns { get; set; }
    }

    public class DataTableResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public DataTablePagination Pagination { get; set; } = new();
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class InlineEditRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
    }

    public class InlineEditResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public object? NewValue { get; set; }
        public string? NewDisplayValue { get; set; }
    }
}
