namespace Automate.Cli;

internal class ProgramErrorCodes
{
    #region Basic Codes
    public const int Success = 0;
    public const int Error = 1;
    #endregion

    #region Error codes for Analyze verb
    public const int Analyze_CriticalFailure = -100;
    public const int Analyze_GeneratedReport_BillableFileDefaulted = -101;
    public const int Analyze_GeneratedReport_ReportLocDefaulted = -102;
    public const int Analyze_GeneratedReport_QueryDefaulted = -103;
    public const int Analyze_GeneratedReport_FileAndReportDefaulted = -104;
    public const int Analyze_GeneratedReport_ReportAndQueryDefaulted = -105;
    public const int Analyze_GeneratedReport_FileAndQueryDefaulted = -106;
    public const int Analyze_GeneratedReport_AllFilesDefaulted = -107;
    public const int Analyze_FailedReport_BillableFileDefaulted = -111;
    public const int Analyze_FailedReport_ReportLocDefaulted = -112;
    public const int Analyze_FailedReport_QueryDefaulted = -113;
    public const int Analyze_FailedReport_FileAndReportDefaulted = -114;
    public const int Analyze_FailedReport_ReportAndQueryDefaulted = -115;
    public const int Analyze_FailedReport_FileAndQueryDefaulted = -116;
    public const int Analyze_FailedReport_AllFilesDefaulted = -117;
    #endregion

    #region Error codes for Message verb
    /// <summary>
    /// Critical Failure of message analysis. Something serious went wrong.
    /// </summary>
    internal const int Message_CriticalFailure = -200;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndMessageMissing = -201;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndCallMissing = -202;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndCustomerMissing = -203;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndReportMissing = -204;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndMsgAndCallMissing = -205;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndMsgAndCustomerMissing = -206;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndMsgAndReportMissing = -207;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndCallAndCustomerMissing = -208;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndCallAndReportMissing = -209;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndCustomerAndReportMissing = -210;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndAllMissingButText = -211;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndAllMissingButCall = -212;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndAllMissingButCustomer = -213;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndAllMissingButReport = -214;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndAllMissing = -215;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_ResultAndUnknown = -216;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndNoneMissing = -217;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndMessageMissing = -218;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndCallMissing = -219;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndCustomerMissing = -220;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndReportMissing = -221;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndMsgAndCallMissing = -222;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndMsgAndCustomerMissing = -223;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndMsgAndReportMissing = -224;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndCallAndCustomerMissing = -225;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndCallAndReportMissing = -226;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndCustomerAndReportMissing = -227;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndAllMissingButText = -228;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndAllMissingButCall = -229;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndAllMissingButCustomer = -230;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndAllMissingButReport = -231;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndAllMissing = -232;
    /// <summary>
    /// 
    /// </summary>
    internal const int Message_NoResultAndUnknown = -233;
    #endregion

    #region Error Codes for Contacts Verb
    /// <summary>
    /// <para>The directory provided by the user did not exist, so another was generated instead</para>
    /// <para>At least one contact was not generated</para>
    /// <para>At least one contact failed to upload</para>
    /// </summary>
    internal const int Contacts_CriticalFailure = -300;
    /// <summary>
    /// This means that the directory provided by the user did not exist, and either a default was used or another had to be created
    /// </summary>
    internal const int Contacts_DirectoryFailed = -301;
    /// <summary>
    /// At least one contact was not generated
    /// </summary>
    internal const int Contacts_ContactGenFailed = 302;
    /// <summary>
    /// At least one contact failed to upload
    /// </summary>
    internal const int Contacts_UploadFailed = -303;
    /// <summary>
    /// <para>The directory provided by the user did not exist so another was generated instead</para>
    /// <para>At least one contact failed to generate</para>
    /// </summary>
    internal const int Contacts_DirectoryAndContactsFailed = -304;
    /// <summary>
    /// <para>The directory provided by the user did not exist so another was generated instead</para>
    /// <para>At least one contact list failed to upload</para>
    /// </summary>
    internal const int Contacts_DirectoryAndUploadFailed = -305;
    /// <summary>
    /// <para>At least one contact was not generated</para>
    /// <para>At least one contact failed to upload</para>
    /// </summary>
    internal const int Contacts_ContactsAndUploadFailed = -306;
    /// <summary>
    /// An unknown error occurred
    /// </summary>
    internal const int Contacts_Unknown = -307;
    #endregion

    #region Error Codes for Convert Json to Csv Verb

    #endregion
}
