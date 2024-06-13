using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP.Upgrade.Application.Common.Contracts.IServices;

namespace TP.Upgrade.Api.Controllers
{
    public class ProductSpecificationController : ApiBaseController
    {
        private readonly ISpecificationAttributeOptionService _specificationAttributeOptionService;
        public ProductSpecificationController(ISpecificationAttributeOptionService specificationAttributeOptionService)
        {
            _specificationAttributeOptionService= specificationAttributeOptionService;
        }

        [AllowAnonymous, HttpGet("GetProductFeatureAttributeOption")]
        public async Task<IActionResult> GetProductFeatureAttributeOption()
        {
            byte specificationAttributeId = 1; // Here 1 mean Specification Attribute is: Restrictions.
            return Ok(await _specificationAttributeOptionService
                .GetSpecificationAttributeOptionsBySpecificationAttributeId(specificationAttributeId)); 
        }
        [AllowAnonymous, HttpGet("GetProductDeclaimerAttributeOption")]
        public async Task<IActionResult> GetProductDeclaimerAttributeOption()
        {
            byte specificationAttributeId = 2; // Here 2 mean Specification Attribute is: Listing Notes.
            return Ok(await _specificationAttributeOptionService
                .GetSpecificationAttributeOptionsBySpecificationAttributeId(specificationAttributeId));
        }
    }
}
