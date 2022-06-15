CREATE table employeer (
    id int AUTO_INCREMENT NOT NULL PRIMARY KEY,
    Name varchar(64) NOT NULL,
    Created DateTime NOT NULL,
    UNIQUE (name)
    );
    
CREATE table employeer_payment (
    id int AUTO_INCREMENT NOT NULL PRIMARY KEY,
    EmployeerId int NOT NULL,
    TransferArrivalDate DateTime NOT NULL,
    Amount decimal(10,2) NOT NULL,
    FOREIGN KEY (EmployeerId) REFERENCES employeer (Id)
    );
    
CREATE table job_item (
    Id int AUTO_INCREMENT NOT NULL PRIMARY KEY,
    EmployeerId int NOT NULL,
    Title varchar(128) NOT NULL,
    Description varchar(512),
    Website varchar(256),
    IsCompleted bit NOT NULL DEFAULT(0),
    Price decimal NOT NULL,
    StartDate datetime NOT NULL,
    EndDate datetime,
    FOREIGN KEY (EmployeerId) REFERENCES employeer (Id)
    );