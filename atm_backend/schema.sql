DROP TABLE if exists Accounts;
CREATE TABLE Accounts(
	Id integer primary key autoincrement,
	FirstName text not null,
	LastName text not null,
	Balance real not null,
	PIN int not null
);

INSERT INTO Accounts (FirstName, LastName, Balance, PIN)
VALUES ('Mari', 'Husain', 150.00, 1234);

INSERT INTO Accounts (FirstName, LastName, Balance, PIN)
VALUES ('Eric', 'Chase', 100.00, 1235);

INSERT INTO Accounts (FirstName, LastName, Balance, PIN)
VALUES ('Andrew', 'Lee', 50.00, 4321);