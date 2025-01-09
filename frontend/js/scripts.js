const apiBaseUrl = 'https://localhost:7089';
let currentPage = 1; 
const pageSize = 10; 

function loadUsers(resetPage = false) {
    if (resetPage) {
        currentPage = 1;
        document.getElementById('prevPageButton').disabled = true; 
        document.getElementById('nextPageButton').disabled = false; 
        document.getElementById('goToPageButton').disabled = false; 
        document.getElementById('pageInput').disabled = false; 
    }

    const url = `${apiBaseUrl}/api/v1/users?page=${currentPage}&pageSize=${pageSize}`;

    fetch(url)
        .then(response => {
            if (response.status === 404) {
                alert('No users found');
                currentPage = Math.max(1, currentPage - 1); // Ensure currentPage does not go below 1
                document.getElementById('nextPageButton').disabled = true; 
                return [];
            }

            if (!response.ok) {
                throw new Error('Network response was not ok ' + response.statusText);
            }

            return response.json();
        })
        .then(data => {
            const tbody = document.querySelector('.allUsersTable tbody');
            tbody.innerHTML = ''; // Clear existing data

            if (data.length === 0) {
                alert('No users found.');       
                return;                 
            }

            data.forEach(user => {
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
            document.getElementById('goToPageButton').disabled = false;

            document.getElementById('currentPageContainer').style.visibility = 'visible';
            document.getElementById('currentPageDisplay').textContent = currentPage;

            console.log(`Loaded page ${currentPage}`);
        })
        .catch(error => {
            console.error('Error fetching users:', error);
            alert('Failed to load all users. Check the console for more details.');
        });
}

window.onload = function() {
    loadUsers(true); 
}

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

function getUserById() {
    const userId = document.getElementById('userIdInput').value.trim();
    if (!userId) {
        alert('Please enter a User ID.');
        return;
    }

    const url = `${apiBaseUrl}/api/v1/users/${userId}`;

    fetch(url)
        .then(response => {
            if (!response.ok) {
                if (response.status === 404) {
                    throw new Error('User not found.');
                }
                throw new Error('Network response was not ok ' + response.statusText);
            }
            return response.json();
        })
        .then(user => {
            const tbody = document.querySelector('.allUsersTable tbody');
            tbody.innerHTML = ''; // Clear existing data

            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${user.id.value}</td>
                <td>${user.lastName}</td>
                <td>${user.firstName}</td>
                <td>${user.phoneNumber}</td>
                <td>${user.email}</td>
            `;
            tbody.appendChild(row);

            resetPaginationForGetUserById(); 
        })
        .catch(error => {
            console.error('Error fetching user by ID:', error);
            alert(error.message);
            document.querySelector('.allUsersTable tbody').innerHTML = '';
        });
}    

document.getElementById('loadUsers').addEventListener('click', () => {
    loadUsers(true);
});

document.getElementById('getUserById').addEventListener('click', getUserById);

document.getElementById('prevPageButton').addEventListener('click', prevPage);
document.getElementById('nextPageButton').addEventListener('click', nextPage);
document.getElementById('goToPageButton').addEventListener('click', gotoPage);
        