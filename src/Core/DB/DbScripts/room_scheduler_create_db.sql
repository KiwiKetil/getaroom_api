DROP DATABASE IF EXISTS room_scheduler;
CREATE DATABASE room_scheduler;
USE room_scheduler;

# create user
CREATE USER IF NOT EXISTS 'YOUR_USERNAME'@'localhost' IDENTIFIED BY 'YOUR_PASSWORD';
CREATE USER IF NOT EXISTS 'YOUR_USERNAME'@'%' IDENTIFIED BY 'YOUR_PASSWORD';
 
 # grant access:
GRANT ALL privileges ON room_scheduler.* TO 'YOUR_USERNAME'@'%';
GRANT ALL privileges ON room_scheduler.* TO 'YOUR_USERNAME'@'localhost';
FLUSH PRIVILEGES