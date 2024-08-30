-- SQL Server init script

-- Create the AddressBook database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'AddressBook')
BEGIN
  CREATE DATABASE codele;
END;
GO

USE words;
GO

-- Create the Contacts table
IF OBJECT_ID(N'words', N'U') IS NULL
BEGIN
    CREATE TABLE Contacts
    (
        Id        INT PRIMARY KEY IDENTITY(1,1) ,
        Answer VARCHAR(5) NOT NULL
    );
END;
GO

-- Insert some sample data into the Contacts table
IF (SELECT COUNT(*) FROM Contacts) = 0
BEGIN
    INSERT INTO Contacts (FirstName, LastName, Email, Phone)
    VALUES
        ('write'),
        ('cobol'), 
        ('coder'),
        ('array'),
        ('false'),
        ('build'),
        ('table'),
        ('techy'),
        ('razor'),
        ('azure'),
        ('agile'),
        ('cloud'),
        ('serve'),
        ('debug');
END;
GO