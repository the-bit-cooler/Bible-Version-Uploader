namespace ScripturAI.Models;

public class Verse
{
  public string? id { get; set; } // Unique: "Book:Chapter:Verse:Version"
  public string? verseId { get; set; } // For multi-translation queries
  public string type => "bible-verse";
  public string? version { get; set; } // e.g., "KJV"
  public string? collection { get; set; } // Same as book (Partition Key)
  public string? book { get; set; }
  public int chapter { get; set; }
  public int verse { get; set; }
  public string? text { get; set; }
  public float[] vector { get; set; } = [];
}
