using Microsoft.Azure.Cosmos;

namespace ScripturAI.Services;

public partial class DataService
{
  internal static Container GetDataContainer()
  {
    return dbClient
      .GetDatabase(Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME"))
      .GetContainer(Environment.GetEnvironmentVariable("COSMOS_DATA_CONTAINER_NAME"));
  }
}
