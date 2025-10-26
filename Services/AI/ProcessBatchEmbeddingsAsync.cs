using ScripturAI.Models;

namespace ScripturAI.Services;

public partial class AiService
{
  internal static async Task ProcessBatchEmbeddingsAsync(List<Verse> batch)
  {
    // 🧹 Remove any verses that failed translation
    var validBatch = batch
      .Where(v => !string.IsNullOrWhiteSpace(v.text))
      .ToList();

    if (validBatch.Count == 0)
    {
      Console.WriteLine("Skipping embedding: all verses in this batch are empty.");
      return;
    }

    // 🧠 Generate embeddings for valid verses only
    var texts = validBatch.ConvertAll(v => v.text);
    var embeddingsResponse = await GetEmbeddingClient().GenerateEmbeddingsAsync(texts);
    var embeddings = embeddingsResponse.Value;

    for (int j = 0; j < validBatch.Count; j++)
    {
      if (embeddings[j] != null)
      {
        validBatch[j].vector = embeddings[j].ToFloats().ToArray();
      }
    }

    // ☁️ Upsert only valid verses to Cosmos DB
    foreach (var verse in validBatch)
    {
      await DataService.UpsertVerseAsync(verse);
    }
  }
}
