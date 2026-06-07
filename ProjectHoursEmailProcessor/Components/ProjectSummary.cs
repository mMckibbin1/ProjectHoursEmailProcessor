using EmailParser;

namespace ProjectHoursEmailProcessor.Components;

public sealed class ProjectSummary(
    List<ApprovedHoursEmail> approvedHoursEmails,
    List<SentForApprovalEmail> sentForApprovalEmails,
    List<RejectionEmail> rejectionEmails)
{
    public bool HasData => (approvedHoursEmails.Count +
        sentForApprovalEmails.Count +
        rejectionEmails.Count) > 0;

    public string? BusinessName => GetBusinessName();
    public string? ClientName => GetClientName();
    public string? MentorName => GetMentorName();
    public double? TotalHoursSubmitted => GetTotalHoursSubmitted();
    public double? TotalHoursApproved => GetTotalHoursApproved();

    //public double? TotalHoursRejected => GetTotalHoursRejected();

    public int NumberOfSubmissions => approvedHoursEmails.Count +
        sentForApprovalEmails.Count +
        rejectionEmails.Count;

    public int ApprovedSubmissions => approvedHoursEmails.Count;
    public int RejectedSubmissions => rejectionEmails.Count;
    public int SentForApproval => sentForApprovalEmails.Count;

    private string? GetBusinessName()
    {
        if(approvedHoursEmails.Count > 0)
        {
            return approvedHoursEmails[0].BusinessName;
        }

        if (sentForApprovalEmails.Count > 0)
        {
            return sentForApprovalEmails[0].BusinessName;
        }

        if (rejectionEmails.Count > 0)
        {
            return rejectionEmails[0].BusinessName;
        }

        return null;
    }

    private string? GetClientName()
    {
        if (approvedHoursEmails.Count > 0)
        {
            return approvedHoursEmails[0].ClientName;
        }

        if (sentForApprovalEmails.Count > 0)
        {
            return null;
        }

        if (rejectionEmails.Count > 0)
        {
            return rejectionEmails[0].BusinessClientName;
        }

        return null;
    }

    private string? GetMentorName()
    {
        if (approvedHoursEmails.Count > 0)
        {
            return approvedHoursEmails[0].MentorName;
        }

        if (sentForApprovalEmails.Count > 0)
        {
            return sentForApprovalEmails[0].MentorName;
        }

        if (rejectionEmails.Count > 0)
        {
            return rejectionEmails[0].MentorName;
        }

        return null;
    }

    private double? GetTotalHoursSubmitted()
    {
        if (sentForApprovalEmails.Count > 0)
        {
            return sentForApprovalEmails.Sum(x => x.HoursCompleted);
        }

        return null;
    }

    private double? GetTotalHoursApproved()
    {
        if (approvedHoursEmails.Count > 0)
        {
            return approvedHoursEmails.Sum(x => x.CompletedHours);
        }

        return null;
    }

}
