-- SQL Server init script

-- Create the AddressBook database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'AddressBook')
BEGIN
  CREATE DATABASE codele;
END;
GO

USE codele;
GO

-- Create the Words table
IF OBJECT_ID(N'Words', N'U') IS NULL
BEGIN
    CREATE TABLE Words
    (
        Id        INT PRIMARY KEY IDENTITY(1,1) ,
        Answer VARCHAR(5) NOT NULL
    );
END;
GO

-- Insert some sample data into the Words table
IF (SELECT COUNT(*) FROM Words) = 0
BEGIN
    INSERT INTO Words (Answer)
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