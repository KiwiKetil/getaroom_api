![image](https://github.com/user-attachments/assets/950f7979-d675-41cd-894a-4c5c12621369)

## API is undergoing big changes and will not work as expected at the moment, please come back at a later time

### To Test This API

#### 1. Execute Database Scripts

Execute the 3 scripts in the `GetARoomAPI/Core/DB/DBScripts` folder in MySQL as follows:

1. **Open `getaroom_create_db.sql`:**  
   Before executing this script in MySQL, update it with your desired credentials by replacing the placeholders with your chosen values for `YOUR_USERNAME` and `YOUR_PASSWORD`.

2. **Execute `getaroom_create_tables.sql`:**  
   This script will create the necessary tables in the database.

3. **Execute `getaroom_create_testdata.sql`:**  
   This script will insert sample test data into the database.
   
   Note: All test user passwords are their first name followed by 123!
   For example, if the user's first name is "John", the password is John123!

   Note: Several Endpoints require that password has been changed from the initial password! Follow UpdatePassword endpoint to get new tokenstring.

#### 2. Configure Environment Variables for Database Credentials

Set the following environment variables using the values you specified in `getaroom_create_db.sql`:

- **GETAROOM_DB_USER:**  
  Set this to the username you configured.

- **GETAROOM_DB_PASSWORD:**  
  Set this to the corresponding password.

#### 3. Configure JWT User Secrets (Development Only)

1. Open a terminal in the API’s base folder (the folder containing your project’s `.csproj` file).

2. Run the following commands:

   - dotnet user-secrets init
   - dotnet user-secrets set "Jwt:Key" "YourLocalSecretKeyOfAtLeast256Bits" 


