USE room_scheduler;

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
HashedPassword VARCHAR(255) NOT NULL,
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

CREATE TABLE IF NOT EXISTS Roles
(
RoleName VARCHAR(20) NOT NULL UNIQUE PRIMARY KEY
);

-- ===========================
-- Section: Insert Initial Data Roles Table
-- ===========================

INSERT INTO Roles (RoleName)
SELECT 'Admin'
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Admin');

INSERT INTO Roles (RoleName)
SELECT 'User'
WHERE NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'User');

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
    FirstName VARCHAR(30),
    LastName VARCHAR(30),
    PhoneNumber VARCHAR(15),
    Email VARCHAR(50),
    DeletedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    DeletedBy VARCHAR(25) DEFAULT 'SQLAdmin',
    UserRoles JSON NOT NULL, -- Store the roles as JSON
    PRIMARY KEY (UserId)
);

-- ===========================
-- Section: Create Procedure: Aggregate UserRoles as JSON
-- ===========================

DELIMITER $$

CREATE PROCEDURE GetUserRolesAsJSON(IN p_userId CHAR(36), OUT roles JSON)
BEGIN
    SELECT IFNULL(JSON_ARRAYAGG(JSON_OBJECT('RoleName', RoleName)), '[]')
    INTO roles
    FROM UserRoles
    WHERE UserId = p_userId; -- was behind scenes: WHERE UserRoles.UserId = UserRoles.UserId; when UserID = userID bec takes presedence. 
    -- The issue was that both UserIds in your WHERE clause were referring to the UserId column in the UserRoles table, not comparing the parameter UserId to the column UserId
    -- This type of error is a well-known issue in SQL programming but can be overlooked. 
    --  it's a best practice to use naming conventions that differentiate parameters from column names 
    -- Use Distinct Names: Always ensure that procedure parameters and local variables have names that do not conflict with column names in your database.
    -- Adopt Naming Conventions: Prefix parameter names with p_, in_, or another identifier to differentiate them from column names.
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
            WHEN SESSION_USER() LIKE 'room_scheduler-app@%' THEN 'API'
            ELSE 'Admin'
        END,
        roles -- Use the aggregated JSON value
    );
END$$

DELIMITER ;

-- ===========================
-- Section: Create Trigger: Assign role to user upon registration
-- ===========================

DELIMITER $$

CREATE TRIGGER assign_role_new_user
AFTER INSERT ON Users
FOR EACH ROW
BEGIN
	INSERT INTO userroles (RoleName, UserId)
    VALUES ('User', NEW.Id);
END$$

DELIMITER ;




