CREATE OR ALTER PROCEDURE [dbo].[GetOrdersByCustomer] @customerId BIGINT AS
BEGIN
SELECT 
	customerOrders.Id AS OrderId,
	customerOrders.CreatedDate AS OrderDate,
	customerOrders.OrderConfirmDate,
	products.Name AS SectionName,
	products.ticketRow AS SectionRow,
	events.Name AS EventName,
	events.EventStatusId AS EventStatus,
	events.EventStartUTC AS EventDate,
	events.EventStartTime AS StartTime,
	customerOrders.Quantity AS Quantity,
	customerOrders.OrderTotal,
	customerOrders.OrderIndividualPrice,
	customerOrders.CustomerID,
	customerOrders.ProductId,
	products.CurrencyId,
	products.CurrencyCode,
	products.CurrencySymbol,
	Customer.FirstName AS CustomerFName,
	Customer.LastName AS CustomerLName,
	customerOrders.OrderStatusId,
	customerOrders.ShippingStatusId,
	products.ticketTypeId AS ticketType,
	customerOrders.PaymentId,
	customerOrders.PaymentDate,
	ISNULL(venues.VenueName, '') AS VenueName,
	ISNULL(venues.City, '') AS VenueCity,
	ISNULL(venues.CountryCode, '') AS VenueCountry
FROM [dbo].[customerorders] customerOrders
	INNER JOIN [dbo].[customers] Customer ON Customer.Id = customerOrders.CustomerID
	INNER JOIN [dbo].[products] products ON products.Id = customerOrders.ProductId
	LEFT JOIN [dbo].[events] events ON events.Id = products.eventId
	LEFT JOIN [dbo].[venues] venues ON venues.Id = events.VenueID
WHERE customerOrders.CustomerID = @customerId AND customerOrders.OrderStatusId != 0 AND customerOrders.PaymentStatusId != 0
ORDER BY customerOrders.CreatedDate DESC
END
GO