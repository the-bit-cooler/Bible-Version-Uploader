using System.Text.Json;
using ScripturAI.Models;
using ScripturAI.Services;

Console.Write("Enter a valid bible version: ");
string? version = Console.ReadLine();
if (string.IsNullOrWhiteSpace(version))
{
  Console.WriteLine("Please provide valid bible version to scrape from ScrollMapper GitHub.");
  return;
}

await ScrollMapper(version);

static async Task ScrollMapper(string version)
{
  ScrollMapperBiblicalBooks? bible = await GitHubService.FetchScrollMapperBibleAsync(version);
  if (bible is null || bible.books.Count < 1)
  {
    Console.WriteLine($"{nameof(GitHubService)}.{nameof(GitHubService.FetchScrollMapperBibleAsync)}({version}): Nothing to process.");
    return;
  }

  // Load or initialize processed books tracker
  string processedFilePath = $"processed_{bible.version!.ToLower().Replace(" ", "_")}_books.json";
  List<string> processedBooks = LoadProcessedBooks(processedFilePath);

  // Process each file one by one
  const int maxRetries = 3;
  const int batchSize = 100; // Adjust based on OpenAI rate limits

  foreach (var bibleBook in bible.books)
  {
    if (string.IsNullOrEmpty(bibleBook.name))
    {
      Console.WriteLine($"Skipping a book with no name.");
      continue;
    }

    if (!BibleBookMap.Map.TryGetValue(bibleBook.name, out string? bibleBookName))
    {
      Console.WriteLine($"Unknown book: {bibleBook.name}");
    }

    if (processedBooks.Contains(bibleBookName, StringComparer.OrdinalIgnoreCase))
    {
      Console.WriteLine($"Skipping {bibleBookName} as it has already been processed.");
      continue;
    }

    // Load verses with retry
    List<Verse> bookVerses = [];
    foreach (var ch in bibleBook.chapters)
    {
      foreach (var v in ch.verses)
      {
        bookVerses.Add(new Verse
        {
          id = $"{bibleBookName}:{ch.chapter}:{v.verse}:{bible.version}",
          verseId = $"{bibleBookName}:{ch.chapter}:{v.verse}",
          version = bible.version,
          collection = bibleBookName,
          book = bibleBookName,
          chapter = ch.chapter,
          verse = v.verse,
          text = v.text?.Trim(),
        });
      }
    }

    // Process in batches with retry
    bool allBatchesSucceeded = true;
    for (int i = 0; i < bookVerses.Count; i += batchSize)
    {
      var batch = bookVerses.GetRange(i, Math.Min(batchSize, bookVerses.Count - i));
      bool batchSuccess = await RetryAsync(() => AiService.ProcessBatchEmbeddingsAsync(batch), maxRetries);
      if (!batchSuccess)
      {
        Console.WriteLine($"Failed to process batch starting at index {i} for {bibleBookName} for {bible.version} after {maxRetries} retries.");
        allBatchesSucceeded = false;
        break; // Stop processing this book to avoid partial uploads; manual intervention needed
      }
    }

    if (allBatchesSucceeded)
    {
      processedBooks.Add(bibleBookName!);
      SaveProcessedBooks(processedFilePath, processedBooks);
      Console.WriteLine($"Completed processing {bibleBookName} for {bible.version}.");
    }
    else
    {
      Console.WriteLine($"Partial failure in {bibleBookName} for {bible.version}; not marking as processed.");
    }
  }
}
static List<string> LoadProcessedBooks(string filePath)
{
  if (File.Exists(filePath))
  {
    try
    {
      string json = File.ReadAllText(filePath);
      return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error loading processed books: {ex.Message}. Starting fresh.");
    }
  }
  return new List<string>();
}

static void SaveProcessedBooks(string filePath, List<string> processedBooks)
{
  try
  {
    string json = JsonSerializer.Serialize(processedBooks);
    File.WriteAllText(filePath, json);
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Error saving processed books: {ex.Message}.");
  }
}

static async Task<bool> RetryAsync(Func<Task> action, int maxRetries)
{
  for (int attempt = 1; attempt <= maxRetries; attempt++)
  {
    try
    {
      await action();
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
      if (attempt == maxRetries)
      {
        return false;
      }
      await Task.Delay(1000 * attempt); // Exponential backoff in ms
    }
  }
  return false;
}
