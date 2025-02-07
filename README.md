# To Test This API:

1. Execute the 3 scripts in the `RoomSchedulerAPI/Core/DB/DBScripts` folder in MySQL as follows:

   - Open `room_scheduler_create_db.sql`. Before executing the script, update the script with your desired `YOUR_USERNAME` and `YOUR_PASSWORD`.
   - Execute `room_scheduler_create_tables.sql`.
   - Execute `room_scheduler_create_testdata.sql`.

2. Set the username and password you chose in the `room_scheduler_create_db.sql` as environment variables:

   - **ROOM_DB_USER**: set this to the username created in `room_scheduler_create_db.sql`.

   - **ROOM_DB_PASSWORD**: set this to the password chosen in `room_scheduler_create_db.sql`.
  
3. Set User secrets for JWT Key. Run these commands in API base path:
   - dotnet user-secrets init   
   - dotnet user-secrets set "Jwt:Key" "YourLocalSecretKeyOfAtLeast256Bits"

