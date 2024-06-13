CREATE OR ALTER PROCEDURE [dbo].[GetEventsByVenueId]
-- Parameters.
	@venueId bigint,
	@radiusFrom int,
	@radiusTo int,
	@startIndex int,
	@elimit int,
	@latitude varchar(20),
	@longitude varchar(20),
	@eventStartDate DateTime2(0)=null,
	@eventEndDate DateTime2(0)=null,
	@sortBy varchar(20)
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
	SET @valFrom = @startIndex*@elimit;
	IF (@eventStartDate is null) begin
		SET @eventStartDate = GETUTCDATE();
	END;
	SET @lng_min = @longitude1 - @radiusFrom/abs(cos(radians(@latitude1))*69);
	SET @lng_max = @longitude1 + @radiusTo/abs(cos(radians(@latitude1))*69);
	SET @lat_min = @latitude1 - (@radiusFrom/69);
	SET @lat_max = @latitude1 + (@radiusTo/69);
WITH CTE2 AS (
	SELECT
		E.Id,E.Name,E.Description,E.EventStartUTC,LEFT(E.EventStartTime,5) AS eventStartTime,E.ImageURL,E.IsHotEvent,E.VenueID,
		EV.VenueName, EV.City,EV.CountryCode,E.TimeZone,E.TicketMaxPrice,E.TicketMinPrice, E.AvailableTickets,TicketUploaded,EventSearchCount,SegmentID,
		E.Event_ID,E.Notes,E.EventStatusId,E.EventEndUTC,
		(
          6371 *
          acos(
              cos( RADIANS(  @latitude1 ) ) *
              cos( radians( EV.Latitude ) ) *
              cos(
                  radians( EV.Longitude ) - RADIANS( @longitude1 )
              ) +
              sin(RADIANS(@latitude1)) *
              sin(radians(EV.Latitude))
          )
		) distance
  FROM 
    (SELECT  Id,Name,Description,EventStartUTC,LEFT(EventStartTime,5) AS eventStartTime,ImageURL,IsHotEvent,VenueID, 
		TimeZone,TicketMaxPrice,TicketMinPrice, TicketAvailable AS AvailableTickets,TicketUploaded,EventSearchCount,SegmentID,
		Event_ID,Notes,EventStatusId,EventEndUTC 
	FROM Events 
	WHERE  Published = 1 AND IsDeleted = 0 and venueId= @venueId
        AND (( @eventStartDate IS NOT NULL AND EventStartUTC >= @eventStartDate ) OR (@eventStartDate IS NULL))
        AND (( @eventEndDate IS not NULL AND EventStartUTC <= @eventEndDate ) OR (@eventEndDate IS NULL))) E 
        INNER JOIN Venues EV ON EV.Id = E.VenueId      
      )
	SELECT 
		c.* 
	FROM 
		CTE2 c 
    WHERE
		distance > @radiusFrom AND distance < @radiusTo
    GROUP BY Name,
		Id,Name,Description,EventStartUTC,eventStartTime,ImageURL,IsHotEvent,VenueID,
		VenueName, City,CountryCode,TimeZone,TicketMaxPrice,TicketMinPrice,  AvailableTickets,TicketUploaded,EventSearchCount,SegmentID,
		Event_ID,Notes,EventStatusId,EventEndUTC,--Rank,
	distance
	ORDER BY
	  CASE 
	       WHEN @sortBy='distance' THEN distance
            WHEN @sortBy='price' THEN TicketMinPrice
	  END 
END
GO