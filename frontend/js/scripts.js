const apiBaseUrl = 'https://localhost:7089';
const userRole = "admin"; // get from token(?)
const currentHtmlPage = document.body.id;
let currentPage = 1;
const pageSize = 5;

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

async function loadUsers(resetPage = false, clearFilters = false) {
    if (resetPage) {
        currentPage = 1;
        document.getElementById('prevPageButton').disabled = true;
        document.getElementById('nextPageButton').disabled = false;
        document.getElementById('goToPageButton').disabled = false;
        document.getElementById('pageInput').disabled = false;
        document.getElementById('userIdInput').value = '';
    }

    const params = new URLSearchParams(window.location.search);
    const firstName = params.get("firstname");
    const lastName = params.get("lastname");
    const phoneNumber = params.get("phonenumber");
    const email = params.get("email");
    const sortBy = params.get("sortby");
    const order = params.get("order");

    let url = `${apiBaseUrl}/api/v1/users?page=${currentPage}&pageSize=${pageSize}`;

    if (!clearFilters) {
        if (firstName) url += `&firstName=${encodeURIComponent(firstName)}`;
        if (lastName) url += `&lastName=${encodeURIComponent(lastName)}`;
        if (phoneNumber) url += `&phoneNumber=${encodeURIComponent(phoneNumber)}`;
        if (email) url += `&email=${encodeURIComponent(email)}`;
        if (sortBy) url += `&sortby=${encodeURIComponent(sortBy)}`
        if (order) url += `&order=${encodeURIComponent(order)}`
    } else {
        history.replaceState(null, '', window.location.pathname); // Clear query string
    }

    try {
        const response = await fetch(url);

        if (response.status === 404) {
            currentPage = Math.max(1, currentPage - 1);
            document.getElementById('nextPageButton').disabled = true;
            document.getElementById('goToPageButton').disabled = true;
            document.getElementById('pageInput').disabled = true;
            populateTable([]);
            makeButtonsAndInputsVisible();
            return { totalCount: 0 }; // Return zero totalCount for empty results
        }

        if (!response.ok) {
            throw new Error(`Network response was not ok: ${response.statusText}`);
        }

        const jsonResponse = await response.json();
        const users = jsonResponse.data || [];
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
        document.getElementById('currentPageDisplay').textContent = `${currentPage} / ${totalPages}`;

        console.log(`Loaded page ${currentPage}`);

        return { totalCount }; // Return the totalCount
    } catch (error) {
        console.error('Error fetching users:', error);
        alert('Failed to load all users. Check the console for more details.');
        return { totalCount: 0 }; // Handle errors gracefully with zero totalCount
    }
}

if(currentHtmlPage === "usersBody")
    window.onload = function () {
        loadUsers(true);
};

function nextPage() {
    currentPage++;
    loadUsers(false, false);
}

function prevPage() {
    if (currentPage > 1) {
        currentPage--;
        loadUsers(false, false);
    } else {
        alert('You are already on the first page!');
    }
}

async function gotoPage() {
    const pageInput = document.getElementById('pageInput').value;
    const page = parseInt(pageInput);

    if (isNaN(page) || page <= 0) {
        alert('Please enter a valid page number.');
        return;
    }

    const { totalCount } = await loadUsers();
    const totalPages = Math.ceil(totalCount / pageSize); // Use your totalCount variable
    if (page > totalPages) {       
        document.getElementById('pageInput').value = '';
        currentPage = page;
        return;
    }

    currentPage = page;
    loadUsers(false, false);
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

async function getUserById(userId) {    
    history.replaceState(null, '', window.location.pathname);
    if (!userId) {
        alert('Please enter a User ID.');
        return;
    }

    const url = `${apiBaseUrl}/api/v1/users/${userId}`;

    try {
        const response = await fetch(url);

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

showPanel(currentHtmlPage);

// all eventlisterners must have conditional checks since the dont exist in index.html (should have used separate .js for each html(?))
if(document.getElementById('loadUsers')){
    document.getElementById('loadUsers').addEventListener('click', () => {
        loadUsers(true, true);
    });
}

const getUserByIdButton = document.getElementById('getUserById')
if(getUserByIdButton){
    getUserByIdButton.addEventListener('click', async () => {
        const userId = document.getElementById('userIdInput').value.trim();
        getUserById(userId);
    })
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