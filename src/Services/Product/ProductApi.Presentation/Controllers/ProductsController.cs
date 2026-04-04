using BuildingBlocks.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Mappings;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            //Get all products from the repo
            var products = await _productRepository.GetAllAsync();
            if (!products.Any())
            {
                return NotFound("No products found in the database");
            }
            //convert data from entity to DTOList
            var response = ProductMappings.ToDtoList(products);
            return Ok(response);
        }


    }
}
