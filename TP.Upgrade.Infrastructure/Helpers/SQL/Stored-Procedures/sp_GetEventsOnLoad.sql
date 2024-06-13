CREATE OR ALTER PROCEDURE [dbo].[GetEventsOnLoad]
-- Parameters.
	@startIndex int,
	@elimit int,
	@point varchar(100),
	@radiusFrom int,
	@radiusTo int,
    @latitude varchar(20),
	@longitude varchar(20),
	@categoryId int,
	@subCategoryId int,
	@eventStartDate varchar(20)=null,
	@eventEndDate varchar(20)=null,
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
	SET @valFrom = @startIndex*@elimit;
	SET @lng_min = @longitude1 -@radiusFrom /abs(cos(radians(@latitude1))*69);
	SET @lng_max = @longitude1 +@radiusTo/abs(cos(radians(@latitude1))*69);
	SET @lat_min = @latitude1 - (@radiusFrom/69);
	SET @lat_max = @latitude1 + (@radiusTo/69);
;WITH CTE2 AS (
	SELECT
      E.Id, E.Name, E.Description, E.EventStartUTC, E.EventStartTime, E.ImageURL, E.IsHotEvent, E.VenueID,
      E.Event_ID, E.Notes, E.EventStatusId, E.EventEndUTC, E.OnSaleStartDateTime, E.OnSaleEndDateTime, E.Published,
	  E.IsDeleted, E.VenueName, E.CountryCode, TicketMinPrice,TicketMaxPrice, TicketAvailable as AvailableTickets,
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
      ) distance,
	  ROW_NUMBER() Over(Partition by e.SegmentId Order By (
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
      )) as RowNum,
	  E.SegmentId
	FROM 
      (SELECT   
			EE.Id, Name, Description, EventStartUTC, EventStartTime, ImageURL, IsHotEvent, EE.VenueID,
			EE.TicketAvailable, EE.Event_ID, Notes, EventStatusId, EventEndUTC, OnSaleStartDateTime,
			OnSaleEndDateTime, Published, EE.IsDeleted, TicketMinPrice, TicketMaxPrice, DisplayOrder,
			SegmentId, GenreId, SubGenreId, EE.CreatedDate, EE.UpdatedDate, EE.TimeZone, EV.VenueName,
			EV.Longitude, EV.Latitude, EV.CountryCode 
		FROM Events EE INNER JOIN Venues EV ON EV.Id = EE.VenueId  
		WHERE Published = 1 AND EE.IsDeleted = 0 AND EE.SegmentId IN (SELECT Id FROM Categories WHERE Name IN ('Sports', 'Concerts', 'Theater & Comedy', 'Miscellaneous'))
			AND (( @subCategoryId > 0 AND EE.GenreId = @subCategoryId ) OR (@subCategoryId = 0 ))
			AND (EV.CountryCode=@countryCode OR @countryCode IS NULL)
			AND (( @eventStartDate IS NOT NULL AND Convert(date, EventStartUTC) >= Convert(date, @eventStartDate) ) OR (@eventStartDate IS NULL AND Convert(date, EventStartUTC) >= Convert(date, GETDATE ())))
			AND (( @eventEndDate IS not NULL AND Convert(date, EventStartUTC) <= Convert(date, @eventEndDate) ) OR (@eventEndDate IS NULL))
			AND (EV.Longitude BETWEEN @lng_min AND @lng_max)	AND (EV.Latitude BETWEEN @lat_min and @lat_max)) E      
      )
	SELECT 
		c.* 
	FROM 
		CTE2 c 
    WHERE
		distance > @radiusFrom AND distance < @radiusTo
		AND RowNum <= @elimit
    GROUP BY Name,
		Id,Name,Description,EventStartUTC,EventStartTime,ImageURL,IsHotEvent,VenueID,TicketMinPrice,TicketMaxPrice,AvailableTickets,
		Event_ID,Notes,EventStatusId,EventEndUTC,OnSaleStartDateTime,OnSaleEndDateTime,Published,IsDeleted, VenueName,CountryCode,
		distance, RowNum, SegmentId
	ORDER BY
	   c.distance ,c.EventStartUTC, c.AvailableTickets
  OFFSET @valFrom ROWS FETCH NEXT @elimit*3 ROWS ONLY OPTION(RECOMPILE)
END
--exec GetEventsOnLoad @startIndex=0,@elimit=12,@point=N'51.498255,-0.115023',@latitude=N'51.498255',@longitude=N'-0.115023',@radiusFrom=0,@radiusTo=1000,@categoryId=239,@subCategoryId=239,@eventStartDate='2022-05-27',@eventEndDate=null,@countryCode=N'GB'

GO