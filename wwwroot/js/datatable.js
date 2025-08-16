/**
 * DataTable Module - ES6 Module for enhanced table functionality
 * Features: Keyboard navigation, inline editing, saved views, bulk actions, CSV export
 */

class DataTable {
    constructor(tableId, options = {}) {
        this.tableId = tableId;
        this.container = document.querySelector(`[data-table-id="${tableId}"]`);
        this.table = this.container?.querySelector('.datatable');
        this.options = {
            apiEndpoint: null,
            updateEndpoint: null,
            enableKeyboard: true,
            enableInlineEdit: true,
            enableSavedViews: true,
            enableBulkActions: true,
            ...options
        };
        
        this.selectedRows = new Set();
        this.currentEditCell = null;
        this.lastSelectedRow = null;
        this.savedViews = this.loadSavedViews();
        
        this.init();
    }

    init() {
        if (!this.container || !this.table) {
            console.error('DataTable: Container or table not found');
            return;
        }

        this.bindEvents();
        this.updateRowCount();
        this.loadSavedView();
    }

    bindEvents() {
        // Column visibility
        this.container.addEventListener('change', (e) => {
            if (e.target.matches('.column-toggle-menu input[type="checkbox"]')) {
                this.toggleColumn(e.target.dataset.column, e.target.checked);
            }
        });

        // Row selection
        this.container.addEventListener('change', (e) => {
            if (e.target.matches('#select-all')) {
                this.toggleSelectAll(e.target.checked);
            } else if (e.target.matches('.row-selector')) {
                this.toggleRowSelection(e.target.value, e.target.checked);
            }
        });

        // Saved views
        this.container.addEventListener('click', (e) => {
            if (e.target.matches('[data-action="save-view"]')) {
                e.preventDefault();
                this.saveCurrentView();
            } else if (e.target.matches('[data-action="reset-view"]')) {
                e.preventDefault();
                this.resetToDefaultView();
            }
        });

        // Bulk actions
        this.container.addEventListener('click', (e) => {
            if (e.target.matches('[data-action="bulk-status"]')) {
                e.preventDefault();
                this.showBulkStatusModal();
            } else if (e.target.matches('[data-action="bulk-export"]')) {
                e.preventDefault();
                this.exportSelectedRows();
            }
        });

        // Export CSV
        this.container.addEventListener('click', (e) => {
            if (e.target.matches('[data-action="export-csv"]')) {
                e.preventDefault();
                this.exportToCSV();
            }
        });

        // Inline editing
        if (this.options.enableInlineEdit) {
            this.bindInlineEditEvents();
        }

        // Keyboard navigation
        if (this.options.enableKeyboard) {
            this.bindKeyboardEvents();
        }

        // Pagination
        this.container.addEventListener('click', (e) => {
            if (e.target.matches('.pagination .page-link')) {
                e.preventDefault();
                const page = parseInt(e.target.dataset.page);
                if (page > 0) {
                    this.goToPage(page);
                }
            }
        });

        // Sorting
        this.container.addEventListener('click', (e) => {
            if (e.target.closest('.sortable')) {
                e.preventDefault();
                const header = e.target.closest('.sortable');
                const column = header.dataset.column;
                this.sortByColumn(column);
            }
        });
    }

    bindInlineEditEvents() {
        this.container.addEventListener('dblclick', (e) => {
            const cell = e.target.closest('.editable');
            if (cell && !this.currentEditCell) {
                this.startInlineEdit(cell);
            }
        });

        this.container.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && this.currentEditCell) {
                e.preventDefault();
                this.finishInlineEdit();
            } else if (e.key === 'Escape' && this.currentEditCell) {
                e.preventDefault();
                this.cancelInlineEdit();
            }
        });
    }

    bindKeyboardEvents() {
        this.container.addEventListener('keydown', (e) => {
            const activeElement = document.activeElement;
            const currentRow = activeElement.closest('.datatable-row');
            
            if (!currentRow) return;

            switch (e.key) {
                case 'ArrowUp':
                    e.preventDefault();
                    this.navigateRow('up', currentRow);
                    break;
                case 'ArrowDown':
                    e.preventDefault();
                    this.navigateRow('down', currentRow);
                    break;
                case 'Enter':
                    if (!this.currentEditCell) {
                        e.preventDefault();
                        this.openRowDetails(currentRow);
                    }
                    break;
                case ' ':
                    e.preventDefault();
                    this.toggleRowSelection(currentRow.dataset.rowId);
                    break;
            }
        });
    }

    navigateRow(direction, currentRow) {
        const rows = Array.from(this.table.querySelectorAll('.datatable-row'));
        const currentIndex = rows.indexOf(currentRow);
        let nextIndex;

        if (direction === 'up') {
            nextIndex = Math.max(0, currentIndex - 1);
        } else {
            nextIndex = Math.min(rows.length - 1, currentIndex + 1);
        }

        if (nextIndex !== currentIndex) {
            const nextRow = rows[nextIndex];
            const firstCell = nextRow.querySelector('td:not(.select-column):not(.actions-cell)');
            if (firstCell) {
                firstCell.focus();
                firstCell.setAttribute('tabindex', '0');
            }
        }
    }

    startInlineEdit(cell) {
        const display = cell.querySelector('.editable-display');
        const input = cell.querySelector('.editable-input');
        
        if (!display || !input) return;

        this.currentEditCell = cell;
        display.style.display = 'none';
        input.style.display = 'block';
        input.focus();
        input.select();
    }

    async finishInlineEdit() {
        if (!this.currentEditCell) return;

        const cell = this.currentEditCell;
        const display = cell.querySelector('.editable-display');
        const input = cell.querySelector('.editable-input');
        const rowId = cell.closest('.datatable-row').dataset.rowId;
        const field = cell.dataset.field;
        const newValue = input.value;

        try {
            const response = await this.updateField(rowId, field, newValue);
            
            if (response.success) {
                display.textContent = response.newDisplayValue || newValue;
                cell.dataset.value = newValue;
                this.showToast('success', 'Field updated successfully');
            } else {
                this.showToast('error', response.errorMessage || 'Failed to update field');
                input.value = cell.dataset.value;
            }
        } catch (error) {
            this.showToast('error', 'Failed to update field');
            input.value = cell.dataset.value;
        }

        this.endInlineEdit();
    }

    cancelInlineEdit() {
        if (!this.currentEditCell) return;

        const cell = this.currentEditCell;
        const display = cell.querySelector('.editable-display');
        const input = cell.querySelector('.editable-input');
        
        input.value = cell.dataset.value;
        this.endInlineEdit();
    }

    endInlineEdit() {
        if (!this.currentEditCell) return;

        const cell = this.currentEditCell;
        const display = cell.querySelector('.editable-display');
        const input = cell.querySelector('.editable-input');
        
        display.style.display = 'block';
        input.style.display = 'none';
        this.currentEditCell = null;
    }

    async updateField(rowId, field, value) {
        if (!this.options.updateEndpoint) {
            throw new Error('Update endpoint not configured');
        }

        const response = await fetch(this.options.updateEndpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': this.getAntiForgeryToken()
            },
            body: JSON.stringify({
                id: rowId,
                field: field,
                value: value
            })
        });

        return await response.json();
    }

    toggleColumn(columnKey, visible) {
        const headers = this.table.querySelectorAll(`th[data-column="${columnKey}"]`);
        const cells = this.table.querySelectorAll(`td[data-field="${columnKey}"]`);
        
        headers.forEach(header => {
            header.style.display = visible ? '' : 'none';
        });
        
        cells.forEach(cell => {
            cell.style.display = visible ? '' : 'none';
        });

        this.saveColumnVisibility();
    }

    toggleSelectAll(checked) {
        const checkboxes = this.table.querySelectorAll('.row-selector');
        checkboxes.forEach(checkbox => {
            checkbox.checked = checked;
            this.toggleRowSelection(checkbox.value, checked);
        });
    }

    toggleRowSelection(rowId, selected = null) {
        if (selected === null) {
            selected = !this.selectedRows.has(rowId);
        }

        if (selected) {
            this.selectedRows.add(rowId);
        } else {
            this.selectedRows.delete(rowId);
        }

        this.updateBulkActionsVisibility();
        this.updateRowCount();
    }

    updateBulkActionsVisibility() {
        const bulkActionsDropdown = this.container.querySelector('#bulk-actions-dropdown');
        if (bulkActionsDropdown) {
            bulkActionsDropdown.style.display = this.selectedRows.size > 0 ? 'block' : 'none';
        }
    }

    updateRowCount() {
        const selectedCount = this.container.querySelector('#selected-count');
        const totalCount = this.container.querySelector('#total-count');
        
        if (selectedCount) {
            selectedCount.textContent = this.selectedRows.size;
        }
        
        if (totalCount) {
            const totalRows = this.table.querySelectorAll('.datatable-row').length;
            totalCount.textContent = totalRows;
        }
    }

    saveCurrentView() {
        const viewName = prompt('Enter a name for this view:');
        if (!viewName) return;

        const view = {
            name: viewName,
            timestamp: new Date().toISOString(),
            columns: this.getColumnVisibility(),
            filters: this.getCurrentFilters(),
            sort: this.getCurrentSort()
        };

        this.savedViews[viewName] = view;
        this.saveSavedViews();
        this.showToast('success', `View "${viewName}" saved successfully`);
    }

    loadSavedView() {
        const currentRoute = window.location.pathname;
        const savedView = this.savedViews[currentRoute];
        
        if (savedView) {
            this.applySavedView(savedView);
        }
    }

    applySavedView(view) {
        // Apply column visibility
        Object.entries(view.columns).forEach(([column, visible]) => {
            this.toggleColumn(column, visible);
        });

        // Apply filters and sort (would need to be implemented based on your filtering system)
        // this.applyFilters(view.filters);
        // this.applySort(view.sort);
    }

    resetToDefaultView() {
        if (confirm('Are you sure you want to reset to the default view?')) {
            const currentRoute = window.location.pathname;
            delete this.savedViews[currentRoute];
            this.saveSavedViews();
            location.reload();
        }
    }

    getColumnVisibility() {
        const visibility = {};
        const checkboxes = this.container.querySelectorAll('.column-toggle-menu input[type="checkbox"]');
        
        checkboxes.forEach(checkbox => {
            visibility[checkbox.dataset.column] = checkbox.checked;
        });
        
        return visibility;
    }

    getCurrentFilters() {
        // Implementation depends on your filtering system
        return {};
    }

    getCurrentSort() {
        // Implementation depends on your sorting system
        return {};
    }

    saveColumnVisibility() {
        const visibility = this.getColumnVisibility();
        const currentRoute = window.location.pathname;
        
        if (!this.savedViews[currentRoute]) {
            this.savedViews[currentRoute] = {};
        }
        
        this.savedViews[currentRoute].columns = visibility;
        this.saveSavedViews();
    }

    loadSavedViews() {
        try {
            const saved = localStorage.getItem('datatable-views');
            return saved ? JSON.parse(saved) : {};
        } catch {
            return {};
        }
    }

    saveSavedViews() {
        try {
            localStorage.setItem('datatable-views', JSON.stringify(this.savedViews));
        } catch (error) {
            console.error('Failed to save views:', error);
        }
    }

    exportToCSV() {
        const visibleColumns = Array.from(this.table.querySelectorAll('th:not(.select-all-column):not(.actions-column)'))
            .filter(th => th.style.display !== 'none')
            .map(th => th.textContent.trim());

        const rows = Array.from(this.table.querySelectorAll('.datatable-row'))
            .map(row => {
                return visibleColumns.map(column => {
                    const cell = row.querySelector(`td[data-field="${column}"]`);
                    return cell ? `"${cell.textContent.trim()}"` : '""';
                }).join(',');
            });

        const csvContent = [visibleColumns.join(','), ...rows].join('\n');
        this.downloadCSV(csvContent, `${this.tableId}-export.csv`);
    }

    exportSelectedRows() {
        if (this.selectedRows.size === 0) {
            this.showToast('warning', 'No rows selected for export');
            return;
        }

        const visibleColumns = Array.from(this.table.querySelectorAll('th:not(.select-all-column):not(.actions-column)'))
            .filter(th => th.style.display !== 'none')
            .map(th => th.textContent.trim());

        const rows = Array.from(this.table.querySelectorAll('.datatable-row'))
            .filter(row => this.selectedRows.has(row.dataset.rowId))
            .map(row => {
                return visibleColumns.map(column => {
                    const cell = row.querySelector(`td[data-field="${column}"]`);
                    return cell ? `"${cell.textContent.trim()}"` : '""';
                }).join(',');
            });

        const csvContent = [visibleColumns.join(','), ...rows].join('\n');
        this.downloadCSV(csvContent, `${this.tableId}-selected-export.csv`);
    }

    downloadCSV(content, filename) {
        const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }

    showBulkStatusModal() {
        // Implementation for bulk status change modal
        this.showToast('info', 'Bulk status change feature coming soon');
    }

    openRowDetails(row) {
        const viewBtn = row.querySelector('[data-action="view"]');
        if (viewBtn) {
            viewBtn.click();
        }
    }

    goToPage(page) {
        // Implementation depends on your pagination system
        const url = new URL(window.location);
        url.searchParams.set('page', page);
        window.location.href = url.toString();
    }

    sortByColumn(column) {
        // Implementation depends on your sorting system
        const url = new URL(window.location);
        const currentSort = url.searchParams.get('sort');
        const currentDirection = url.searchParams.get('direction') || 'asc';
        
        let newDirection = 'asc';
        if (currentSort === column && currentDirection === 'asc') {
            newDirection = 'desc';
        }
        
        url.searchParams.set('sort', column);
        url.searchParams.set('direction', newDirection);
        window.location.href = url.toString();
    }

    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    showToast(type, message) {
        // Use the global toast system
        if (window.showToast) {
            window.showToast(type, message);
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }
}

// Export for use as ES module
export default DataTable;

// Also make available globally for backward compatibility
window.DataTable = DataTable;
