const apiBaseUrl = 'https://localhost:7089';
const userRole = "admin"; // get from token
const currentHtmlPage = document.body.id;
let currentPage = 1;
const pageSize = 10;
let currentFilters = {};

 // for testing this token lifetime is extended
 let token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJmNDdhYzEwYi01OGNjLTQzNzItYTU2Ny0wZTAyYjJjM2Q0NzEiLCJ1bmlxdWVfbmFtZSI6InNhcmFoQGV4YW1wbGUuY29tIiwicGFzc3dvcmRVcGRhdGVkIjoidHJ1ZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJBZG1pbiIsIlVzZXIiXSwiZXhwIjoxNzczMjE4MTQxLCJpc3MiOiJnZXRBcm9vbV9BUEkiLCJhdWQiOiJnZXRBcm9vbV9BUEkifQ.SUMYes6mCCjciRkpvvwazgI6QH_lhME5Y9IsENlUEuM";

if(currentHtmlPage === "usersBody")
    window.onload = function () {
        fetchUsers(true);
};

function showPanel(currentHtmlPage) {
    const applicablePages = ["indexBody", "reservationsBody", "roomsBody"]
    if (applicablePages.includes(currentHtmlPage)){
        const adminPanel = document.getElementById("adminPanel");
        const userPanel = document.getElementById("userPanel");

        if (userRole === "admin" && adminPanel) {
            adminPanel.style.display = "block";
        } else if (userRole === "user" && userPanel) {
            userPanel.style.display = "block";
        } else {
            console.error("Neither adminPanel nor userPanel found.");
        }
    }
}

async function fetchUsers(resetPage = false, filters = {}) {
    if (resetPage) {
        currentPage = 1;
        document.getElementById('prevPageButton').disabled = true;
        document.getElementById('nextPageButton').disabled = false;
        document.getElementById('goToPageButton').disabled = false;
        document.getElementById('pageInput').disabled = false;              
    }

    const activeHeader = document.querySelector('.sortable[data-active="true"]');
    const sortBy = activeHeader ? activeHeader.dataset.column : 'LastName';
    const order = activeHeader ? activeHeader.dataset.order : 'ASC';

    let url = `${apiBaseUrl}/api/v1/users?page=${currentPage}&pageSize=${pageSize}&sortby=${encodeURIComponent(sortBy)}&order=${encodeURIComponent(order)}`;

    // Append the search filters
    Object.keys(filters).forEach(key => {
        if (filters[key]) {
            url += `&${encodeURIComponent(key)}=${encodeURIComponent(filters[key])}`;
        }
    });
   
    // Build the fetch options object with headers
    const fetchOptions = {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    };  

    try {
        const response = await fetch(url,  fetchOptions  );
        if (response.status === 404) {
            currentPage = Math.max(1, currentPage - 1);
            document.getElementById('nextPageButton').disabled = true;
            document.getElementById('goToPageButton').disabled = true;
            document.getElementById('pageInput').disabled = true;
            populateTable([]);
            makeButtonsAndInputsVisible();
            return { totalCount: 0 };
        }
        if (!response.ok) {
            throw new Error(`Network response was not ok: ${response.statusText}`);
        }
        const jsonResponse = await response.json();
        const users = jsonResponse.userDTOs || [];
        const totalCount = jsonResponse.totalCount || 0;
        const totalPages = Math.ceil(totalCount / pageSize);

        populateTable(users);
        makeButtonsAndInputsVisible();
        const hasResults = users.length > 0;
        const isLastPage = currentPage >= totalPages;

        document.querySelector('.allUsersTable').style.visibility = 'visible';
        document.getElementById('prevPageButton').disabled = currentPage === 1;
        document.getElementById('nextPageButton').disabled = isLastPage || !hasResults;
        document.getElementById('goToPageButton').disabled = !hasResults;
        document.getElementById('pageInput').disabled = !hasResults;

        document.getElementById('currentPageContainer').style.visibility = 'visible';
        document.getElementById('users-managment-header').style.visibility = 'visible';
        document.getElementById('currentPageDisplay').textContent = `${currentPage} / ${totalPages}`;

        console.log(`Loaded page ${currentPage}`);
        return { totalCount };
    } catch (error) {
        console.error('Error fetching users:', error);
        alert('Failed to load all users. Check the console for more details.');
        return { totalCount: 0 };
    }
}

async function fetchUserById (userId) {    
    history.replaceState(null, '', window.location.pathname);
    if (!userId) {
        alert('Please enter a User ID.');
        return;
    }

    const url = `${apiBaseUrl}/api/v1/users/${userId}`;

        // Build the fetch options object with headers
        const fetchOptions = {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        };      

    try {
        const response = await fetch(url, fetchOptions);

        if (!response.ok) {
            if (response.status === 404) {
                populateTable([]);
                document.getElementById('currentPageContainer').style.visibility = 'hidden';                
               return;              
            }           
        }

        const data = await response.json();
        populateTable(data);
        document.querySelector('.allUsersTable').style.visibility = 'visible';
      
        resetPagination();
    } catch (error) {
        console.error('Error fetching user by ID:', error);
        alert(error.message);
        document.querySelector('.allUsersTable tbody').innerHTML = '';
    }
}

function nextPage() {
    currentPage++;
    fetchUsers(false, currentFilters);
}

function prevPage() {
    if (currentPage > 1) {
        currentPage--;
        fetchUsers(false, currentFilters);
    } else {
        alert('You are already on the first page!');
    }
}

async function gotoPage() {
    const pageInput = document.getElementById('pageInput').value;
    const page = parseInt(pageInput, 10);

    if (isNaN(page) || page <= 0) {
        alert('Please enter a valid page number.');
        return;
    }

    // Use currentFilters as the second parameter so that the search filters are preserved.
    const { totalCount } = await fetchUsers(false, currentFilters);
    const totalPages = Math.ceil(totalCount / pageSize); 
    if (page > totalPages) {       
        document.getElementById('pageInput').value = '';        
        return;
    }

    currentPage = page;
    fetchUsers(false, currentFilters);
    document.getElementById('pageInput').value = '';
}

function resetPagination() {
    document.getElementById('prevPageButton').disabled = true;
    document.getElementById('nextPageButton').disabled = true;
    document.getElementById('goToPageButton').disabled = true;
    document.getElementById('pageInput').disabled = true;
    document.getElementById('currentPageDisplay').textContent = '1';
}

function makeButtonsAndInputsVisible() {
    const buttons = document.querySelectorAll('button');
    buttons.forEach(button => {
    button.style.visibility = 'visible';
    });

    const input = document.querySelectorAll('input');
    input.forEach(input => {
    input.style.visibility = 'visible';
});            
}

function populateTable(data) {
    const table = document.querySelector('.allUsersTable');
    const message = document.getElementById('noDataMessage');
    const tbody = document.querySelector('.allUsersTable tbody');
    tbody.innerHTML = ''; // Clear existing data

    if (!data || (Array.isArray(data) && data.length === 0)) {
        // No data found, hide table and show message
        table.style.display = 'none';
        message.style.display = 'block';
        message.textContent = 'No users found'; // Default for queries
        document.getElementById('currentPageContainer').style.visibility = 'hidden';
    } else {
        // Data found, hide message and show table
        table.style.display = 'table';
        message.style.display = 'none';

        // Populate the table with data
        const users = Array.isArray(data) ? data : [data]; // Handle single user or array
        users.forEach(user => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${user.id.value}</td>
                <td>${user.lastName}</td>
                <td>${user.firstName}</td>
                <td>${user.phoneNumber}</td>
                <td>${user.email}</td>
            `;
            tbody.appendChild(row);
        });
    }
}

function onSortHeaderClick(event) {
    const header = event.target.closest('.sortable'); // Ensure a sortable header was clicked
    if (!header) return;

    const currentOrder = header.dataset.order;
    const newOrder = currentOrder === 'ASC' ? 'DESC' : 'ASC';

    // Reset other headers' active states
    document.querySelectorAll('.sortable').forEach(h => {
        h.dataset.active = false;
        h.querySelector('i').className = 'fas fa-sort'; // Reset icon
    });

    // Set active state for clicked header
    header.dataset.order = newOrder;
    header.dataset.active = true;
    header.querySelector('i').className = newOrder === 'ASC' ? 'fas fa-sort-up' : 'fas fa-sort-down';

    // Use the existing filters and reload users with the new sorting parameters
    fetchUsers(true, currentFilters);
}

function navigateTo(url) {
    window.location.href = url;
}
    
showPanel(currentHtmlPage);

if(document.getElementById('manageUsersButton')){
    document.getElementById('manageUsersButton').addEventListener('click', () => navigateTo('users.html'));
}

if(document.getElementById('manageReservationsButton')){
    document.getElementById('manageReservationsButton').addEventListener('click', () => navigateTo('reservations.html'));
}

if(document.getElementById('manageRoomsButton')){
    document.getElementById('manageRoomsButton').addEventListener('click', () => navigateTo('rooms.html'));
}

// all eventlisterners must have conditional checks since they dont exist in index.html (should have used separate .js for each html(?))
if(document.getElementById('fetchUsers')){
    document.getElementById('fetchUsers').addEventListener('click', () => {
          document.querySelectorAll('.sortable').forEach(h => {
        h.dataset.active = false;
        h.querySelector('i').className = 'fas fa-sort'; // Reset icon
    });
        fetchUsers(true);
    });
}

if (document.getElementById('searchButton')) {
    document.getElementById('searchButton').addEventListener('click', () => {
        // Retrieve the selected search column value.
        const searchColumnElement = document.getElementById('searchColumn');
        const selectedColumn = searchColumnElement.value;
        
        // Retrieve the search input value.
        const searchTerm = document.getElementById('searchInput').value.trim();

        if (!selectedColumn) {
            alert('Please select a search criteria.');
            return;
        }

        if(selectedColumn === 'Id'){
            fetchUserById(searchTerm)
            return;
        }
        
        // Create the filters object and update the global variable.
        currentFilters = {};
        currentFilters[selectedColumn] = searchTerm;        

        // Call fetchUsers with the active filters.
        fetchUsers(true, currentFilters);
    });
}

if(document.getElementById('prevPageButton')){
    document.getElementById('prevPageButton').addEventListener('click', prevPage);
}

if(document.getElementById('nextPageButton')){
    document.getElementById('nextPageButton').addEventListener('click', nextPage);
}

if(document.getElementById('goToPageButton')){
document.getElementById('goToPageButton').addEventListener('click', gotoPage);
}

document.querySelectorAll('.sortable').forEach(header => {
    header.addEventListener('click', onSortHeaderClick);
});

