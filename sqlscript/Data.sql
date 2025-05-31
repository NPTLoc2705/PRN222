-- Insert Users
INSERT INTO Users (Id, FullName, Role, Email, PhoneNumber, Address, Password, IsActive)
VALUES 
  (NEWID(), 'Alice Johnson', 0, 'alice.johnson@example.com', '1234567890', '123 Main St', 'hashedpassword1', 1),
  (NEWID(), 'Bob Smith', 1, 'bob.smith@example.com', '2345678901', '456 Maple Ave', 'hashedpassword2', 1),
  (NEWID(), 'Carol White', 2, 'carol.white@example.com', '3456789012', '789 Oak Blvd', 'hashedpassword3', 1),
  (NEWID(), 'David Brown', 3, 'david.brown@example.com', '4567890123', '321 Pine Rd', 'hashedpassword4', 1),
  (NEWID(), 'Eve Black', 0, 'eve.black@example.com', '5678901234', '654 Cedar Ln', 'hashedpassword5', 1);

-- Declare variables to hold user IDs
DECLARE @UserId1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE FullName = 'Alice Johnson');
DECLARE @UserId2 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE FullName = 'Bob Smith');
DECLARE @UserId3 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE FullName = 'Carol White');
DECLARE @UserId4 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE FullName = 'David Brown');
DECLARE @UserId5 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Users WHERE FullName = 'Eve Black');

-- Insert Posts
INSERT INTO Posts (Id, Title, Content, CreatedAt, UpdatedAt, StaffId, IsPublished)
VALUES
  (NEWID(), 'Post One', 'Content of post one.', GETUTCDATE(), GETUTCDATE(), @UserId1, 1),
  (NEWID(), 'Post Two', 'Content of post two.', GETUTCDATE(), GETUTCDATE(), @UserId2, 0),
  (NEWID(), 'Post Three', 'Content of post three.', GETUTCDATE(), GETUTCDATE(), @UserId3, 1),
  (NEWID(), 'Post Four', 'Content of post four.', GETUTCDATE(), GETUTCDATE(), @UserId4, 0),
  (NEWID(), 'Post Five', 'Content of post five.', GETUTCDATE(), GETUTCDATE(), @UserId5, 1);
		
