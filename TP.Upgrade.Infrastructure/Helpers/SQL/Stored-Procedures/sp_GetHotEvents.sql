CREATE OR ALTER PROCEDURE [dbo].[GetHotEvents]
-- Parameters.
	@radiusFrom int,
	@radiusTo int,
	@latitude varchar(20),
	@longitude varchar(20),
	@eventStartDate DateTime2(0)=null,
	@eventEndDate DateTime2(0)=null,
	@sortBy varchar(20),
	@startIndex int,
	@elimit int,
	@countryCode varchar(20)=null
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
	SET @lng_min = @longitude1 - @radiusFrom/abs(cos(radians(@latitude1))*69);
	SET @lng_max = @longitude1 + @radiusTo/abs(cos(radians(@latitude1))*69); -- 10 multiply is extra need to remove in future
	SET @lat_min = @latitude1 - (@radiusFrom/69);
	SET @lat_max = @latitude1 + (@radiusTo/69) -- 10 multiply is extra need to remove in future
 	SET @valFrom = @startIndex*@elimit;
	IF (@eventStartDate is null) 
	BEGIN
		SET @eventStartDate = GETUTCDATE();
	END;
WITH CTE2 AS (
  SELECT
      E.Id, E.Name, E.Description, E.EventStartUTC, LEFT(E.EventStartTime,5) AS eventStartTime, E.ImageURL, E.IsHotEvent, E.VenueID,
      E.VenueName, E.City, E.CountryCode, E.TimeZone, E.TicketMaxPrice, E.TicketMinPrice, E.AvailableTickets, TicketUploaded,
	  EventSearchCount,SegmentID,E.Event_ID,E.Notes,E.EventStatusId,E.EventEndUTC,
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
    (SELECT EE.Id, Name, Description, EventStartUTC, LEFT(EventStartTime,5) AS eventStartTime, ImageURL, IsHotEvent, VenueID, 
		EE.TimeZone, TicketMaxPrice, TicketMinPrice, TicketAvailable AS AvailableTickets, TicketUploaded, EventSearchCount, SegmentID,
		EV.VenueName, EV.City, EV.CountryCode, EV.Longitude, EV.Latitude, Event_ID, Notes, EventStatusId, EventEndUTC 
	FROM Events EE INNER JOIN Venues EV ON EV.Id = EE.VenueId 
	WHERE CONVERT(DATE, EventStartUTC) > CONVERT(DATE, GETUTCDATE()) and  Published = 1 AND EE.IsDeleted = 0
		AND (EV.CountryCode=@countryCode OR @countryCode IS NULL)
        AND ((@eventStartDate IS NOT NULL AND CONVERT(DATE,EventStartUTC) >= CONVERT(DATE,@eventStartDate) ) OR (@eventStartDate IS NULL))
        AND (( @eventEndDate IS not NULL AND CONVERT(DATE,EventStartUTC) <= CONVERT(DATE,@eventEndDate) ) OR (@eventEndDate IS NULL))
		AND IsHotEvent =1 ) E
      )
	SELECT 
		c.* 
	FROM 
		CTE2 c 
	WHERE
		distance > @radiusFrom AND distance < @radiusTo
	GROUP BY Name, Id, Name, Description, EventStartUTC, eventStartTime, ImageURL, IsHotEvent, VenueID, VenueName, 
		City, CountryCode, TimeZone, TicketMaxPrice, TicketMinPrice, AvailableTickets, TicketUploaded, EventSearchCount,
		SegmentID ,Event_ID ,Notes ,EventStatusId ,EventEndUTC ,distance
	ORDER BY
	CASE 
		WHEN @sortBy='distance' THEN distance
		WHEN @sortBy='price' THEN TicketMinPrice
	END 
OFFSET @valFrom ROWS FETCH NEXT @elimit ROWS ONLY OPTION(RECOMPILE)
END
GO