/**
 * Global Search Module
 * Handles the global search functionality in the navbar
 */
class GlobalSearch {
    constructor() {
        this.searchInput = document.getElementById('globalSearch');
        this.searchBtn = document.getElementById('searchBtn');
        this.resultsContainer = document.getElementById('searchResults');
        this.selectedIndex = -1;
        this.results = [];
        this.debounceTimer = null;
        this.isVisible = false;
    }

    init() {
        if (!this.searchInput) {
            console.log('Global search input not found');
            return;
        }
        
        console.log('Initializing global search');
        this.bindEvents();
        this.setupKeyboardShortcuts();
        console.log('Global search initialized successfully');
    }

    bindEvents() {
        // Search input events
        this.searchInput.addEventListener('input', (e) => this.handleInput(e));
        this.searchInput.addEventListener('keydown', (e) => this.handleKeydown(e));
        this.searchInput.addEventListener('focus', () => this.showResults());
        this.searchInput.addEventListener('blur', () => {
            // Delay hiding to allow for clicks on results
            setTimeout(() => this.hideResults(), 200);
        });

        // Search button
        if (this.searchBtn) {
            this.searchBtn.addEventListener('click', () => this.performSearch());
        }

        // Click outside to close
        document.addEventListener('click', (e) => {
            if (!this.searchInput.contains(e.target) && !this.resultsContainer.contains(e.target)) {
                this.hideResults();
            }
        });
    }

    setupKeyboardShortcuts() {
        // Global keyboard shortcuts
        document.addEventListener('keydown', (e) => {
            // '/' key to focus search (when not in input fields)
            if (e.key === '/' && !this.isInputFocused()) {
                console.log('Global search shortcut triggered');
                e.preventDefault();
                this.focusSearch();
            }
        });
    }

    async handleInput(e) {
        const query = e.target.value.trim();
        
        // Clear previous debounce timer
        if (this.debounceTimer) {
            clearTimeout(this.debounceTimer);
        }

        // Hide results if query is too short
        if (query.length < 2) {
            this.hideResults();
            return;
        }

        // Debounce the search
        this.debounceTimer = setTimeout(async () => {
            await this.fetchSuggestions(query);
        }, 300);
    }

    handleKeydown(e) {
        if (!this.isVisible) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                this.navigateResults(1);
                break;
            case 'ArrowUp':
                e.preventDefault();
                this.navigateResults(-1);
                break;
            case 'Enter':
                e.preventDefault();
                if (this.selectedIndex >= 0 && this.selectedIndex < this.results.length) {
                    this.selectResult(this.results[this.selectedIndex]);
                } else {
                    this.performSearch();
                }
                break;
            case 'Escape':
                e.preventDefault();
                this.hideResults();
                this.searchInput.blur();
                break;
        }
    }

    async fetchSuggestions(query) {
        try {
            const response = await fetch(`/Search/Suggest?q=${encodeURIComponent(query)}`);
            if (!response.ok) throw new Error('Search request failed');
            
            const results = await response.json();
            this.results = results;
            this.displayResults(results);
        } catch (error) {
            console.error('Search error:', error);
            this.showError('Search failed. Please try again.');
        }
    }

    displayResults(results) {
        if (!this.resultsContainer) return;

        if (results.length === 0) {
            this.resultsContainer.innerHTML = `
                <div class="dropdown-menu show p-3">
                    <div class="text-muted">No results found</div>
                </div>
            `;
            this.showResults();
            return;
        }

        const html = `
            <div class="dropdown-menu show">
                ${results.map((result, index) => `
                    <a href="#" class="dropdown-item search-result-item ${index === this.selectedIndex ? 'active' : ''}" 
                       data-index="${index}" data-type="${result.type}" data-id="${result.id}">
                        <div class="d-flex align-items-center">
                            <div class="me-3">
                                <i class="${this.getResultIcon(result.type)}"></i>
                            </div>
                            <div class="flex-grow-1">
                                <div class="fw-medium">${this.escapeHtml(result.label)}</div>
                                <div class="small text-muted">${this.escapeHtml(result.meta)}</div>
                            </div>
                            <div class="ms-2">
                                <small class="text-muted">${result.type}</small>
                            </div>
                        </div>
                    </a>
                `).join('')}
                <div class="dropdown-divider"></div>
                <a href="#" class="dropdown-item text-center" id="viewAllResults">
                    <small>View all results</small>
                </a>
            </div>
        `;

        this.resultsContainer.innerHTML = html;
        this.showResults();

        // Bind result item clicks
        this.resultsContainer.querySelectorAll('.search-result-item').forEach(item => {
            item.addEventListener('click', (e) => {
                e.preventDefault();
                const index = parseInt(item.dataset.index);
                this.selectResult(this.results[index]);
            });
        });

        // Bind "View all results" click
        const viewAllBtn = this.resultsContainer.querySelector('#viewAllResults');
        if (viewAllBtn) {
            viewAllBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.performSearch();
            });
        }
    }

    navigateResults(direction) {
        const newIndex = this.selectedIndex + direction;
        
        if (newIndex >= -1 && newIndex < this.results.length) {
            this.selectedIndex = newIndex;
            this.updateSelection();
        }
    }

    updateSelection() {
        this.resultsContainer.querySelectorAll('.search-result-item').forEach((item, index) => {
            item.classList.toggle('active', index === this.selectedIndex);
        });
    }

    selectResult(result) {
        if (!result || !result.actionUrls) return;

        // Navigate to the view URL
        if (result.actionUrls.view) {
            window.location.href = result.actionUrls.view;
        }
    }

    performSearch() {
        const query = this.searchInput.value.trim();
        if (query) {
            window.location.href = `/Search/Global?q=${encodeURIComponent(query)}`;
        }
    }

    showResults() {
        if (this.resultsContainer) {
            this.resultsContainer.style.display = 'block';
            this.isVisible = true;
        }
    }

    hideResults() {
        if (this.resultsContainer) {
            this.resultsContainer.style.display = 'none';
            this.isVisible = false;
            this.selectedIndex = -1;
        }
    }

    focusSearch() {
        if (this.searchInput) {
            this.searchInput.focus();
            this.searchInput.select();
        }
    }

    isInputFocused() {
        const activeElement = document.activeElement;
        return activeElement && (
            activeElement.tagName === 'INPUT' || 
            activeElement.tagName === 'TEXTAREA' || 
            activeElement.contentEditable === 'true'
        );
    }

    getResultIcon(type) {
        const icons = {
            equipment: 'fas fa-desktop',
            techconfig: 'fas fa-network-wired',
            floorplan: 'fas fa-map',
            user: 'fas fa-user'
        };
        return icons[type] || 'fas fa-search';
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    showError(message) {
        if (this.resultsContainer) {
            this.resultsContainer.innerHTML = `
                <div class="dropdown-menu show p-3">
                    <div class="text-danger">${this.escapeHtml(message)}</div>
                </div>
            `;
            this.showResults();
        }
    }
}

// Initialize global search when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    const globalSearch = new GlobalSearch();
    globalSearch.init();
});

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = GlobalSearch;
}
