using Microsoft.Extensions.Configuration;
using Serilog;

namespace MewingPad.Utils.AudioManager;

public class AudioManager
{
    private readonly HttpClient _client;
    private readonly ILogger _logger = Log.ForContext<AudioManager>();
    private readonly Uri _baseUri;
    private readonly IConfiguration _config;

    public AudioManager() { }

    public AudioManager(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _client = httpClient;
        _baseUri = new Uri(_config["ApiSettings:AudioServerAddress"]!);
    }

    public virtual async Task<Stream?> GetFileStreamAsync(string srcpath)
    {
        _logger.Verbose("Entering GetFileStreamAsync method");
        Stream stream;
        try
        {
            stream = await _client.GetStreamAsync(
                _baseUri + $"audiotracks/{srcpath}"
            );
        }
        catch (Exception ex)
        {
            _logger.Error("Exception occurred: {Message}", ex);
            return null;
        }
        _logger.Verbose("Exiting GetFileStreamAsync method");
        return stream;
    }

    public virtual async Task<bool> CreateFileFromStreamAsync(
        Stream fileStream,
        string fileName
    )
    {
        _logger.Verbose("Entering CreateFileFromStreamAsync method");

        try
        {
            using var content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "audio", fileName },
            };

            using var response = await _client.PostAsync(
                _baseUri + "audiotracks",
                content
            );
            if (response.IsSuccessStatusCode)
            {
                _logger.Information($"File '{fileName}' uploaded successfully");
            }
            else
            {
                _logger.Error(
                    $"Failed to upload file '{fileName}'. Status: {response.StatusCode}"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.Error(
                ex,
                "Error creating file from stream. Reason: {ErrorMessage}",
                ex.Message
            );
            return false;
        }

        _logger.Verbose("Exiting CreateFileFromStreamAsync method");
        return true;
    }

    public virtual async Task<bool> DeleteFileAsync(string filepath)
    {
        _logger.Verbose("Entering DeleteFileAsync method");

        try
        {
            HttpResponseMessage response = await _client.DeleteAsync(
                _baseUri + $"audiotracks/{filepath}"
            );
            if (response.IsSuccessStatusCode)
            {
                _logger.Information($"File '{filepath}' deleted successfully");
            }
            else
            {
                _logger.Error(
                    $"Failed to delete file '{filepath}'. Status: {response.StatusCode}"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Exception occurred", ex);
            return false;
        }

        _logger.Verbose("Exiting DeleteFileAsync method");
        return true;
    }

    public virtual async Task<bool> UpdateFileFromStreamAsync(
        Stream fileStream,
        string filename
    )
    {
        _logger.Verbose("Entering UpdateFileFromStreamAsync");

        try
        {
            using var content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "audio", filename },
            };

            using var response = await _client.PutAsync(
                _baseUri + $"audiotracks/{filename}",
                content
            );
            if (response.IsSuccessStatusCode)
            {
                _logger.Information($"File '{filename}' uploaded successfully");
            }
            else
            {
                _logger.Error(
                    $"Failed to upload file '{filename}'. Status: {response.StatusCode}"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.Error(
                ex,
                "Error creating file from stream. Reason: {Message}"
            );
            return false;
        }

        _logger.Verbose("Exiting UpdateFileFromStreamAsync");
        return true;
    }
}
