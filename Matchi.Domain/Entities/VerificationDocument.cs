using Matchi.Domain.Common;

namespace Matchi.Domain.Entities;

public class VerificationDocument : Entity
{
    public long VerificationId { get; private set; };

    public long MediaId { get; private set; };

    public string DocumentType { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public DateTime UploadedAt { get; private set; };

    public DateTime? VerifiedAt { get; private set; };

    public string? RejectReason { get; private set; };

    public Media Media { get; private set; } = null!;

    public Verification Verification { get; private set; } = null!;

    private VerificationDocument()
    {
    }

    public VerificationDocument(long verificationId, long mediaId, string documentType, string status)
    {
        VerificationId = verificationId;
        MediaId = mediaId;
        DocumentType = documentType;
        Status = status;
    }
}
