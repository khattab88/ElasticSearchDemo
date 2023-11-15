using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IElasticClient elasticClient, ILogger<ProductsController> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        [HttpGet(Name = "SearchProducts")]
        public async Task<IActionResult> Get(string keyword)
        {
            var results = await _elasticClient.SearchAsync<Product>(
                s => s.Query(
                    q => q.QueryString(
                        d => d.Query('*'+ keyword +'*')
                        )
                    ).Size(100));

            return Ok(results.Documents.ToList());
        }

        [HttpPost(Name = "AddProduct")]
        public async Task<IActionResult> Post(Product product) 
        {
            await _elasticClient.IndexDocumentAsync(product);

            return Created("", product);
        }
    }
}
