namespace ScripturAI.Models;

public class ScrollMapperBiblicalBookListObject
{
  public string? language { get; set; }
  public string? book { get; set; }
  public string? title { get; set; }
  public string? source { get; set; }
  public string? content_type { get; set; }
}

public class ScrollMapperBiblicalBooks
{
  public string? version { get; set; }
  public List<ScrollMapperBiblicalBook> books { get; set; } = [];
}

public class ScrollMapperBiblicalBook
{
  public string? name { get; set; }
  public List<ScrollMapperBiblicalChapter> chapters { get; set; } = [];
}

public class ScrollMapperBiblicalChapter
{
  public int chapter { get; set; }
  public string? name { get; set; }
  public List<ScrollMapperBiblicalVerse> verses { get; set; } = [];
}

public class ScrollMapperBiblicalVerse
{
  public int verse { get; set; }
  public int chapter { get; set; }
  public string? name { get; set; }
  public string? text { get; set; }
}
