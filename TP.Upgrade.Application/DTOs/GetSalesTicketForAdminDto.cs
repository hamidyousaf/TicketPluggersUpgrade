namespace TP.Upgrade.Application.DTOs
{
    public class GetSalesTicketForAdminDto
    {
        public VendorForGetOrderByIdDto Vendor { get; set; }
        public ProductForGetSalesTicketForAdminDto Product { get; set; }
        public IEnumerable<GetOrderByIdDto> Sales { get; set; }
    }
    public class ProductForGetSalesTicketForAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EventForGetOrderByIdDto Event { get; set; }
    }
}
