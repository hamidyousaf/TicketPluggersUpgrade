CREATE OR ALTER PROCEDURE [dbo].[GetEventsOnLoadNew]
-- Parameters.
	@radiusFrom int,
	@radiusTo int,
	@startIndex int,
	@elimit int,
	@latitude varchar(20),
	@longitude varchar(20),
	@categoryId int,
	@subCategoryId int,
	@eventStartDate DateTime2(0)=NULL,
	@eventEndDate DateTime2(0)=NULL,
	@skip int,
	@countryCode varchar(20)=NULL
AS
BEGIN
-- Declaring and setting variables.
	DECLARE @lng_min DECIMAL(10,2);
	DECLARE @lng_max DECIMAL(10,2);
    DECLARE @lat_min DECIMAL(10,2);
	DECLARE @lat_max DECIMAL(10,2);
	DECLARE @latitude1 DECIMAL(10,2);
	DECLARE @longitude1 DECIMAL(10,2);
	DECLARE @valFrom INT;
	SET @latitude1=cast(@latitude AS DECIMAL(10,2));
	SET @longitude1= cast(@longitude AS DECIMAL(10,2));
    
	IF (@eventStartDate is null) 
	BEGIN
		SET @eventStartDate = GETUTCDATE();
	END;
	SET @valFrom = @startIndex * @skip;
	SET @lng_min = @longitude1 - @radiusFrom/abs(cos(radians(@latitude1))*69);
	SET @lng_max = @longitude1 + @radiusTo/abs(cos(radians(@latitude1))*69);
	SET @lat_min = @latitude1 - (@radiusFrom/69);
	SET @lat_max = @latitude1 + (@radiusTo/69)
;WITH CTE2 AS (
	SELECT
		E.Id,E.Name,E.Description,E.EventStartUTC,E.EventStartTime,E.ImageURL,E.IsHotEvent,E.VenueID,TicketAvailable as AvailableTickets,
		E.Event_ID,E.Notes,E.EventStatusId,E.EventEndUTC,E.OnSaleStartDateTime,E.OnSaleEndDateTime,E.Published,E.IsDeleted,
		E.VenueName,CountryCode,TicketMinPrice,
		(
          6371 *
          acos(
              cos( RADIANS(  @latitude1 ) ) *
              cos( radians( E.Latitude ) ) *
              cos(
                  radians( E.Longitude ) - RADIANS( @longitude1 )
              ) +
              sin(RADIANS(@latitude1)) *
              sin(radians(E.Latitude))
          )
		) distance
	FROM 
      (
		SELECT EE.Id, Name, Description, EventStartUTC, EventStartTime, ImageURL, IsHotEvent, EE.VenueID, TicketAvailable, EE.Event_ID, Notes,
			EventStatusId, EventEndUTC, OnSaleStartDateTime, OnSaleEndDateTime, Published, EE.IsDeleted, EE.TicketMinPrice, DisplayOrder,
			SegmentId, GenreId, SubGenreId, EE.CreatedDate, EE.UpdatedDate, EE.TimeZone, EV.VenueName, EV.Longitude, EV.Latitude, EV.CountryCode 
		FROM Events EE INNER JOIN Venues EV ON EV.Id = EE.VenueId  
		WHERE Published = 1 AND EE.IsDeleted = 0 AND EE.SegmentId=@categoryId
			AND (( @subCategoryId > 0 AND EE.GenreId = @subCategoryId ) OR (@subCategoryId = 0 ))
			AND (EV.CountryCode=@countryCode OR @countryCode IS NULL)
			AND (( @eventStartDate IS NOT NULL AND EventStartUTC >= @eventStartDate ) OR (@eventStartDate IS NULL))
			AND (( @eventEndDate IS not NULL AND EventStartUTC <= @eventEndDate ) OR (@eventEndDate IS NULL))
			AND (EV.Longitude BETWEEN @lng_min AND @lng_max)	AND (EV.Latitude BETWEEN @lat_min and @lat_max)) E 
      )
	SELECT 
		c.* 
	FROM 
		CTE2 c 
    WHERE
		distance > @radiusFrom AND distance < @radiusTo
    GROUP BY Name, Id, Name, Description, EventStartUTC, EventStartTime, ImageURL, IsHotEvent, VenueID, TicketMinPrice, AvailableTickets,
		Event_ID, Notes, EventStatusId, EventEndUTC, OnSaleStartDateTime, OnSaleEndDateTime, Published, IsDeleted, VenueName, CountryCode, distance
	ORDER BY
	   c.distance ,c.EventStartUTC
  OFFSET @valFrom ROWS FETCH NEXT @elimit ROWS ONLY OPTION(RECOMPILE)
END
GO