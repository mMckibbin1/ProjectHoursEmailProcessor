using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace EmailParser;

public static class EmailDataParser
{
    public static async Task<AggregatedEmailData> ParseAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        csvReader.Context.RegisterClassMap<CSVDataMap>();

        IAsyncEnumerable<CSVData> records = csvReader.GetRecordsAsync<CSVData>();

        return await AggregatedEmailData.ParseCSVData(records);
    }
}

internal sealed class CSVDataMap : ClassMap<CSVData>
{
    public CSVDataMap()
    {
        Map(m => m.Subject).Name("Subject");
        Map(m => m.Body).Convert(args => args.Row.GetField("Body")?.Replace("\r", ""));
        Map(m => m.ToAddress).Name("To: (Address)");
        Map(m => m.ToName).Name("To: (Name)");
    }
}


internal sealed class CSVData
{
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public required string ToAddress { get; init; }
    public required string ToName { get; init; }
}


public sealed class ApprovedHoursEmail
{
    public required string MentorName { get; init; }
    public required string BusinessName { get; init; }
    public required string ClientName { get; init; }
    public required DateOnly SessionDate { get; init; }
    public required double CompletedHours { get; init; }
    public required double TotalHoursToDate { get; init; }
    public required bool FinalSession { get; init; }
    public required string SessionDetails { get; init; }

    private ApprovedHoursEmail() { }

    internal static ApprovedHoursEmail FromCSVData(CSVData data)
    {
        string[] splitBody = data.Body.Split("\n");

        bool finalSession = splitBody[42].Equals("no", StringComparison.CurrentCultureIgnoreCase)
            ? false
            : splitBody[42].Equals("yes", StringComparison.CurrentCultureIgnoreCase) ? true
            : throw new Exception();

        return new()
        {
            MentorName = splitBody[48],
            BusinessName = splitBody[11],
            SessionDate = DateOnly.Parse(splitBody[16]),
            CompletedHours = double.Parse(splitBody[21]),
            SessionDetails = splitBody[26],
            ClientName = splitBody[32],
            TotalHoursToDate = double.Parse(splitBody[37]),
            FinalSession = finalSession
        };
    }
}

public sealed class SentForApprovalEmail
{
    public required string MentorName { get; init; }
    public required string BusinessName { get; init; }
    public required DateOnly SessionDate { get; init; }
    public required double HoursCompleted { get; init; }
    public required double TotalHoursToDate { get; init; }
    public required bool FinalSession { get; init; }
    public required string SessionDetails { get; init; }

    private SentForApprovalEmail() { }

    internal static SentForApprovalEmail FromCSVData(CSVData data)
    {
        string[] splitBody = data.Body.Split("\n");

        bool isFinalSession = splitBody[30].Equals("no", StringComparison.CurrentCultureIgnoreCase)
            ? false
            : splitBody[30].Equals("yes", StringComparison.CurrentCultureIgnoreCase) ? true
            : throw new Exception();

        int index = data.ToName.IndexOf(';');
        string mentorName = data.ToName[..index];

        return new()
        {
            MentorName = mentorName,
            BusinessName = splitBody[5],
            SessionDate = DateOnly.Parse(splitBody[10]),
            HoursCompleted = double.Parse(splitBody[15]),
            SessionDetails = splitBody[20],
            TotalHoursToDate = double.Parse(splitBody[25]),
            FinalSession = isFinalSession
        };
    }
}

public sealed class RejectionEmail
{
    public required string BusinessName { get; init; }
    public required string BusinessClientName { get; init; }
    public required string MentorName { get; init; }
    public required string MentorEmail { get; init; }
    public required DateOnly SessionDate { get; init; }

    private RejectionEmail() { }

    internal static RejectionEmail FromCSVData(CSVData data)
    {
        string[] splitBody = data.Body.Split("\n");

        var span = data.Subject.AsSpan();
        int businessNameIndex = span.IndexOf("Mentoring");
        var bs = span[..businessNameIndex].Trim().ToString();

        int index = data.Subject.LastIndexOf("-") + 1;
        string date = data.Subject.Substring(index).Trim();
        DateOnly sessionDate = DateOnly.Parse(date);

        return new()
        {
            BusinessName = bs,
            BusinessClientName = splitBody[0].Substring(3, splitBody[0].Length - 4),
            MentorEmail = splitBody[13].Substring(7),
            MentorName = splitBody[11],
            SessionDate = sessionDate
        };
    }
}

public sealed class OtherEmail
{
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public required string ToAddress { get; init; }

    private OtherEmail() { }

    internal static OtherEmail FromCSVData(CSVData data)
    {
        return new()
        {
            Body = data.Body,
            Subject = data.Subject,
            ToAddress = data.ToAddress
        };
    }
}


public sealed class AggregatedEmailData
{
    public IReadOnlyList<ApprovedHoursEmail> ApprovedHoursEmails { get; private init; } = [];
    public IReadOnlyList<SentForApprovalEmail> SentForApprovalEmails { get; private init; } = [];
    public IReadOnlyList<RejectionEmail> RejectionEmails { get; private init; } = [];
    public IReadOnlyList<OtherEmail> OtherEmails { get; private init; } = [];

    private AggregatedEmailData() { }

    internal static async Task<AggregatedEmailData> ParseCSVData(IAsyncEnumerable<CSVData> csvData)
    {
        List<ApprovedHoursEmail> approvedHoursEmails = [];
        List<SentForApprovalEmail> sentForApprovalEmails = [];
        List<RejectionEmail> rejectionEmails = [];
        List<OtherEmail> otherEmails = [];

        await foreach (CSVData data in csvData)
        {
            if (isSentForApprovalSubject(data.Subject))
            {
                sentForApprovalEmails.Add(SentForApprovalEmail.FromCSVData(data));
            }
            else if (isApprovedApprovedHoursSubject(data.Subject))
            {
                approvedHoursEmails.Add(ApprovedHoursEmail.FromCSVData(data));
            }
            else if (isRejectionEmailSubject(data.Subject))
            {
                rejectionEmails.Add(RejectionEmail.FromCSVData(data));
            }
            else
            {
                otherEmails.Add(OtherEmail.FromCSVData(data));
            }
        }

        AggregatedEmailData aggregated = new()
        {
            ApprovedHoursEmails = approvedHoursEmails,
            SentForApprovalEmails = sentForApprovalEmails,
            RejectionEmails = rejectionEmails,
            OtherEmails = otherEmails
        };

        return aggregated;

        static bool isSentForApprovalSubject(string subject) => subject.Contains("successfully sent to");
        static bool isApprovedApprovedHoursSubject(string subject) => subject.Contains("Mentoring Hours Approval -");
        static bool isRejectionEmailSubject(string subject) => subject.Contains(" Mentoring Hours Rejection");
    }
}
