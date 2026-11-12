--Delete the CityInfoDB (City Information Database) database if it exists. 
IF EXISTS(SELECT * from sys.databases WHERE name='CityInfoDB') 
BEGIN 
    DROP DATABASE CityInfoDB; 
END 

CREATE DATABASE CityInfoDB;
GO

USE CityInfoDB
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE dbo.CityInfo (
    CityId      INT            IDENTITY (1, 1) PRIMARY KEY,
    CityName    NVARCHAR (50)  NOT NULL,
    Description NVARCHAR (500) NULL
);
GO

insert into CityInfo(CityName,Description) values('New York City', 'the city with world finance center');
insert into CityInfo(CityName,Description) values('Paris', 'the captital city of France');
insert into CityInfo(CityName,Description) values('Sydney', 'A city is best known for its harbourfront Sydney Opera House');
insert into CityInfo(CityName,Description) values('Beijing', 'Capital city of China, it is the nation political, economic, and cultural center');
insert into CityInfo(CityName,Description) values('Toronto', 'The ciptial city of Ontario province');



CREATE TABLE dbo.PointsOfInterest (
    PointsOfInterestId          INT      IDENTITY (1, 1) PRIMARY KEY,
    NameofPOI        NVARCHAR (50)  NOT NULL,
    Description NVARCHAR (1000) NULL,
    CityId      INT            NOT NULL
    CONSTRAINT FK_PointsOfInterest_Cities_CityId FOREIGN KEY ([CityId]) REFERENCES CityInfo (CityId) ON DELETE CASCADE
    );
    
 Go
 

 
 
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Central Park', 'The most visited urban park in the United States.',1);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Empire State Building', 'A 102-story skyscraper located in Midtown Manhattan.',1);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Eiffel Tower', 'A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel.',2);
  insert into PointsOfInterest(NameofPOI,Description,CityId) values('The Louvre', 'The world''s largest museum.',2);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('United Nations', 'The United Nations is headquartered in New York City. A one-hour guided tour offers an exciting opportunity to discover UN Headquarters. ',1);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('The Metropolitan Museum of Art', 'The Metropolitan Museum of Art of New York City is the largest art museum in the United States. It was the third most visited art museum in the world',1);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Sydney Opera House', 'The Sydney Opera House is a multi-venue performing arts centre at Sydney Harbour in Sydney, Australia. It is one of the 20th century''s most famous and distinctive buildings',3);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Bondi Beach', 'The sweeping white-sand crescent of Bondi is one of Australia most iconic beaches.',3);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Forbidden City', 'a palace complex in central Beijing, China. It houses the Palace Museum, and was the former Chinese imperial palace from the Ming dynasty to the end of the Qing dynasty',4);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('The Great Wall of China', 'a fortification system was built to protect and consolidate territories of Chinese states and empires against various nomadic groups of the steppe and their polities',4);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('CN tower', 'CN Tower is beautiful',5);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Royal Ontario Museum', 'The Royal Ontario Museum is a museum of art, world culture and natural history in Toronto, Ontario, Canada. It is one of the largest museums in North America and the largest in Canada. It attracts more than one million visitors every year, making the ROM the most-visited museum in Canada. ',5);
 insert into PointsOfInterest(NameofPOI,Description,CityId) values('Toronto Islands', 'The Toronto Islands are a chain of 15 small islands in Lake Ontario, south of mainland Toronto, Ontario, Canada. Comprising the only group of islands in the western part of Lake Ontario, the Toronto Islands are located just offshore from the city''s downtown and provide shelter for Toronto Harbour. The island community is considered to be the largest urban car-free community in North America, although some service vehicles are permitted. Access to the Islands is by ferry, including the City of Toronto ferries operating from Jack Layton Ferry Terminal at the foot of Bay Street, or by water taxis. The Islands is a popular recreation destination. Recreational bicyclists are accommodated on the ferries. Bicycles can be rented at Centre Island. Canoes, kayaks and paddle boats can also be rented on the island. A disc golf course exists on the island. The main beach is along the south shore and the beach on the west shore is clothing-optional.',5);