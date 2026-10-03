namespace Matchi.Application.Notifications;

public static class NotificationTypes
{
    public const string RequestCreated = "RequestCreated";
    public const string RequestCancelled = "RequestCancelled";
    public const string NewProposal = "NewProposal";
    public const string ProposalAccepted = "ProposalAccepted";
    public const string ProposalRejected = "ProposalRejected";
    public const string InvitationReceived = "InvitationReceived";
    public const string InvitationAccepted = "InvitationAccepted";
    public const string InvitationRejected = "InvitationRejected";
    public const string ExecutionCreated = "ExecutionCreated";
    public const string ExecutionScheduled = "ExecutionScheduled";
    public const string ProviderAssigned = "ProviderAssigned";
    public const string ExecutionStarted = "ExecutionStarted";
    public const string ExecutionCompleted = "ExecutionCompleted";
    public const string ExecutionCancelled = "ExecutionCancelled";
}

public static class NotificationEntityTypes
{
    public const string Request = "Request";
    public const string Proposal = "Proposal";
    public const string Deal = "Deal";
    public const string ServiceExecution = "ServiceExecution";
    public const string ExecutionAssignment = "ExecutionAssignment";
    public const string BusinessProvider = "BusinessProvider";
}
