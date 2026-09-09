using FluentValidation;
using FluentValidation.Results;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Features.Providers;
using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Executions.Commands.StartServiceExecution;

public sealed record StartServiceExecutionCommand(long ExecutionId) : IRequest<string>;

public sealed class StartServiceExecutionCommandHandler : IRequestHandler<StartServiceExecutionCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceExecutionRepository _executions;

    public StartServiceExecutionCommandHandler(
        ICurrentUserService currentUserService,
        IServiceExecutionRepository executions)
    {
        _currentUserService = currentUserService;
        _executions = executions;
    }

    public async Task<string> Handle(StartServiceExecutionCommand command, CancellationToken cancellationToken)
    {
        var userId = Actor.RequireUserId(_currentUserService);
        var execution = await _executions.GetTrackedForStartCompleteAsync(
            command.ExecutionId,
            userId,
            cancellationToken);

        if (execution is null)
            throw new KeyNotFoundException("Execution was not found.");

        try
        {
            execution.Start();
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(new[] { new ValidationFailure("executionId", ex.Message) });
        }

        await _executions.SaveChangesAsync(cancellationToken);
        return execution.Status;
    }
}
