To test this API:

1. Run the 3 scripts in the `RoomSchedulerAPI/Core/DB/DBScripts` folder in MySQL: 
   - First, run `create_db.sql` set YOUR_PASSWORD and YOUR_USERNAME (default is YOUR_PASSWORD and YOUR_USERNAME)
   - Then, run `room_scheduler_create_tables.sql`
   - Finally, run `room_scheduler_create_testusers.sql`

2. Set the username and password as environment variables:
   - `ROOM_DB_USER`: set this to the username created in `create_db.sql`
   - `ROOM_DB_PASSWORD`: set this to the password for that user
