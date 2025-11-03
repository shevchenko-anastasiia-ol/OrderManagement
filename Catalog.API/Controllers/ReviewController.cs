using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Commands.Review.CreateReview;
using Catalog.Application.Commands.Review.DeleteReview;
using Catalog.Application.Commands.Review.UpdateReview;
using Catalog.Application.Commands.Review.AddReviewReply;
using Catalog.Application.Commands.Review.DeleteAllProductReviews;
using Catalog.Application.Queries.Review.GetReviewById;
using Catalog.Application.Queries.Review.GetReviews;
using Catalog.Application.Queries.Review.GetReviewsByProduct;
using Catalog.Application.Queries.Review.GetReviewsByAuthor;
using Catalog.Application.Queries.Review.GetReviewsByRating;
using Catalog.Application.Queries.Review.GetProductAverageRating;
using Catalog.Application.Queries.Review.GetRatingDistribution;
using Catalog.Application.Queries.Review.SearchReviewsInComments;
using Catalog.Application.Queries.Review.GetHighRatedReviews;
using Catalog.Application.Queries.Review.GetLowRatedReviews;
using Catalog.Application.Queries.Review.GetMostRecentReviews;
using Catalog.Application.Queries.Review.GetReviewsWithReplies;
using Catalog.Domain.Entities.Parameters;
using Catalog.Domain.Exceptions;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    public class ReviewController : BaseApiController
    {
        public ReviewController(IMediator mediator) : base(mediator) { }

        // GET: api/review/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var review = await _mediator.Send(new GetReviewByIdQuery { ReviewId = id }, cancellationToken);
                if (review == null)
                    return NotFound();

                var etag = GenerateETag(review.UpdatedAt ?? review.CreatedAt);
                AddETagHeader(etag);

                return Ok(review);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review
        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] ReviewParameters parameters, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetReviewsQuery(parameters);
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // POST: api/review
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var createdReview = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdReview.Id }, createdReview);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // PUT: api/review/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateReviewCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.ReviewId != id)
                    return BadRequest("ID mismatch");

                var requestETag = GetIfMatchHeader();
                var existingReview = await _mediator.Send(new GetReviewByIdQuery { ReviewId = id }, cancellationToken);
                if (existingReview == null)
                    return NotFound();

                var currentETag = GenerateETag(existingReview.UpdatedAt ?? existingReview.CreatedAt);
                if (!ValidateETag(requestETag, currentETag))
                    return StatusCode(412, new { message = "ETag mismatch. The resource has been modified." });

                var updatedReview = await _mediator.Send(command, cancellationToken);

                var newETag = GenerateETag(updatedReview.UpdatedAt ?? updatedReview.CreatedAt);
                AddETagHeader(newETag);

                return Ok(updatedReview);
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

        // POST: api/review/{id}/reply
        [HttpPost("{id}/reply")]
        public async Task<IActionResult> AddReply(string id, [FromBody] AddReviewReplyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (command.ReviewId != id)
                    return BadRequest("ID mismatch");

                var updatedReview = await _mediator.Send(command, cancellationToken);
                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // DELETE: api/review/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteReviewCommand { ReviewId = id };
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(string productId, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetReviewsByProductQuery { ProductId = productId };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/author/{author}
        [HttpGet("author/{author}")]
        public async Task<IActionResult> GetByAuthor(string author, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetReviewsByAuthorQuery { Author = author };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/rating/{rating}
        [HttpGet("rating/{rating:int}")]
        public async Task<IActionResult> GetByRating(int rating, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetReviewsByRatingQuery { Rating = rating };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/product/{productId}/average-rating
        [HttpGet("product/{productId}/average-rating")]
        public async Task<IActionResult> GetAverageRating(string productId, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetProductAverageRatingQuery { ProductId = productId };
                var averageRating = await _mediator.Send(query, cancellationToken);
                return Ok(new { productId = productId, averageRating = averageRating });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/product/{productId}/rating-distribution
        [HttpGet("product/{productId}/rating-distribution")]
        public async Task<IActionResult> GetRatingDistribution(string productId, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetRatingDistributionQuery { ProductId = productId };
                var distribution = await _mediator.Send(query, cancellationToken);
                return Ok(distribution);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchInComments([FromQuery] string searchTerm, CancellationToken cancellationToken)
        {
            try
            {
                var query = new SearchReviewsInCommentsQuery { SearchTerm = searchTerm };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/high-rated
        [HttpGet("high-rated")]
        public async Task<IActionResult> GetHighRatedReviews([FromQuery] string? productId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetHighRatedReviewsQuery { ProductId = productId };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/low-rated
        [HttpGet("low-rated")]
        public async Task<IActionResult> GetLowRatedReviews([FromQuery] string? productId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetLowRatedReviewsQuery { ProductId = productId };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/recent
        [HttpGet("recent")]
        public async Task<IActionResult> GetMostRecentReviews([FromQuery] int count = 10, [FromQuery] string? productId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetMostRecentReviewsQuery { Count = count, ProductId = productId };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // GET: api/review/with-replies
        [HttpGet("with-replies")]
        public async Task<IActionResult> GetReviewsWithReplies([FromQuery] string? productId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new GetReviewsWithRepliesQuery { ProductId = productId };
                var reviews = await _mediator.Send(query, cancellationToken);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // DELETE: api/review/product/{productId}/all
        [HttpDelete("product/{productId}/all")]
        public async Task<IActionResult> DeleteAllProductReviews(string productId, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteAllProductReviewsCommand { ProductId = productId };
                var deletedCount = await _mediator.Send(command, cancellationToken);
                return Ok(new { productId = productId, deletedCount = deletedCount });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}