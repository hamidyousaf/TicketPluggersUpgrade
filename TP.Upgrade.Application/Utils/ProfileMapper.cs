using AutoMapper;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Utils
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            #region Events profile mapper
            CreateMap<Event, CreateEventRequest>();
            CreateMap<EventStatus, EventStatusDto>();
            CreateMap<Category, CategoryDto>();

            CreateMap<Event, GetCustomEventDto>()
                .ForMember(dest => dest.Venue, opt => opt
                    .MapFrom(src => src.Venue.VenueName));
            CreateMap<Event, GetEventByTextDto>()
                .ForMember(dest => dest.Venue, opt => opt
                    .MapFrom(src => src.Venue.VenueName));
            
            CreateMap<Event, SearchEventDto>()
                .ForMember(dest => dest.Venue, opt => opt
                    .MapFrom(src => src.Venue.VenueName))
                .ForMember(dest => dest.City, opt => opt
                    .MapFrom(src => src.Venue.City))
                .ForMember(dest => dest.Country, opt => opt
                    .MapFrom(src => src.Venue.CountryCode));

            CreateMap<Event, EventLite>()
                .ForMember(dest => dest.Venue, opt => opt
                    .MapFrom(src => src.Venue.VenueName))
                .ForMember(dest => dest.City, opt => opt
                    .MapFrom(src => src.Venue.City))
                .ForMember(dest => dest.CountryCode, opt => opt
                    .MapFrom(src => src.Venue.CountryCode))
                .ForMember(dest => dest.IsFavourits, opt => opt
                    .MapFrom(src => false))
                .ForMember(dest => dest.StartTime, opt => opt
                    .MapFrom(src => src.EventStartTime.ToString(@"hh\:mm")));
            #endregion

            #region CustomerFavourie profile mapper
            CreateMap<CustomerFavourite, CustomerFavouriteLite>().ReverseMap();
            CreateMap<CreateCustomerFavouriteRequest, CustomerFavourite>();
            CreateMap<CustomerFavourite, GetFavouriteVenueDto>();
            CreateMap<Venue, GetFavouriteVenueVM>();
            CreateMap<Venue, GetFavouriteEventForVenueVM>();
            CreateMap<Event, GetPopularSearchedEventDto>();
            CreateMap<Event, GetFavouriteEventForEventVM>()
                .ForMember(dest => dest.StartTime, opt => opt
                    .MapFrom(src => src.EventStartTime.ToString(@"hh\:mm")));
            CreateMap<CustomerFavourite, GetFavouriteEventDto>();
            #endregion

            #region Currency profile mapper
            CreateMap<Currency, CurrencyLite>();
            #endregion

            #region Venue profile mapper
            CreateMap<Venue, VenueLite>();
            #endregion

            #region Customer profile mapper
            CreateMap<Customer, CustomerLite>();
            CreateMap<Customer, GetCustomerSimpleProfileDto>()
                .ForMember(dest => dest.ProfilePicture, opt => opt
                    .MapFrom(src => src.User.ProfilePicture));
            CreateMap<Customer, GetCustomerProfileByIdDto>();
            CreateMap<Address, GetCustomerProfileByIdVM>();
            CreateMap<UpdateCustomerRequest, Customer>();
            CreateMap<User, Customer>()
                .ForMember(dest => dest.Username, opt => opt
                    .MapFrom(src => src.UserName))
                .ForMember(dest => dest.UserId, opt => opt
                    .MapFrom(src => src.Id));
            #endregion

            #region Address profile mapper
            CreateMap<Address, AddressLite>().ReverseMap();
            CreateMap<CreateAddressRequest, Address>();
            CreateMap<UpdateCustomerRequestForAddressVM, UpdateAddressRequest>();
            #endregion

            #region User profile mapper
            CreateMap<UpdateCustomerRequestForPasswordVM, ChangePasswordRequest>();
            CreateMap<UpdateCustomerRequest, UpdateUserRequest>();
            #endregion
            
            #region Specification Attribute Option profile mapper
            CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionLite>();
            #endregion

            #region Product profile mapper
            CreateMap<CreateProductRequest, Product>()
                .ForMember(dest => dest.StockQuantity, opt => opt
                    .MapFrom(src => src.Quantity))
                .ForMember(dest => dest.CreatedDate, opt => opt
                    .MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.TotalPrice, opt => opt
                    .MapFrom(src => (src.Price * src.Quantity) - src.ProceedCost));
            CreateMap<Product, ProductLite>();
            #endregion

            #region User profile mapper
            CreateMap<CustomerOrder, CustomerOrderLite>();
            #endregion

            CreateMap<CustomerOrder, GetOrderByIdDto>();
            CreateMap<Customer, CustomerForGetOrderByIdDto>();
            CreateMap<Customer, VendorForGetOrderByIdDto>();
            CreateMap<Product, ProductForGetOrderByIdDto>();
            CreateMap<Event, EventForGetOrderByIdDto>();
            CreateMap<Venue, VenueForGetOrderByIdDto>();
            CreateMap<Venue, VenueForGetEventsByVenueIdDto>();
            CreateMap<Venue, VenueForGetEventsBySearchDto>();
            CreateMap<Event, GetEventsByVenueIdDto>();
            CreateMap<Event, GetEventsBySearchDto>();
            CreateMap<ReportProblemRequest, ReportProblem>();
            CreateMap<Event, EventForMessageSessionDto>();
            CreateMap<Product, ProductLiteWithSpecifications>();
            CreateMap<Product, GetCustomerListingByIdResponse>();
            CreateMap<Product, GetProductByIdForEditForAppDto>()
                .ForMember(dest => dest.EventName, opt => opt
                    .MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.EventStart, opt => opt
                    .MapFrom(src => src.Event.EventStartUTC))
                .ForMember(dest => dest.Venue, opt => opt
                    .MapFrom(src => src.Event.Venue.VenueName))
                .ForMember(dest => dest.VenueCity, opt => opt
                    .MapFrom(src => src.Event.Venue.City))
                .ForMember(dest => dest.VenueCountry, opt => opt
                    .MapFrom(src => src.Event.Venue.CountryCode))
                .ForMember(dest => dest.SubTotal, opt => opt
                    .MapFrom(src => src.StockQuantity * src.Price));



            CreateMap<Venue, VenueForMessageSessionDto>();
            CreateMap<Product, MessageSessionDto>()
                .ForMember(dest => dest.OrderId, opt => opt
                    .MapFrom(src => (long) src.Id));
            CreateMap<CustomerOrder, MessageSessionDto>()
                .ForMember(dest => dest.OrderId, opt => opt
                    .MapFrom(src => src.Id))
                .ForMember(dest => dest.LastUpdatesOn, opt => opt
                    .MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.Event, opt => opt
                    .MapFrom(src => src.Product.Event));
            CreateMap<CustomerOrder, GetOrdersByTicketTypeIdDto>()
                .ForMember(dest => dest.OrderId, opt => opt
                    .MapFrom(src => src.Id))
                .ForMember(dest => dest.CurrentDate, opt => opt
                    .MapFrom(src => src.ExpectedTicketUploadDate))
                .ForMember(dest => dest.RequestDate, opt => opt
                    .MapFrom(src => src.TicketUploadRequestDate))
                .ForMember(dest => dest.EventName, opt => opt
                    .MapFrom(src => src.Product.Event.Name))
                .ForMember(dest => dest.Reason, opt => opt
                    .MapFrom(src => src.ResetShipmentReason))
                .ForMember(dest => dest.SellorName, opt => opt
                    .MapFrom(src => src.Vendor.FirstName + " " + src.Vendor.LastName));
        }
    }
}
