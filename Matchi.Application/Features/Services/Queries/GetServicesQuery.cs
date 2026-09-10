using MediatR;
using System.Collections.Generic;

namespace Matchi.Application.Features.Services.Queries;

public record GetServicesQuery(long? CategoryId, string? Query, int Page, int PageSize)
    : IRequest<IEnumerable<ServiceDto>>;

public record ServiceDto(long Id, string Name, long CategoryId);
