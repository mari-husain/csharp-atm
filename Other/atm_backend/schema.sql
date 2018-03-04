DROP TABLE if exists Accounts;
DROP TABLE if exists Atm;
CREATE TABLE Accounts(
	Id integer primary key autoincrement,
	FirstName text not null,
	LastName text not null,
	Balance real not null,
	PINHash text not null
);

CREATE TABLE Atm(
	Id integer primary key autoincrement,
	LastUpdated text not null,
	Pennies int not null,
	Nickels int not null,
	Dimes int not null,
	Quarters int not null,
	Ones int not null,
	Fives int not null,
	Tens int not null,
	Twenties int not null,
	Fifties int not null
);

