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
('e47ac10b-58cc-4372-a567-0e02b2c3d485', 'Kevin', 'Harris', '76545670', 'kevin.harris@example.com', 'hashedpassword1234', 'salt1234');

INSERT INTO UserRoles (UserId, RoleId)
VALUES 
    ('69d8924d-f13d-44a5-b52f-6170c5cf702d', 1),
    ('69d8924d-f13d-44a5-b52f-6170c5cf702d', 2),
    ('6d7b1ca5-54f6-4859-a746-fc712d564128', 2);
