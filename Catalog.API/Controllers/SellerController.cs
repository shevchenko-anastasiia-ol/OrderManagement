using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Commands.Seller.CreateSeller;
using Catalog.Application.Commands.Seller.DeleteSeller;
using Catalog.Application.Commands.Seller.UpdateSeller;
using Catalog.Application.Commands.Seller.UpdateSellerEmail;
using Catalog.Application.Commands.Seller.UpdateSellerPhone;
using Catalog.Application.Queries.Seller.GetSellerById;
using Catalog.Application.Queries.Seller.GetSellers;
using Catalog.Application.Queries.Seller.SearchSellersByName;
using Catalog.Application.Queries.Seller.GetSellerByEmail;
using Catalog.Application.Queries.Seller.GetSellerByPhone;
using Catalog.Application.Queries.Seller.GetSellerProductCount;
using Catalog.Application.Queries.Seller.SellerHasProducts;
using Catalog.Application.Queries.Seller.GetRecentSellers;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Exceptions;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    public class SellerController : BaseApiController
    {
        public SellerController(IMediator mediator) : base(mediator) { }

        // GET: api/seller/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var seller = await _mediator.Send(new GetSellerByIdQuery { SellerId = id }, cancellationToken);
                if (seller == null)
                    return NotFound();

                var etag = GenerateETag(seller.UpdatedAt ?? seller.CreatedAt);
                AddETagHeader(etag);

                return Ok(seller);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/seller
        [HttpGet]
        public async Task<IActionResult> GetSellers([FromQuery] SellerParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSellersQuery { Parameters = parameters };
                var sellers = await _mediator.Send(query, cancellationToken);
                return Ok(sellers);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // POST: api/seller
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSellerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var createdSeller = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdSeller.Id }, createdSeller);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PUT: api/seller/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateSellerCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.SellerId != id)
                    return BadRequest("ID mismatch");

                var requestETag = GetIfMatchHeader();
                var existingSeller = await _mediator.Send(new GetSellerByIdQuery { SellerId = id }, cancellationToken);
                if (existingSeller == null)
                    return NotFound();

                var currentETag = GenerateETag(existingSeller.UpdatedAt ?? existingSeller.CreatedAt);
                if (!ValidateETag(requestETag, currentETag))
                    return StatusCode(412, new { message = "ETag mismatch. The resource has been modified." });

                var updatedSeller = await _mediator.Send(command, cancellationToken);

                var newETag = GenerateETag(updatedSeller.UpdatedAt ?? updatedSeller.CreatedAt);
                AddETagHeader(newETag);

                return Ok(updatedSeller);
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

        // DELETE: api/seller/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteSellerCommand { SellerId = id };
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/seller/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            try
            {
                var query = new SearchSellersByNameQuery { Name = name };
                var sellers = await _mediator.Send(query, cancellationToken);
                return Ok(sellers);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/seller/email/{email}
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSellerByEmailQuery { Email = email };
                var seller = await _mediator.Send(query, cancellationToken);
                if (seller == null)
                    return NotFound();

                return Ok(seller);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/seller/phone/{phone}
        [HttpGet("phone/{phone}")]
        public async Task<IActionResult> GetByPhone(string phone, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSellerByPhoneQuery { Phone = phone };
                var seller = await _mediator.Send(query, cancellationToken);
                if (seller == null)
                    return NotFound();

                return Ok(seller);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/seller/{id}/products/count
        [HttpGet("{id}/products/count")]
        public async Task<IActionResult> GetProductCount(string id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSellerProductCountQuery { SellerId = id };
                var count = await _mediator.Send(query, cancellationToken);
                return Ok(new { sellerId = id, productCount = count });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/seller/{id}/has-products
        [HttpGet("{id}/has-products")]
        public async Task<IActionResult> HasProducts(string id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new SellerHasProductsQuery { SellerId = id };
                var hasProducts = await _mediator.Send(query, cancellationToken);
                return Ok(new { sellerId = id, hasProducts = hasProducts });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/seller/recent
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentSellers([FromQuery] int count = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetRecentSellersQuery { Count = count };
                var sellers = await _mediator.Send(query, cancellationToken);
                return Ok(sellers);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PATCH: api/seller/{id}/email
        [HttpPatch("{id}/email")]
        public async Task<IActionResult> UpdateEmail(string id, [FromBody] UpdateSellerEmailCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.SellerId != id)
                    return BadRequest("ID mismatch");

                await _mediator.Send(command, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PATCH: api/seller/{id}/phone
        [HttpPatch("{id}/phone")]
        public async Task<IActionResult> UpdatePhone(string id, [FromBody] UpdateSellerPhoneCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.SellerId != id)
                    return BadRequest("ID mismatch");

                await _mediator.Send(command, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}