namespace ScripturAI.Models;

public class FailedTranslation
{
  public Verse? Verse { get; set; }
  public DateTime Timestamp { get; set; }
  public string? Reason { get; set; }
}
