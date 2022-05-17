create table users
(
    id INT UNIQUE AUTO_INCREMENT,
    username varchar(255) not null UNIQUE,
    password varchar(255) not null,
    role varchar(255),
    creation_date datetime,
    updated_date datetime,
    Primary key(id)
);

create table Address
(
    id INT UNIQUE AUTO_INCREMENT,
    road varchar(100) not null,
    postalCode varchar(30) not null,
    city varchar(50) not null,
    country varchar(50) not null,
    User INT  REFERENCES users(id),
    creationDate datetime,
    updatedDate datetime,
    Primary key(id),
    index(User)
);