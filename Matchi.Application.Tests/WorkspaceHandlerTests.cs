using FluentValidation;
using Matchi.Application.Features.Users.Commands;
using Matchi.Application.Features.Users.Queries;
using Matchi.Application.Workspaces;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Tests;

public sealed class WorkspaceResolverTests
{
    [Fact]
    public void Preferred_WinsWhenAccessible()
    {
        var available = WorkspaceResolver.Available(true, true, false);
        Assert.Equal("provider", WorkspaceResolver.Resolve(available, "provider", "customer"));
    }

    [Fact]
    public void InvalidPreferred_FallsBackToLast()
    {
        var available = WorkspaceResolver.Available(true, true, false);
        Assert.Equal("provider", WorkspaceResolver.Resolve(available, "business", "provider"));
    }

    [Fact]
    public void InvalidPreferredAndLast_UseDeterministicFallback()
    {
        var available = WorkspaceResolver.Available(true, true, true);
        Assert.Equal("customer", WorkspaceResolver.Resolve(available, "unknown", "nope"));
    }

    [Fact]
    public void OnlyCustomer()
    {
        var available = WorkspaceResolver.Available(true, false, false);
        Assert.Equal(new[] { "customer" }, available);
        Assert.Equal("customer", WorkspaceResolver.Resolve(available, "provider", "business"));
    }

    [Fact]
    public void OnlyProvider()
    {
        var available = WorkspaceResolver.Available(false, true, false);
        Assert.Equal(new[] { "provider" }, available);
        Assert.Equal("provider", WorkspaceResolver.Resolve(available, "customer", null));
    }

    [Fact]
    public void OnlyBusiness()
    {
        var available = WorkspaceResolver.Available(false, false, true);
        Assert.Equal(new[] { "business" }, available);
        Assert.Equal("business", WorkspaceResolver.Resolve(available, "customer", "provider"));
    }

    [Fact]
    public void MultipleWorkspaces_FallbackOrderIsCustomerProviderBusiness()
    {
        var available = WorkspaceResolver.Available(true, true, true);
        Assert.Equal(new[] { "customer", "provider", "business" }, available);
    }

    [Fact]
    public void StalePersistedValue_DoesNotGrantAccess()
    {
        var available = WorkspaceResolver.Available(true, false, false);
        Assert.False(WorkspaceResolver.Contains(available, "provider"));
        Assert.Equal("customer", WorkspaceResolver.Resolve(available, "provider", "provider"));
    }

    [Fact]
    public void Normalize_RejectsUnknownTokens()
    {
        Assert.Equal("customer", WorkspaceResolver.Normalize("Customer"));
        Assert.Equal("provider", WorkspaceResolver.Normalize("PROVIDER"));
        Assert.Null(WorkspaceResolver.Normalize("customer-workspace"));
    }
}

public sealed class WorkspaceStateCommandTests
{
    [Fact]
    public async Task SetPreferred_RequiresAccessibleWorkspace()
    {
        var env = Env(hasProvider: false);
        var handler = new SetPreferredWorkspaceCommandHandler(
            new FakeCurrentUser(env.User.Id),
            env.Users,
            env.Access);

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SetPreferredWorkspaceCommand("provider"), CancellationToken.None));
        Assert.Contains(ex.Errors, e => e.PropertyName == "workspace");
        Assert.Null(env.User.PreferredWorkspace);
    }

    [Fact]
    public async Task SetLast_RequiresAccessibleWorkspace()
    {
        var env = Env(hasProvider: false);
        var handler = new SetLastWorkspaceCommandHandler(
            new FakeCurrentUser(env.User.Id),
            env.Users,
            env.Access);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new SetLastWorkspaceCommand("business"), CancellationToken.None));
        Assert.Null(env.User.LastWorkspace);
    }

    [Fact]
    public async Task SwitchUpdatesLast_NotPreferred()
    {
        var env = Env(hasProvider: true);
        env.User.SetPreferredWorkspace("customer");
        env.User.SetLastWorkspace("customer");

        var handler = new SetLastWorkspaceCommandHandler(
            new FakeCurrentUser(env.User.Id),
            env.Users,
            env.Access);

        var state = await handler.Handle(new SetLastWorkspaceCommand("provider"), CancellationToken.None);

        Assert.Equal("customer", env.User.PreferredWorkspace);
        Assert.Equal("provider", env.User.LastWorkspace);
        Assert.Equal("customer", state.PreferredWorkspace);
        Assert.Equal("provider", state.LastWorkspace);
        Assert.Equal("customer", state.ResolvedWorkspace);
        Assert.Equal(1, env.Users.UpdateCount);
    }

    [Fact]
    public async Task SetPreferred_AlsoSetsLast()
    {
        var env = Env(hasProvider: true);
        env.User.SetPreferredWorkspace("customer");
        env.User.SetLastWorkspace("customer");

        var handler = new SetPreferredWorkspaceCommandHandler(
            new FakeCurrentUser(env.User.Id),
            env.Users,
            env.Access);

        var state = await handler.Handle(new SetPreferredWorkspaceCommand("provider"), CancellationToken.None);

        Assert.Equal("provider", env.User.PreferredWorkspace);
        Assert.Equal("provider", env.User.LastWorkspace);
        Assert.Equal("provider", state.ResolvedWorkspace);
    }

    [Fact]
    public async Task GetState_IgnoresStalePreferred()
    {
        var env = Env(hasProvider: false);
        env.User.SetPreferredWorkspace("provider");
        env.User.SetLastWorkspace("customer");

        var handler = new GetMyWorkspaceStateQueryHandler(
            new FakeCurrentUser(env.User.Id),
            env.Users,
            env.Access);

        var state = await handler.Handle(new GetMyWorkspaceStateQuery(), CancellationToken.None);

        Assert.Equal("customer", state.ResolvedWorkspace);
        Assert.DoesNotContain("provider", state.AvailableWorkspaces);
    }

    [Fact]
    public async Task Commands_UseAuthenticatedUserOnly()
    {
        var env = Env(hasProvider: true);
        var other = new User("09120000099").WithId(99);
        env.Users.Other = other;

        var handler = new SetPreferredWorkspaceCommandHandler(
            new FakeCurrentUser(env.User.Id),
            env.Users,
            env.Access);

        await handler.Handle(new SetPreferredWorkspaceCommand("provider"), CancellationToken.None);

        Assert.Equal("provider", env.User.PreferredWorkspace);
        Assert.Null(other.PreferredWorkspace);
    }

    [Fact]
    public async Task AccessService_ProviderRequiresRecord_NotRole()
    {
        var env = Env(hasProvider: false);
        env.Roles.Roles.Add("PROVIDER");
        var available = await env.Access.ListAvailableAsync(env.User.Id);
        Assert.Equal(new[] { "customer" }, available);
    }

    private static WorkspaceEnv Env(bool hasProvider)
    {
        var user = new User("09120000001").WithId(7);
        var users = new WorkspaceUserRepository(user);
        var roles = new FakeUserRoleRepository { Roles = ["USER"] };
        var providers = new FakeProviderRepository();
        if (hasProvider)
            providers.Mine = new Provider(user.Id, "Prov", "09120000000").WithId(5);
        var businesses = new OfferingBusinessRepository();
        var access = new WorkspaceAccessService(roles, providers, businesses);
        return new WorkspaceEnv(user, users, roles, access);
    }

    private sealed record WorkspaceEnv(
        User User,
        WorkspaceUserRepository Users,
        FakeUserRoleRepository Roles,
        WorkspaceAccessService Access);
}

internal sealed class WorkspaceUserRepository : IUserRepository
{
    public WorkspaceUserRepository(User user)
    {
        User = user;
    }

    public User User { get; }
    public User? Other { get; set; }
    public int UpdateCount { get; private set; }

    public Task<User?> GetByMobileAsync(string mobile, CancellationToken cancellationToken = default) =>
        Task.FromResult<User?>(User.Mobile == mobile ? User : null);

    public Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        if (User.Id == userId)
            return Task.FromResult<User?>(User);
        if (Other?.Id == userId)
            return Task.FromResult<User?>(Other);
        return Task.FromResult<User?>(null);
    }

    public Task<User> AddAsync(User user, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        UpdateCount++;
        return Task.CompletedTask;
    }
}

internal sealed class FakeUserRoleRepository : IUserRoleRepository
{
    public List<string> Roles { get; set; } = ["USER"];

    public Task EnsureRoleAsync(long userId, string roleCode, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<IReadOnlyList<string>> GetRoleCodesByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>>(Roles);

    public Task<IReadOnlyList<string>> GetPermissionCodesByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>>([]);
}
