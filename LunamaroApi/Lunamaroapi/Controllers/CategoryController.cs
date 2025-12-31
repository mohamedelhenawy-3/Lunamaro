using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Category;
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
        private readonly ICategoryService _categoryService;
        
        public CategoryController(ICategoryService category)
        {
            _categoryService = category;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateCategory")]
        public async Task<ActionResult> Add([FromBody] CreateCategoryDTO catdto)
        {
            var createdCategory = await _categoryService.AddAsync(catdto);
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
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CategoryDTO dto)
        {
            var exists = await _categoryService.ExistsAsync(id);
            if (!exists)
                return NotFound();

            await _categoryService.UpdateAsync(id, dto);
            return NoContent(); // 204

        }
        [Authorize(Roles = "Admin")]
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
