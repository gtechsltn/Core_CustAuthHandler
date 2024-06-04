Create Database AppSecurity;

Use AppSecurity;

Create Table Users (
  UserId UNIQUEIDENTIFIER PRIMARY KEY default NEWID(),
  FirstName Varchar(100) Not Null,
  LastName  Varchar(100) Not Null,
  Email varchar(100) Not Null,
  UserName varchar(100) Not Null Unique,
  [Password] varchar(20) Not Null 
);

Insert into Users (FirstName, LastName,Email,UserName, Password)
Values (
  'Mahesh', 'Sabnis', 'mahesh.sabnis@myemail.com','mahsab','P@ssw0rd_'
);

Select * from Users;