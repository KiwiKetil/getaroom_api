DROP DATABASE IF EXISTS room_scheduler;
CREATE DATABASE room_scheduler;
USE room_scheduler;

# create user
CREATE USER IF NOT EXISTS 'room_scheduler-app'@'localhost' IDENTIFIED BY 'room_scheduler123';
CREATE USER IF NOT EXISTS 'room_scheduler-app'@'%' IDENTIFIED BY 'room_scheduler123';
 
 # grant access:
GRANT ALL privileges ON room_scheduler.* TO 'room_scheduler-app'@'%';
GRANT ALL privileges ON room_scheduler.* TO 'room_scheduler-app'@'localhost';
FLUSH PRIVILEGES