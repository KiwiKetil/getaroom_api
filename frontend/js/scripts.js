const apiBaseUrl = 'https://localhost:7089';
let currentPage = 1;
const pageSize = 10;

async function loadUsers(resetPage = false) {
    if (resetPage) {
        currentPage = 1;
        document.getElementById('prevPageButton').disabled = true;
        document.getElementById('nextPageButton').disabled = false;
        document.getElementById('goToPageButton').disabled = false;
        document.getElementById('pageInput').disabled = false;
        document.getElementById('userIdInput').value = '';
    }

    const url = `${apiBaseUrl}/api/v1/users?page=${currentPage}&pageSize=${pageSize}`;

    try {
        const response = await fetch(url);

        if (response.status === 404) {
            alert('No users found');
            currentPage = Math.max(1, currentPage - 1); // Keep currentPage valid
            document.getElementById('nextPageButton').disabled = true;
            return;
        }

        if (!response.ok) {
            throw new Error(`Network response was not ok: ${response.statusText}`);
        }

        const data = await response.json();
        if (!data || data.length === 0) {
            alert('No users found.');
            return;
        }

        populateTable(data);

        // Make buttons and table visible
        const buttons = document.querySelectorAll('button');
        buttons.forEach(button => {
            button.style.visibility = 'visible';
        });

        const input = document.querySelectorAll('input');
        input.forEach(input => {
            input.style.visibility = 'visible';
        });

        document.querySelector('.allUsersTable').style.visibility = 'visible';

        document.getElementById('prevPageButton').disabled = currentPage === 1;
        document.getElementById('nextPageButton').disabled = data.length < pageSize;
        document.getElementById('currentPageContainer').style.visibility = 'visible';
        document.getElementById('currentPageDisplay').textContent = currentPage;

        console.log(`Loaded page ${currentPage}`);
    } catch (error) {
        console.error('Error fetching users:', error);
        alert('Failed to load all users. Check the console for more details.');
    }
}

window.onload = function () {
    loadUsers(true);
};

function nextPage() {
    currentPage++;
    loadUsers();
}

function prevPage() {
    if (currentPage > 1) {
        currentPage--;
        loadUsers();
    } else {
        alert('You are already on the first page!');
    }
}

function gotoPage() {
    const pageInput = document.getElementById('pageInput').value;
    const page = parseInt(pageInput);

    if (isNaN(page) || page <= 0) {
        alert('Please enter a valid page number.');
        return;
    }
    currentPage = page;
    loadUsers();
    document.getElementById('pageInput').value = '';
}

function resetPaginationForGetUserById() {
    document.getElementById('prevPageButton').disabled = true;
    document.getElementById('nextPageButton').disabled = true;
    document.getElementById('goToPageButton').disabled = true;
    document.getElementById('pageInput').disabled = true;
    document.getElementById('currentPageDisplay').textContent = '1';
}

async function getUserById(userId) {    
    if (!userId) {
        alert('Please enter a User ID.');
        return;
    }

    const url = `${apiBaseUrl}/api/v1/users/${userId}`;

    try {
        const response = await fetch(url);

        if (!response.ok) {
            if (response.status === 404) {
                throw new Error('User not found.');
            }
            throw new Error(`Network response was not ok: ${response.statusText}`);
        }

        const data = await response.json();

        populateTable(data);
      
        resetPaginationForGetUserById();
    } catch (error) {
        console.error('Error fetching user by ID:', error);
        alert(error.message);
        document.querySelector('.allUsersTable tbody').innerHTML = '';
    }
}

function populateTable(data) {
    const tbody = document.querySelector('.allUsersTable tbody');
        tbody.innerHTML = ''; // Clear existing data

        const users = Array.isArray(data) ? data : [data]; // handle both single user and array of users

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

document.getElementById('loadUsers').addEventListener('click', () => {
    loadUsers(true);
});

getUserByIdButton = document.getElementById('getUserById')
getUserByIdButton.addEventListener('click', async () => {
    const userId = document.getElementById('userIdInput').value.trim();
    getUserById(userId);
})

document.getElementById('prevPageButton').addEventListener('click', prevPage);
document.getElementById('nextPageButton').addEventListener('click', nextPage);
document.getElementById('goToPageButton').addEventListener('click', gotoPage);
