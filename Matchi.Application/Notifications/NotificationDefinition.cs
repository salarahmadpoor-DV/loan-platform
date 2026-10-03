namespace Matchi.Application.Notifications;

public sealed record NotificationDefinition(
    string Type,
    string Title,
    string Message,
    string? EntityType,
    long? EntityId,
    string? ActionUrl)
{
    public string ReferenceKey => EntityId is > 0 && !string.IsNullOrWhiteSpace(EntityType)
        ? $"{Type}:{EntityType}:{EntityId}"
        : $"{Type}:{EntityId}";
}

public static class NotificationCatalog
{
    public static NotificationDefinition RequestCreated(long requestId) =>
        new(
            NotificationTypes.RequestCreated,
            "درخواست ثبت شد",
            "درخواست شما با موفقیت ثبت شد.",
            NotificationEntityTypes.Request,
            requestId,
            $"/customer/requests/{requestId}");

    public static NotificationDefinition RequestCancelled(long requestId) =>
        new(
            NotificationTypes.RequestCancelled,
            "درخواست لغو شد",
            "یک درخواست لغو شده است.",
            NotificationEntityTypes.Request,
            requestId,
            $"/provider/requests/{requestId}");

    public static NotificationDefinition NewProposal(long requestId, long proposalId) =>
        new(
            NotificationTypes.NewProposal,
            "پیشنهاد جدید",
            "برای درخواست شما یک پیشنهاد جدید ثبت شده است.",
            NotificationEntityTypes.Proposal,
            proposalId,
            $"/customer/requests/{requestId}/proposals");

    public static NotificationDefinition ProposalAccepted(long dealId) =>
        new(
            NotificationTypes.ProposalAccepted,
            "پیشنهاد پذیرفته شد",
            "پیشنهاد شما پذیرفته شد و معامله ایجاد گردید.",
            NotificationEntityTypes.Deal,
            dealId,
            "/provider/deals");

    public static NotificationDefinition ProposalRejected(long proposalId) =>
        new(
            NotificationTypes.ProposalRejected,
            "پیشنهاد رد شد",
            "پیشنهاد شما رد شد.",
            NotificationEntityTypes.Proposal,
            proposalId,
            "/provider/proposals");

    public static NotificationDefinition InvitationReceived(long membershipId) =>
        new(
            NotificationTypes.InvitationReceived,
            "دعوت به کسب‌وکار",
            "یک کسب‌وکار شما را به عضویت دعوت کرده است.",
            NotificationEntityTypes.BusinessProvider,
            membershipId,
            "/provider/invitations");

    public static NotificationDefinition InvitationAccepted(long membershipId) =>
        new(
            NotificationTypes.InvitationAccepted,
            "دعوت پذیرفته شد",
            "دعوت عضویت کسب‌وکار پذیرفته شد.",
            NotificationEntityTypes.BusinessProvider,
            membershipId,
            "/provider/business/providers");

    public static NotificationDefinition InvitationRejected(long membershipId) =>
        new(
            NotificationTypes.InvitationRejected,
            "دعوت رد شد",
            "دعوت عضویت کسب‌وکار رد شد.",
            NotificationEntityTypes.BusinessProvider,
            membershipId,
            "/provider/business/providers");

    public static NotificationDefinition ExecutionCreated(long executionId) =>
        new(
            NotificationTypes.ExecutionCreated,
            "اجرای خدمت ایجاد شد",
            "یک اجرای خدمت برای معامله ثبت شد.",
            NotificationEntityTypes.ServiceExecution,
            executionId,
            "/provider/executions");

    public static NotificationDefinition ExecutionScheduled(long executionId) =>
        new(
            NotificationTypes.ExecutionScheduled,
            "زمان‌بندی اجرا تغییر کرد",
            "زمان‌بندی اجرای خدمت به‌روزرسانی شد.",
            NotificationEntityTypes.ServiceExecution,
            executionId,
            "/provider/executions");

    public static NotificationDefinition ProviderAssigned(long assignmentId) =>
        new(
            NotificationTypes.ProviderAssigned,
            "به اجرا اختصاص داده شدید",
            "شما به یک اجرای خدمت اختصاص داده شدید.",
            NotificationEntityTypes.ExecutionAssignment,
            assignmentId,
            "/provider/executions");

    public static NotificationDefinition ExecutionStarted(long executionId) =>
        new(
            NotificationTypes.ExecutionStarted,
            "اجرا شروع شد",
            "اجرای خدمت شروع شده است.",
            NotificationEntityTypes.ServiceExecution,
            executionId,
            "/provider/executions");

    public static NotificationDefinition ExecutionCompleted(long executionId) =>
        new(
            NotificationTypes.ExecutionCompleted,
            "اجرا تکمیل شد",
            "اجرای خدمت تکمیل شد.",
            NotificationEntityTypes.ServiceExecution,
            executionId,
            "/customer/deals");

    public static NotificationDefinition ExecutionCancelled(long executionId) =>
        new(
            NotificationTypes.ExecutionCancelled,
            "اجرا لغو شد",
            "اجرای خدمت لغو شد.",
            NotificationEntityTypes.ServiceExecution,
            executionId,
            "/provider/executions");
}
