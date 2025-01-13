# To Test This API:

1. Execute the 3 scripts in the `RoomSchedulerAPI/Core/DB/DBScripts` folder in MySQL as follows:

   - Open `create_db.sql`. Before executing, ensure you update the script with your desired `YOUR_USERNAME` and `YOUR_PASSWORD`.
   - Execute `room_scheduler_create_tables.sql`.
   - Execute `room_scheduler_create_testdata.sql`.

2. Set the username and password you chose as environment variables:

   - **ROOM_DB_USER**: set this to the username created in `create_db.sql`.

   - **ROOM_DB_PASSWORD**: set this to the password for that user.
