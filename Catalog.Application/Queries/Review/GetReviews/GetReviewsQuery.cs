using Catalog.Application.Interfaces.Queries;
using Catalog.Domain.Common.Helpers;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.Application.Queries.Review.GetReviews;

public record GetReviewsQuery (ReviewParameters Parameters) : IQuery<PagedList<Domain.Entities.Review>>
{
    
}