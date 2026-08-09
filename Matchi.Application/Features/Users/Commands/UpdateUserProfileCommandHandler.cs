using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Users.Commands;

public sealed class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return false;

        user.UpdateProfile(request.Name);
        await _userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }
}
