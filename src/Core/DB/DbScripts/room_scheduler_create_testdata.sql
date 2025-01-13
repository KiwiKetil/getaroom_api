USE room_scheduler;

INSERT INTO Users (Id, FirstName, LastName, PhoneNumber, Email, HashedPassword, Salt)
VALUES 
('f47ac10b-58cc-4372-a567-0e02b2c3d471', 'Sarah', 'Connor', '22234354', 'sarah@example.com', 'hashedpassword456', 'somesaltpls'),
('d75a97de-58cc-4c56-bd74-95f12dbaf2da', 'John', 'Doe', '76354736', 'john.doe@example.com', 'hashedpassword123', 'salt123'),
('b47ac10b-58cc-4372-a567-0e02b2c3d473', 'Jane', 'Smith', '76530989', 'jane.smith@example.com', 'hashedpassword789', 'salt789'),
('c47ac10b-58cc-4372-a567-0e02b2c3d474', 'Emily', 'Jones', '66634567', 'emily.jones@example.com', 'hashedpassword456', 'salt456'),
('8b20ac67-80ff-4ac4-a5f4-564c0a254e30', 'Michael', 'Brown', '54567890', 'michael.brown@example.com', 'hashedpassword111', 'salt111'),
('6d7b1ca5-54f6-4859-a746-fc712d564128', 'Linda', 'Davis', '44458901', 'linda.davis@example.com', 'hashedpassword222', 'salt222'),
('f7d235c9-8e5f-4db1-a64a-bae96f3e2e1e', 'Chris', 'Miller', '33369012', 'chris.miller@example.com', 'hashedpassword333', 'salt333'),
('0eada259-0a95-4c55-9e87-888c1c45c601', 'Patricia', 'Wilson', '27890123', 'patricia.wilson@example.com', 'hashedpassword444', 'salt444'),
('69d8924d-f13d-44a5-b52f-6170c5cf702d', 'David', 'Moore', '11189012', 'david.moore@example.com', 'hashedpassword555', 'salt555'),
('e84ab3e6-5e7b-4c03-a8c0-155123f9f2e7', 'Jennifer', 'Taylor', '79012345', 'jennifer.taylor@example.com', 'hashedpassword666', 'salt666'),
('a47ac10b-58cc-4372-a567-0e02b2c3d481', 'Steven', 'Anderson', '30123456', 'steven.anderson@example.com', 'hashedpassword777', 'salt777'),
('b47ac10b-58cc-4372-a567-0e02b2c3d482', 'Laura', 'Thomas', '43212367', 'laura.thomas@example.com', 'hashedpassword888', 'salt888'),
('c47ac10b-58cc-4372-a567-0e02b2c3d483', 'Brian', 'Jackson', '54323478', 'brian.jackson@example.com', 'hashedpassword999', 'salt999'),
('d47ac10b-58cc-4372-a567-0e02b2c3d484', 'Lisa', 'White', '63456789', 'lisa.white@example.com', 'hashedpassword000', 'salt000'),
('e47ac10b-58cc-4372-a567-0e02b2c3d485', 'Kevin', 'Harris', '76545670', 'kevin.harris@example.com', 'hashedpassword1234', 'salt1234'),
('f57ac10b-58cc-4372-a567-0e02b2c3d486', 'James', 'Wilson', '55567890', 'james.wilson@example.com', 'hashedpassword001', 'salt001'),
('e67ac10b-58cc-4372-a567-0e02b2c3d487', 'Anna', 'Scott', '54456789', 'anna.scott@example.com', 'hashedpassword002', 'salt002'),
('d77ac10b-58cc-4372-a567-0e02b2c3d488', 'Peter', 'Brown', '53345678', 'peter.brown@example.com', 'hashedpassword003', 'salt003'),
('c87ac10b-58cc-4372-a567-0e02b2c3d489', 'Laura', 'Taylor', '52234567', 'laura.taylor@example.com', 'hashedpassword004', 'salt004'),
('b97ac10b-58cc-4372-a567-0e02b2c3d490', 'Sophie', 'Martin', '51123456', 'sophie.martin@example.com', 'hashedpassword005', 'salt005'),
('a07ac10b-58cc-4372-a567-0e02b2c3d491', 'Nathan', 'Harris', '50012345', 'nathan.harris@example.com', 'hashedpassword006', 'salt006'),
('997ac10b-58cc-4372-a567-0e02b2c3d492', 'Isabella', 'Moore', '48901234', 'isabella.moore@example.com', 'hashedpassword007', 'salt007'),
('887ac10b-58cc-4372-a567-0e02b2c3d493', 'Ethan', 'King', '47890123', 'ethan.king@example.com', 'hashedpassword008', 'salt008'),
('777ac10b-58cc-4372-a567-0e02b2c3d494', 'Emily', 'Allen', '46789012', 'emily.allen@example.com', 'hashedpassword009', 'salt009'),
('667ac10b-58cc-4372-a567-0e02b2c3d495', 'Oliver', 'Young', '45678901', 'oliver.young@example.com', 'hashedpassword010', 'salt010');

INSERT INTO UserRoles (UserId, RoleName)
VALUES    
    ('f47ac10b-58cc-4372-a567-0e02b2c3d471', "Admin");  
    
INSERT INTO Rooms (Id, RoomName, Capacity, MonitorAvailable)
VALUES
    ('123a4567-b89b-1263-a456-426614174001', 'Conference Room A', 20, 3),
    ('223b5678-c89b-1364-b567-426614174002', 'Meeting Room A', 10, 1),
    ('323c6789-d89d-1465-c678-426614174003', 'Meeting Room B', 8, 1),
    ('323c6789-d89d-1465-c678-426614174004', 'Meeting Room C', 6, 1),
    ('423d7890-c89c-1364-c567-426614174005', 'Meeting Room D', 4, 0),
    ('523e8901-a89a-1262-a456-426614174006', 'Meeting Room E', 6, 0),
    ('623b9012-b89b-1353-b678-426614174007', 'Meeting Room F', 10, 1),
    ('723c0123-d89d-1464-d567-426614174008', 'Board Room G', 12, 2);
    
INSERT INTO RoomReservations (Id, UserId, RoomId, StartTime, EndTime
) VALUES 
    ('11111111-2222-3333-4444-555555555555', 'f47ac10b-58cc-4372-a567-0e02b2c3d471', '123a4567-b89b-1263-a456-426614174001', '2025-01-15 10:00:00', '2025-01-15 11:00:00'),
    ('11111111-2222-3333-4444-555555555556', 'f57ac10b-58cc-4372-a567-0e02b2c3d486', '123a4567-b89b-1263-a456-426614174001', '2025-01-15 12:00:00', '2025-01-15 13:00:00'),
    ('11111111-2222-3333-4444-555555555557', 'e67ac10b-58cc-4372-a567-0e02b2c3d487', '223b5678-c89b-1364-b567-426614174002', '2025-01-16 10:00:00', '2025-01-16 11:00:00'),
    ('11111111-2222-3333-4444-555555555558', 'd77ac10b-58cc-4372-a567-0e02b2c3d488', '323c6789-d89d-1465-c678-426614174003', '2025-01-17 14:00:00', '2025-01-17 15:00:00'),
    ('11111111-2222-3333-4444-555555555559', 'c87ac10b-58cc-4372-a567-0e02b2c3d489', '323c6789-d89d-1465-c678-426614174004', '2025-01-18 09:00:00', '2025-01-18 10:00:00'),
    ('11111111-2222-3333-4444-555555555560', 'b97ac10b-58cc-4372-a567-0e02b2c3d490', '423d7890-c89c-1364-c567-426614174005', '2025-01-19 11:00:00', '2025-01-19 12:00:00'),
    ('11111111-2222-3333-4444-555555555561', 'a07ac10b-58cc-4372-a567-0e02b2c3d491', '523e8901-a89a-1262-a456-426614174006', '2025-01-20 13:00:00', '2025-01-20 14:00:00'),
    ('11111111-2222-3333-4444-555555555562', '997ac10b-58cc-4372-a567-0e02b2c3d492', '623b9012-b89b-1353-b678-426614174007', '2025-01-21 15:00:00', '2025-01-21 16:00:00'),
    ('11111111-2222-3333-4444-555555555563', '887ac10b-58cc-4372-a567-0e02b2c3d493', '723c0123-d89d-1464-d567-426614174008', '2025-01-22 08:00:00', '2025-01-22 09:00:00'),
    ('11111111-2222-3333-4444-555555555564', '777ac10b-58cc-4372-a567-0e02b2c3d494', '223b5678-c89b-1364-b567-426614174002', '2025-01-23 17:00:00', '2025-01-23 18:00:00'),
    ('11111111-2222-3333-4444-555555555565', '667ac10b-58cc-4372-a567-0e02b2c3d495', '323c6789-d89d-1465-c678-426614174004', '2025-01-24 10:00:00', '2025-01-24 11:00:00'),
    ('11111111-2222-3333-4444-555555555566', 'c87ac10b-58cc-4372-a567-0e02b2c3d489', '323c6789-d89d-1465-c678-426614174004', '2025-01-25 12:00:00', '2025-01-25 13:00:00'),
    ('11111111-2222-3333-4444-555555555567', '887ac10b-58cc-4372-a567-0e02b2c3d493', '223b5678-c89b-1364-b567-426614174002', '2025-01-26 14:00:00', '2025-01-26 15:00:00'),
    ('11111111-2222-3333-4444-555555555568', 'd77ac10b-58cc-4372-a567-0e02b2c3d488', '323c6789-d89d-1465-c678-426614174003', '2025-01-27 08:00:00', '2025-01-27 09:00:00'),
    ('11111111-2222-3333-4444-555555555569', 'f47ac10b-58cc-4372-a567-0e02b2c3d471', '323c6789-d89d-1465-c678-426614174004', '2025-01-28 09:00:00', '2025-01-28 10:00:00'),
    ('11111111-2222-3333-4444-555555555570', 'f47ac10b-58cc-4372-a567-0e02b2c3d471', '423d7890-c89c-1364-c567-426614174005', '2025-01-29 11:00:00', '2025-01-29 12:00:00'),    
    ('11111111-2222-3333-4444-555555555571', 'f57ac10b-58cc-4372-a567-0e02b2c3d486', '523e8901-a89a-1262-a456-426614174006', '2025-01-30 13:00:00', '2025-01-30 14:00:00'),
    ('11111111-2222-3333-4444-555555555572', '887ac10b-58cc-4372-a567-0e02b2c3d493', '623b9012-b89b-1353-b678-426614174007', '2025-01-31 15:00:00', '2025-01-31 16:00:00'),
    ('11111111-2222-3333-4444-555555555573', '667ac10b-58cc-4372-a567-0e02b2c3d495', '723c0123-d89d-1464-d567-426614174008', '2025-02-01 08:00:00', '2025-02-01 09:00:00'),
    ('11111111-2222-3333-4444-555555555574', '6d7b1ca5-54f6-4859-a746-fc712d564128', '423d7890-c89c-1364-c567-426614174005', '2025-02-02 17:00:00', '2025-02-02 18:00:00');
