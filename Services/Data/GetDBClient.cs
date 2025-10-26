using Microsoft.Azure.Cosmos;

namespace ScripturAI.Services;

public partial class DataService
{
  static readonly CosmosClient dbClient = new(Environment.GetEnvironmentVariable("COSMOS_URL"), Environment.GetEnvironmentVariable("COSMOS_KEY"));
}
