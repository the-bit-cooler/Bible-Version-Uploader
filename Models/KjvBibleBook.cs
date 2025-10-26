namespace ScripturAI.Models;

public class KjvBibleBook
{
  public string? book { get; set; }
  public List<Chapter> chapters { get; set; } = [];
}

public class Chapter
{
  public string? chapter { get; set; }
  public List<VerseRaw> verses { get; set; } = [];
}

public class VerseRaw
{
  public string? verse { get; set; }
  public string? text { get; set; }
}
