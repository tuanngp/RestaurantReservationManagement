create database RestaurantReservationDB;
use RestaurantReservationDB;

create table Users (
	ID int primary key identity,
	FullName nvarchar(100) not null,
	Email nvarchar(100) unique not null,
	Password nvarchar(255) not null,
	Role nvarchar(20) CHECK (Role IN ('Admin', 'Customer')) NOT NULL,
	CreatedAt datetime default GETDATE()
)

create table Tables (
	ID int primary key identity,
	TableNumber int unique not null,
	Seats int CHECK (Seats >= 0) not null,
	Status nvarchar(50) Check (Status in ('Available', 'Reserved', 'Occupied')) NOT NULL,
	CreatedAt DATETIME DEFAULT GETDATE()
)

create table Reservations (
	ID int primary key identity(1,1),
	UserID int foreign key references Users(ID),
	TableID int foreign key references Tables(ID),
	ReservationDate datetime DEFAULT GETDATE() CHECK(ReservationDate >= GETDATE()),
	Status nvarchar(50) Check (Status in ('Pending', 'Confirmed', 'Cancelled')) NOT NULL,
	CreatedAt DATETIME DEFAULT GETDATE()
)