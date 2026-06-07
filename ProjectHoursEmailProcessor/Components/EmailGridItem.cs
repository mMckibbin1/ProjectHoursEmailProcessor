using EmailParser;

namespace ProjectHoursEmailProcessor.Components;

public sealed class EmailGridItem
{
    public required string MentorName { get; init; }
    public required string BusinessName { get; init; }
    public required string? ClientName { get; init; }
    public required DateOnly SessionDate { get; init; }
    public required double? CompletedHours { get; init; }
    public required EmailType EmailType { get; init; }

    private EmailGridItem() { }

    public static EmailGridItem FromEmail(SentForApprovalEmail email)
    {
        return new()
        {
            BusinessName = email.BusinessName,
            ClientName = null,
            CompletedHours = email.HoursCompleted,
            MentorName = email.MentorName,
            SessionDate = email.SessionDate,
            EmailType = EmailType.Approval
        };
    }

    public static EmailGridItem FromEmail(ApprovedHoursEmail email)
    {
        return new()
        {
            BusinessName = email.BusinessName,
            ClientName = email.ClientName,
            CompletedHours = email.CompletedHours,
            MentorName = email.MentorName,
            SessionDate = email.SessionDate,
            EmailType = EmailType.Approved
        };
    }

    public static EmailGridItem FromEmail(RejectionEmail email)
    {
        return new()
        {
            BusinessName = email.BusinessName,
            ClientName = email.BusinessClientName,
            CompletedHours = null,
            MentorName = email.MentorName,
            SessionDate = email.SessionDate,
            EmailType = EmailType.Rejected
        };
    }
}

public enum EmailType
{
    Approval,
    Approved,
    Rejected
}
