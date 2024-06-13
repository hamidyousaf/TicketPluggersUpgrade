INSERT INTO Currencies (Name, CurrencyCode, Symbol, Country, Rate, IsPublished, CreatedBy, CreatedDate, IsDeleted, CountryCode)
VALUES
('US Dollar', 'USD', '$', 'United States', 1.0, 1, '', GETUTCDATE(), 0, 'US'),
('Euro', 'EUR', '€', 'Europe', 1.18, 1, '', GETUTCDATE(), 0, 'EU'),
('British Pound', 'GBP', '£', 'United Kingdom', 1.39, 1, '', GETUTCDATE(), 0, 'GB'),
('Japanese Yen', 'JPY', '¥', 'Japan', 0.0091, 1, '', GETUTCDATE(), 0, 'JP'),
('Australian Dollar', 'AUD', '$', 'Australia', 0.77, 1, '', GETUTCDATE(), 0, 'AU'),
('Canadian Dollar', 'CAD', '$', 'Canada', 0.81, 1, '', GETUTCDATE(), 0, 'CA'),
('Swiss Franc', 'CHF', 'Fr.', 'Switzerland', 1.10, 1, '', GETUTCDATE(), 0, 'CH'),
('Chinese Yuan', 'CNY', '¥', 'China', 0.16, 1, '', GETUTCDATE(), 0, 'CN'),
('Hong Kong Dollar', 'HKD', '$', 'Hong Kong', 0.13, 1, '', GETUTCDATE(), 0, 'HK'),
('New Zealand Dollar', 'NZD', '$', 'New Zealand', 0.71, 1, '', GETUTCDATE(), 0, 'NZ');