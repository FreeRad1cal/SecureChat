CREATE TABLE Users (
    Id VARCHAR(255) NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
	PRIMARY KEY (id)
);

CREATE TABLE Profiles (
    Id INT NOT NULL AUTO_INCREMENT,
    Age INT NOT NULL,
    Sex VARCHAR(1) NOT NULL,
	Location VARCHAR(255) NOT NULL,
	PRIMARY KEY (id)
);

CREATE TABLE UserProfileMap (
	UserId VARCHAR(255) NOT NULL,
	ProfileId INT NOT NULL,
	PRIMARY KEY (UserId, ProfileId),
	FOREIGN KEY (UserId) 
		REFERENCES Users(Id)
		ON DELETE CASCADE,
	FOREIGN KEY (ProfileId) 
		REFERENCES Profiles(Id)
		ON DELETE CASCADE
);