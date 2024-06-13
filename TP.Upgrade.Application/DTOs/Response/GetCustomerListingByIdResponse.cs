namespace TP.Upgrade.Application.DTOs.Response
{
    public class GetCustomerListingByIdResponse : ProductLite
    {
        public int Id { get; set; }
        public IList<byte> SelectedSpecificationAttributeIds { get; set; }
    }
}
