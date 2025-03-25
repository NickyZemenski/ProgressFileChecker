using ServiceApp;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Newtonsoft.Json.JsonConvert;

internal class Program
{
    #region User Info

    public static string username= "interview.nikola.zemenski";
    public static string password= "31Q{Jmv5";
    public static string folderPath = "D:\\Test";// enter


    #endregion 


    #region runtimeVariables
    private static FileSystemWatcher _fileWatcher;
    private static TokenRequestModel tokenRequest;
    private static TokenResponseModel tokenResponse;
    private static UserResponseModel userResponse;
    private static FilesResponseModel homeFolderFiles;


    #endregion
    #region uri

    private const string moveItBaseUrL = "https://testserver.moveitcloud.com/api/v1"; 

    private const string moveItAuthUrl = $"{moveItBaseUrL}/token";

    private const string moveItUserUrl = $"{moveItBaseUrL}/users/self";

    private static string moveItGetFiles(string folderId) => $"{moveItBaseUrL}/folders/{folderId}/files";

    #endregion
    #region web requests

    static async Task<string> Post(string uri, string body, string token=null)
    {
        using (HttpClient client = new HttpClient())
        {
            if (!string.IsNullOrWhiteSpace(token)) { // check
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);// set
            }

            var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            try
            {
                HttpResponseMessage response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                return "{ }";            
            }
        }
    }
    static async Task<string> FileUpload(string uri, RequestFileUploadModel parameters, string token = null)
    {

        HttpClient client = new HttpClient();
 
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, moveItGetFiles(parameters.Id));

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Authorization", $"Bearer {token}");

        MultipartFormDataContent content = new MultipartFormDataContent();
        content.Add(new StringContent(parameters.hashtype), "hashtype");
        content.Add(new StringContent(parameters.hash), "hash");
        content.Add(new ByteArrayContent(File.ReadAllBytes(parameters.file)), "file", Path.GetFileName(parameters.file));
        content.Add(new StringContent(parameters.comments), "comments");
        request.Content = content;
      //  request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

        HttpResponseMessage response = client.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();
        string responseBody = response.Content.ReadAsStringAsync().Result;
        return responseBody;
    }

    static async Task<string> Get(string uri, string body, string token = null)
    {
        using (HttpClient client = new HttpClient())
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
              
                return responseBody;
            }
            catch (Exception ex)
            {
                return "{ }";
            }
        }
    }
    #endregion
    #region monitoring

    private static void StartFolderMonitoring()
    {
        
        
            string filter = "*.*";

            if (_fileWatcher != null)
            {
                _fileWatcher.Dispose();
            }

            _fileWatcher = new FileSystemWatcher(folderPath)
            {
                NotifyFilter = NotifyFilters.FileName,     // usually its a good idea to check if something gets updated
                                                           // but due to the specification saying only add "nww files" and not modified i havent added NotifyFilters.LastWrite

                Filter = $"{filter}",   // if we want /txt or other specifics only we can easily change our filter var
                EnableRaisingEvents = true
            };

            _fileWatcher.Created += async (sender, e) => await OnFileCreated(e);
        while (Console.Read() != 'q') ;
    }
    private static async Task OnFileCreated(FileSystemEventArgs e)
    {
        RequestFileUploadModel fileRequest = new RequestFileUploadModel() { Id = userResponse.homeFolderID.ToString(), hash = RequestFileUploadModel.GetFileHash(e.FullPath), file = e.FullPath };
        FileUpload(moveItGetFiles(userResponse.homeFolderID.ToString()),  fileRequest, tokenResponse.access_token);
    }


    #endregion
    private static void Main(string[] args)
    {

         tokenRequest = new TokenRequestModel() { username = username, password = password };

         tokenResponse = DeserializeObject<TokenResponseModel>(Post(moveItAuthUrl, tokenRequest.ToString()).Result);
         userResponse = DeserializeObject<UserResponseModel>(Get(moveItUserUrl, "", tokenResponse.access_token).Result);
         homeFolderFiles = DeserializeObject<FilesResponseModel>(Get(moveItGetFiles(userResponse.homeFolderID.ToString()), "", tokenResponse.access_token).Result);

        StartFolderMonitoring();
 

    }


}