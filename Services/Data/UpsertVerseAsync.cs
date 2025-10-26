using Microsoft.Azure.Cosmos;
using ScripturAI.Models;

namespace ScripturAI.Services;

public partial class DataService
{
  internal static async Task UpsertVerseAsync(Verse verse)
  {
    await GetDataContainer().UpsertItemAsync(verse, new PartitionKey(verse.collection));
  }
}
