namespace StatsClient.MVVM.Model;

public class DebugMessagesModel(string dLocation, string dLineNumber, string dTime, string dMessage)
{
    public string? DLocation { get; set; } = dLocation;
    public string? DLineNumber { get; set; } = dLineNumber;
    public string? DTime { get; set; } = dTime;
    public string? DMessage { get; set; } = dMessage;
}
