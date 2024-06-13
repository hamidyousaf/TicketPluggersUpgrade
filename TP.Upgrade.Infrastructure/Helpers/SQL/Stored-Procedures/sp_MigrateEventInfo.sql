CREATE OR ALTER PROCEDURE [dbo].[MigrateEventInfo]
AS
BEGIN
	DECLARE @lastinsertedid int;
	DECLARE @rowcount int;
	SET @lastinsertedid = 0 ;
	SET @rowcount = 0

	/* insert into venue table */
	DELETE FROM EventInfoTemp where SOURCE ='Undefined' OR EVENT_START_DATETIME IS NULL
	INSERT INTO Venues 
		(Venue_ID, VenueName, Street,City,StateCode,CountryCode,Latitude, Longitude,ZipCode,Timezone,VenueURL,VenueImage,Active,CreatedBy,CreatedDate)
	SELEct DISTINct 
		VENUE_ID,VENUE_NAME,VENUE_STREET,VENUE_CITY,VENUE_STATE_CODE,VENUE_COUNTRY_CODE,CAST(VENUE_LATITUDE AS float) AS VENUE_LATITUDE ,
		CAST(VENUE_LONGITUDE AS float) AS VENUE_LONGITUDE ,VENUE_ZIP_CODE,VENUE_TIMEZONE,VENUE_URL, VENUE_URL, 1,'',GETUTCDATE()
	FROM EventInfoTemp eit
	WHERE NOT EXISTS (
		SELEct 1 FROM Venues ev 
			WHERE ev.Venue_ID = eit.VENUE_ID) 
	AND VENUE_ID != '';
	
	/* insert venues whose venue_id is not present */
	INSERT INTO 
		Venues(Venue_ID, VenueName, Street,City,StateCode,CountryCode,Latitude, Longitude,ZipCode,Timezone,VenueURL,VenueImage,Active,CreatedBy,CreatedDate)
	SELEct DISTINct  '',VENUE_NAME,VENUE_STREET,VENUE_CITY,VENUE_STATE_CODE,VENUE_COUNTRY_CODE,VENUE_LATITUDE,VENUE_LONGITUDE,
			VENUE_ZIP_CODE,VENUE_TIMEZONE, COALESCE(VENUE_URL, ''), VENUE_URL, 1,'',GETUTCDATE()
	FROM EventInfoTemp eit
	WHERE VENUE_ID = '' 
	AND NOT EXISTS 
		(SELEct 1 FROM Venues ev 
			WHERE ev.Street = eit.VENUE_Street AND  ev.City = eit.VENUE_City AND ev.Longitude = eit.VENUE_LONGITUDE
			AND ev.Latitude = eit.VENUE_LATITUDE AND ev.CountryCode = eit.VENUE_Country_Code  AND  ev.ZipCode = eit.VENUE_ZIP_CODE AND VENUE_ID = '' );

	/* insert into Categories table */
	INSERT INTO 
		Categories(Name,Description,MetaKeywords,ShowOnHomepage,ParentCategoryId,ChildLevel,IncludeInTopMenu,DisplayOrder,Published,
		CreatedDate,UpdatedDate,CategoryTemplateId,MetaTitle,LimitedToStores,ImageURL,CreatedBy)
	SELEct DISTINct CLASSIFICATION_GENRE, '',CLASSIFICATION_GENRE,1,COALESCE(c.Id ,4),1,1,4,1,GETUTCDATE(),GETUTCDATE(),1,'',0,'', ''
	FROM EventInfoTemp eit
		INNER JOIN Categories c ON eit.CLASSIFICATION_SEGMENT = c.MetaKeywords and c.ParentCategoryId = 0
	WHERE (CLASSIFICATION_GENRE != '' or  CLASSIFICATION_GENRE != 'Undefined') 
		AND NOT EXISTS
		(SELEct 1 FROM Categories t 
			WHERE eit.CLASSIFICATION_GENRE = t.MetaKeywords 
				AND t.ChildLevel = 1 AND c.ParentCategoryId = 0)

	INSERT INTO Categories 
		(Name,Description,MetaKeywords,ShowOnHomepage,ParentCategoryId,ChildLevel,IncludeInTopMenu,DisplayOrder,Published,CreatedDate,UpdatedDate,
		CategoryTemplateId,MetaTitle,LimitedToStores,ImageURL,CreatedBy)
	SELEct DISTINct CLASSIFICATION_SUB_GENRE, '',CLASSIFICATION_SUB_GENRE,1,c.Id,2,1,4,1,GETUTCDATE(),GETUTCDATE(),1,'',0,'',''
	FROM EventInfoTemp eit
		INNER JOIN Categories c ON eit.CLASSIFICATION_GENRE = c.MetaKeywords 
			AND c.ParentCategoryId != 0
	WHERE NOT EXISTS
		(SELEct 1 FROM Categories t 
			WHERE eit.CLASSIFICATION_SUB_GENRE = t.MetaKeywords and  t.ChildLevel = 2)
	AND CLASSIFICATION_SUB_GENRE != ''

	CREATE TABLE CategoryTmp 
		(lev1 nvarchar(max),level1Id bigint,lev2 nvarchar(max),level2Id bigInt, lev3 nvarchar(max), level3Id bigint) 
	INSERT INTO CategoryTmp
		SELEct t1.MetaKeywords AS lev1, t1.Id as level1Id, t2.name as lev2, t2.Id as level2Id, t3.name as lev3 , t3.Id as level3Id
	FROM Categories AS t1
		LEFT JOIN Categories AS t2 ON t2.ParentCategoryId = t1.Id 
		LEFT JOIN Categories AS t3 ON t3.ParentCategoryId = t2.Id
	WHERE t1.ParentCategoryId = 0;

	CREATE TABLE CategoryMisctmp 
		(lev1 nvarchar(max),level1Id bigint,lev2 nvarchar(max),level2Id bigInt, lev3 nvarchar(max), level3Id bigint) 
	INSERT INTO CategoryMisctmp
	SELEct * FROM CategoryTmp 
	WHERE level1Id IN (SELEct Id FROM Categories WHERE Name = 'Miscellaneous' AND ParentCategoryId = 0);

	SET TRANSActION ISOLATION LEVEL READ UNCOMMITTED;
	/* insert into Categories table */
	INSERT INTO Events (Event_ID, Name, Description,Notes,EventStatusId, EventStartUTC,EventEndUTC,
	EventStartTime,OnSaleStartDateTime,OnSaleEndDateTime,ImageURL,IsHotEvent,Published,CreatedDate,
	UpdatedDate,VenueID,DisplayOrder,SegmentId,GenreId,SubGenreId,CurrencyCode,SearchName,TimeZone,
	IsFeatured,EventSearchCount,CreatedBy)
	SELEct 
		eit.EVENT_ID,
		EVENT_NAME,
		EVENT_INFO,
		EVENT_NOTES,
		(CASE
			WHEN EVENT_STATUS = 'onsale' THEN 1
			WHEN EVENT_STATUS = 'rescheduled' THEN 2
			WHEN EVENT_STATUS = 'postponed' THEN 3
			WHEN EVENT_STATUS = 'cancelled' THEN 4
			ELSE 5
		END),
		EVENT_START_DATETIME,
		EVENT_END_DATETIME,
		--EVENT_START_LOCAL_DATE,
		--(CASE WHEN EVENT_START_LOCAL_DATE='0001-01-01 00:00:00' THEN EVENT_START_DATETIME
	   -- ELSE EVENT_START_LOCAL_DATE END),
		EVENT_START_LOCAL_TIME,
		ONSALE_START_DATETIME,
		ONSALE_END_DATETIME,
		EVENT_IMAGE_URL,
		CASE
			WHEN HOT_EVENT = 'true' THEN 1
			ELSE 0
		END,
		 (CASE
			WHEN EVENT_STATUS = 'onsale' THEN 1
			WHEN EVENT_STATUS = 'rescheduled' THEN 1
			WHEN EVENT_STATUS = 'postponed' THEN 1
			WHEN EVENT_STATUS = 'cancelled' THEN 0
			ELSE 0
		END),GETUTCDATE(),GETUTCDATE(),
		COALESCE(ev.Id, ev2.ID, 0),
		0,
		COALESCE(v4.level1Id, v5.level1Id),
		COALESCE(v4.level2Id, v5.level2Id),
		COALESCE(v4.level3Id, v5.level3Id),
		CURRENCY,REPLACE( REPLACE( REPLACE( REPLACE( REPLACE( EVENT_NAME,' ',''),'''',''),'-',''),'+',''),':',''), '',
		CASE
			WHEN HOT_EVENT = 'true' THEN 1
			ELSE 0
		END,
		0,
		''
	FROM
		EventInfoTemp eit
			LEFT JOIN
		Venues ev ON ev.Venue_ID = eit.Venue_ID
			AND ev.CountryCode = eit.VENUE_Country_Code
			AND ev.Venue_ID != ''
			LEFT JOIN
		Venues ev2 ON ev2.City = eit.VENUE_CITY
			AND ev2.Street = eit.VENUE_STREET
			AND ev2.ZipCode = eit.VENUE_ZIP_CODE
			AND ev2.CountryCode = eit.VENUE_Country_Code
			AND ev2.Venue_ID = ''
			AND eit.VENUE_ID = ''
			AND ev2.VenueName = eit.VENUE_NAME
			AND ev.Longitude = eit.VENUE_LONGITUDE
			AND ev.Latitude = eit.VENUE_LATITUDE
			LEFT JOIN
		CategoryTmp v4 ON eit.CLASSIFICATION_SEGMENT = v4.lev1
			AND eit.CLASSIFICATION_GENRE = v4.lev2
			AND eit.CLASSIFICATION_SUB_GENRE = v4.lev3
			LEFT JOIN
		CategoryMisctmp v5 ON eit.CLASSIFICATION_GENRE = v5.lev2
			AND eit.CLASSIFICATION_SUB_GENRE = v5.lev3
	WHERE
		eit.CLASSIFICATION_SEGMENT != '' AND eit.CLASSIFICATION_SEGMENT != 'Undefined' AND eit.CLASSIFICATION_SEGMENT IS NOT NULL AND 
		NOT EXISTS( SELEct  1 FROM Events t2
			WHERE eit.EVENT_ID = t2.EVENT_ID) -- and ev.Id is null and ev2.Id is null

	SET @lastinsertedid = SCOPE_IDENTITY()
	set @rowcount = @@ROWCOUNT; -- INTO lastinsertedid , rowcount;

	/* Drop temp tables */
	DROP TABLE CategoryTmp;
	DROP TABLE CategoryMisctmp;
	TRUNCATE TABLE EventInfoTemp ;

	DELETE FROM Events where EventStartUTC<=GETUTCDATE() and Id not in(select distinct(EventId) from Products )
END