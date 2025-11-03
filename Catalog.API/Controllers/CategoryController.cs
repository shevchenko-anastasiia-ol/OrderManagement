using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Commands.Category;
using Catalog.Application.Commands.Category.DeleteCategory;
using Catalog.Application.Commands.Category.UpdateCategory;
using Catalog.Application.Queries.Category.GetCategoryById;
using Catalog.Application.Queries.Category.GetCategories;
using Catalog.Application.Queries.Category.SearchCategoriesByName;
using Catalog.Application.Queries.Category.GetCategoryByName;
using Catalog.Application.Queries.Category.GetCategoryProductCount;
using Catalog.Application.Queries.Category.IsCategoryInUse;
using Catalog.Application.Queries.Category.GetEmptyCategories;
using Catalog.Application.Queries.Category.GetRecentCategories;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Exceptions;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : BaseApiController
    {
        public CategoryController(IMediator mediator) : base(mediator) { }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _mediator.Send(new GetCategoryByIdQuery { CategoryId = id }, cancellationToken);
                if (category == null)
                    return NotFound();

                var etag = GenerateETag(category.UpdatedAt ?? category.CreatedAt);
                AddETagHeader(etag);

                return Ok(category);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetCategoriesQuery { Parameters = parameters };
                var categories = await _mediator.Send(query, cancellationToken);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var createdCategory = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.CategoryId != id)
                    return BadRequest("ID mismatch");

                var requestETag = GetIfMatchHeader();
                var existingCategory = await _mediator.Send(new GetCategoryByIdQuery { CategoryId = id }, cancellationToken);
                if (existingCategory == null)
                    return NotFound();

                var currentETag = GenerateETag(existingCategory.UpdatedAt ?? existingCategory.CreatedAt);
                if (!ValidateETag(requestETag, currentETag))
                    return StatusCode(412, new { message = "ETag mismatch. The resource has been modified." });

                var updatedCategory = await _mediator.Send(command, cancellationToken);

                var newETag = GenerateETag(updatedCategory.UpdatedAt ?? updatedCategory.CreatedAt);
                AddETagHeader(newETag);

                return Ok(updatedCategory);
            }
            catch (ConcurrencyException ex)
            {
                return HandleConcurrencyConflict(ex);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteCategoryCommand { CategoryId = id };
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/category/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            try
            {
                var query = new SearchCategoriesByNameQuery { Name = name };
                var categories = await _mediator.Send(query, cancellationToken);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/category/name/{name}
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetCategoryByNameQuery { Name = name };
                var category = await _mediator.Send(query, cancellationToken);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/category/{id}/products/count
        [HttpGet("{id}/products/count")]
        public async Task<IActionResult> GetProductCount(string id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetCategoryProductCountQuery { CategoryId = id };
                var count = await _mediator.Send(query, cancellationToken);
                return Ok(new { categoryId = id, productCount = count });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/category/{id}/in-use
        [HttpGet("{id}/in-use")]
        public async Task<IActionResult> IsCategoryInUse(string id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new IsCategoryInUseQuery { CategoryId = id };
                var inUse = await _mediator.Send(query, cancellationToken);
                return Ok(new { categoryId = id, inUse = inUse });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/category/empty
        [HttpGet("empty")]
        public async Task<IActionResult> GetEmptyCategories(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetEmptyCategoriesQuery();
                var categories = await _mediator.Send(query, cancellationToken);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/category/recent
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentCategories([FromQuery] int count = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetRecentCategoriesQuery { Count = count };
                var categories = await _mediator.Send(query, cancellationToken);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}