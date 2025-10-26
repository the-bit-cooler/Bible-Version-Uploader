using System.Net.Http.Json;
using ScripturAI.Models;

namespace ScripturAI.Services;

public partial class GitHubService
{
  public static async Task<ScrollMapperBiblicalBooks?> FetchScrollMapperBibleAsync(string version)
  {
    try
    {
      GitHubFileRef? bookListRef = await FetchScrollMapperBookListRefAsync();
      if (bookListRef is null)
      {
        return default;
      }

      using HttpClient httpClient = new();

      // GitHub API requires User-Agent
      httpClient.DefaultRequestHeaders.Add("User-Agent", "ScripturAI");

      List<ScrollMapperBiblicalBookListObject>? bookListObjectRefs = await httpClient.GetFromJsonAsync<List<ScrollMapperBiblicalBookListObject>>(bookListRef.download_url);

      ScrollMapperBiblicalBookListObject? bookRef = bookListObjectRefs?.Where(o => o.book?.Equals(version, StringComparison.OrdinalIgnoreCase) == true).FirstOrDefault();
      if (bookRef is null)
      {
        return default;
      }

      ScrollMapperBiblicalBooks? bible = await httpClient.GetFromJsonAsync<ScrollMapperBiblicalBooks>(bookRef.source);
      if (bible is null)
      {
        return default;
      }

      bible.version = bookRef.book;

      return bible;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"{nameof(GitHubService)}.{nameof(FetchScrollMapperBibleAsync)}: Error fetching {version} from scrollmapper GitHub: {ex.Message}");
    }

    return default;
  }

  private static async Task<GitHubFileRef?> FetchScrollMapperBookListRefAsync()
  {
    try
    {
      using HttpClient httpClient = new();

      // GitHub API requires User-Agent
      httpClient.DefaultRequestHeaders.Add("User-Agent", "ScripturAI");

      List<GitHubFileRef>? response = await httpClient.GetFromJsonAsync<List<GitHubFileRef>>($"https://api.github.com/repos/scrollmapper/book_list/contents");

      return response?.Where(f => f.name?.Equals("book_list.json", StringComparison.OrdinalIgnoreCase) == true).FirstOrDefault();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"{nameof(GitHubService)}.{nameof(FetchScrollMapperBookListRefAsync)}: Error fetching scrollmapper file refs from GitHub: {ex.Message}");
    }

    return default;
  }
}
