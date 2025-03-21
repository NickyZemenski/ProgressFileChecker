using Microsoft.AspNetCore.Mvc;
using ProgressFileChecker.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ProgressFileChecker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private static FileSystemWatcher _fileWatcher;

        private static string _authToken;

        private static readonly HttpClient _httpClient = new HttpClient();

        private const string moveItBaseUrL = "https://testserver.moveitcloud.com"; // add my url unsure if its actually this

        private const string moveItAuthUrl = $"{moveItBaseUrL}/auth/token";

        private const string moveItUploadUrl = $"{moveItBaseUrL}/files/upload";

        [HttpPost("start-monitoring")]
        public async Task<IActionResult> StartMonitoring([FromBody] FolderInfo folderInfo)
        {
            if (string.IsNullOrEmpty(folderInfo.Username) || string.IsNullOrEmpty(folderInfo.Password) || string.IsNullOrEmpty(folderInfo.Path))
            {
                return BadRequest("Username, password, and folder path are required.");
            }

            try
            {       
                _authToken = await AuthenticateUser(folderInfo.Username, folderInfo.Password);

                StartFolderMonitoring(folderInfo.Path);

                return Ok($"Monitoring started for folder: {folderInfo.Path}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private async Task<string> AuthenticateUser(string username, string password)
        {
            //create authentication data
            var authData = new
            {
                username,
                password,
                grant_type = "password"
            };
            // serialize to json
            var content = new StringContent(JsonSerializer.Serialize(authData), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(moveItAuthUrl, content);
           
            if (response.IsSuccessStatusCode) // success
            {
                var result = await response.Content.ReadAsStringAsync();
                // get the accesstoken
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(result);

                return tokenResponse.GetProperty("access_token").GetString();
            }
            else // failure
            {
                throw new Exception($"Authentication failed: {response.ReasonPhrase}");
            }
        }

        private void StartFolderMonitoring(string folderPath)
        {
            string filter = "*.*";
            
            if (_fileWatcher != null)
            {
                _fileWatcher.Dispose();
            }

            _fileWatcher = new FileSystemWatcher(folderPath)
            {
                NotifyFilter = NotifyFilters.FileName,     // usually its a good idea to check if something gets updated
                                                           // but due to the specification saying only add "nww files" and not modified i havent added   NotifyFilters.LastWrite

                Filter = $"{filter}",   // if we want /txt or other specifics only we can easily change our filter var
                EnableRaisingEvents = true
            };

            _fileWatcher.Created += async (sender, e) => await OnFileCreated(e);
        }

        private async Task OnFileCreated(FileSystemEventArgs e)
        {
            if (!Directory.Exists(e.FullPath)) 
            {
                await UploadFileToMoveIt(e.FullPath);
            }
        }

        private async Task UploadFileToMoveIt(string filePath)
        {
            var fileName = Path.GetFileName(filePath); 

            using var fileStream = System.IO.File.OpenRead(filePath);

            using var content = new StreamContent(fileStream);

            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file", // name of form mield
                FileName = fileName // whatever our files name is
            };

            using var formData = new MultipartFormDataContent();
            formData.Add(content); // this adds the file to the form we are submitting 

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, moveItUploadUrl)
            {
                Content = formData // set the request content 
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

            var response = await _httpClient.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"File {fileName} uploaded successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to upload {fileName}. Error: {await response.Content.ReadAsStringAsync()}");
            }
        }
    }






}
