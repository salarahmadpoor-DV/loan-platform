using Matchi.Domain.Entities;

namespace Matchi.Application.Tests;

internal static class MarketplaceGraph
{
    public const long ProviderUserId = 10;
    public const long ProviderEntityId = 5;
    public const long OtherProviderUserId = 11;
    public const long OtherProviderEntityId = 44;
    public const long BusinessOwnerUserId = 20;
    public const long CustomerUserId = 30;
    public const long OtherUserId = 99;

    public static Provider OwnProvider() =>
        new Provider(ProviderUserId, "Prov", "09120000000").WithId(ProviderEntityId);

    public static Provider OtherProvider() =>
        new Provider(OtherProviderUserId, "Other", "09121111111").WithId(OtherProviderEntityId);

    public static Deal ProviderDeal(string requestType = "Service", string dealStatus = "Active")
    {
        var customer = new Customer(CustomerUserId).WithId(1);
        var request = new Request(customer.Id, requestType, "Need work").WithId(1);
        request.Set(nameof(Request.Customer), customer);
        request.Set(nameof(Request.CustomerId), customer.Id);

        var provider = new Provider(ProviderUserId, "Prov", "09120000000").WithId(5);
        var proposal = Proposal.ForProvider(request.Id, provider.Id, 100m).WithId(1);
        proposal.Set(nameof(Proposal.Provider), provider);

        var deal = Deal.Create(request.Id, proposal.Id, customer.Id, 100m).WithId(1);
        deal.Set(nameof(Deal.Request), request);
        deal.Set(nameof(Deal.Proposal), proposal);
        deal.Set(nameof(Deal.Status), dealStatus);
        return deal;
    }

    public static Deal BusinessDeal(string requestType = "Service", string dealStatus = "Active")
    {
        var customer = new Customer(CustomerUserId).WithId(1);
        var request = new Request(customer.Id, requestType, "Need work").WithId(1);
        request.Set(nameof(Request.Customer), customer);

        var business = new Business(BusinessOwnerUserId, "Biz").WithId(7);
        var proposal = Proposal.ForBusiness(request.Id, business.Id, 200m).WithId(2);
        proposal.Set(nameof(Proposal.Business), business);

        var deal = Deal.Create(request.Id, proposal.Id, customer.Id, 200m).WithId(2);
        deal.Set(nameof(Deal.Request), request);
        deal.Set(nameof(Deal.Proposal), proposal);
        deal.Set(nameof(Deal.Status), dealStatus);
        return deal;
    }
}
