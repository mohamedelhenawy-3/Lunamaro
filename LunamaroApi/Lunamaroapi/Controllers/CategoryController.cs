using Lunamaroapi.DTOs;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]



    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryService;
        public CategoryController(ICategory category)
        {
            _categoryService = category;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpPost("CreateCategory")]
        public async Task<ActionResult> Add([FromBody] CategoryDTO catdto)
        {
            var createdCategory = await _categoryService.AddSync(catdto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CategoryDTO dto)
        {
            var exists = await _categoryService.ExistsAsync(id);
            if (!exists)
                return NotFound();

            await _categoryService.UpdateAsync(id, dto);
            return NoContent(); // 204

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _categoryService.ExistsAsync(id);
            if (!exists)
                return NotFound();

            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
