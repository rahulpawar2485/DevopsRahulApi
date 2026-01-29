using InMemoryCrudApi.Models;
using InMemoryCrudApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryCrudApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // GET: api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(ProductRepository.GetAll());
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = ProductRepository.GetById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public IActionResult Create(Product product)
        {
            var created = ProductRepository.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, Product product)
        {
            var updated = ProductRepository.Update(id, product);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = ProductRepository.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
