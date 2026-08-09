using MediatR;
using System.Collections.Generic;

namespace Matchi.Application.Features.Services.Queries;

public record GetServicesQuery(long? CategoryId = null) : IRequest<IEnumerable<ServiceDto>>;

public record ServiceDto(long Id, string Name, long CategoryId);
