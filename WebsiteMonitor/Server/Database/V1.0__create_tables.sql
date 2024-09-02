CREATE TABLE Users (
    UserID SERIAL PRIMARY KEY,
    GithubID INT NOT NULL,
    Email VARCHAR(120) NOT NULL,
    Username VARCHAR(120) NOT NULL
);

CREATE TABLE Websites (
    WebsiteID SERIAL PRIMARY KEY,
    UserID INT NOT NULL,
    Url VARCHAR(120) UNIQUE NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE MonitorLogs (
    MonitorLogID SERIAL PRIMARY KEY,
    WebsiteID INT NOT NULL,
    DateChecked TIMESTAMP NOT NULL,
    ResponseStatus INT NOT NULL,
    FOREIGN KEY (WebsiteID) REFERENCES Websites(WebsiteID)
);
