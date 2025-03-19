USE getaroom;

-- ===========================
-- Section: Create Tables
-- ===========================

CREATE TABLE IF NOT EXISTS Users
(
Id CHAR(36) PRIMARY KEY,
FirstName VARCHAR (30) NOT NULL,
LastName VARCHAR (30) NOT NULL,
PhoneNumber VARCHAR (15) NOT NULL,
Email VARCHAR (50) NOT NULL,
HashedPassword VARCHAR(255) COLLATE utf8mb4_bin NOT NULL,
Created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
Updated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Rooms
(
Id CHAR(36) PRIMARY KEY,
RoomName VARCHAR (30) NOT NULL,
Capacity TINYINT NOT NULL,
MonitorAvailable BOOL NOT NULL,
Created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
Updated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS RoomReservations
(
Id CHAR(36) PRIMARY KEY,
UserId CHAR(36) NOT NULL,
RoomId CHAR(36) NOT NULL,
StartTime TIMESTAMP NOT NULL,
EndTime TIMESTAMP NOT NULL,
Created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
Updated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
CONSTRAINT FK_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE ON UPDATE CASCADE,
CONSTRAINT FK_Room FOREIGN KEY (RoomId) REFERENCES Rooms(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- ===========================
-- Section: Create and Insert Initial Data Roles Table
-- ===========================

CREATE TABLE IF NOT EXISTS Roles
(
RoleName VARCHAR(20) NOT NULL UNIQUE PRIMARY KEY
);

INSERT INTO Roles (RoleName)
SELECT 'Admin'
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Admin');

INSERT INTO Roles (RoleName)
SELECT 'Client'
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Client');

INSERT INTO Roles (RoleName)
SELECT 'Employee'
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Employee');

CREATE TABLE IF NOT EXISTS UserRoles
(
UserId CHAR(36) NOT NULL,
RoleName CHAR(20) NOT NULL,
Created TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
Updated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
PRIMARY KEY (UserId, RoleName),
FOREIGN KEY (UserId) REFERENCES Users(Id)  ON DELETE CASCADE,
FOREIGN KEY (RoleName) REFERENCES Roles(RoleName)  ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS DeletedUsers (
    UserId CHAR(36) NOT NULL,
    FirstName VARCHAR(30) NOT NULL,
    LastName VARCHAR(30) NOT NULL,
    PhoneNumber VARCHAR(15) NOT NULL,
    Email VARCHAR(50) NOT NULL,
    DeletedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    DeletedBy VARCHAR(25) NOT NULL,
    UserRoles JSON NOT NULL,
    PRIMARY KEY (UserId)
);

CREATE TABLE IF NOT EXISTS PasswordHistory (
	Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    ChangedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- ===========================
-- Section: Create Procedure: Aggregate UserRoles as JSON
-- ===========================

DELIMITER $$
CREATE PROCEDURE GetUserRolesAsJSON(IN p_userId CHAR(36), OUT roles JSON)
BEGIN
    SELECT IFNULL(JSON_ARRAYAGG(JSON_OBJECT('RoleName', RoleName)), '[]') -- if !null return value, if null return []
    INTO roles 
    FROM UserRoles
    WHERE UserId = p_userId; 
END$$
DELIMITER ;

-- ===========================
-- Section: Create Trigger: Move Users to DeletedUsers table upon delete
-- ===========================

DELIMITER $$
CREATE TRIGGER before_users_delete_archive
BEFORE DELETE ON Users
FOR EACH ROW
BEGIN
    DECLARE roles JSON; -- Declare the roles variable to hold JSON output
    
    -- Call the procedure to get roles as JSON
    CALL GetUserRolesAsJSON(OLD.Id, roles);
    
    INSERT INTO DeletedUsers (
        UserId, FirstName, LastName, PhoneNumber, Email, DeletedAt, DeletedBy, UserRoles
    )
    VALUES (
        OLD.Id,
        OLD.FirstName,
        OLD.LastName,
        OLD.PhoneNumber,
        OLD.Email,
        CURRENT_TIMESTAMP,
        CASE 
            WHEN SESSION_USER() LIKE 'YOUR_USERNAME@%' THEN 'API'
            ELSE 'DB'
        END,
        roles -- Use the aggregated JSON value
    );
END$$
DELIMITER ;
