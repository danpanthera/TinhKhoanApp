using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace TinhKhoanApp.Api.Services
{
    public interface ICompressionService
    {
        Task<(byte[] compressedData, double compressionRatio)> CompressDataAsync(object data);
        Task<T> DecompressDataAsync<T>(byte[] compressedData);
        Task<string> DecompressStringAsync(byte[] compressedData);
    }

    public class CompressionService : ICompressionService
    {
        private readonly ILogger<CompressionService> _logger;

        public CompressionService(ILogger<CompressionService> logger)
        {
            _logger = logger;
        }

        public async Task<(byte[] compressedData, double compressionRatio)> CompressDataAsync(object data)
        {
            try
            {
                // Serialize data to JSON
                var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions 
                { 
                    WriteIndented = false // Compact JSON
                });
                
                var originalBytes = Encoding.UTF8.GetBytes(jsonString);
                var originalSize = originalBytes.Length;

                using var outputStream = new MemoryStream();
                
                // Use Brotli compression (better than GZip for text data)
                using (var brotliStream = new BrotliStream(outputStream, CompressionLevel.Optimal))
                {
                    await brotliStream.WriteAsync(originalBytes, 0, originalBytes.Length);
                }

                var compressedData = outputStream.ToArray();
                var compressedSize = compressedData.Length;
                
                var compressionRatio = originalSize > 0 ? (double)compressedSize / originalSize : 1.0;

                _logger.LogInformation($"Compression completed: {originalSize} bytes -> {compressedSize} bytes (ratio: {compressionRatio:P2})");

                return (compressedData, compressionRatio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compressing data");
                throw;
            }
        }

        public async Task<T> DecompressDataAsync<T>(byte[] compressedData)
        {
            var jsonString = await DecompressStringAsync(compressedData);
            return JsonSerializer.Deserialize<T>(jsonString) ?? throw new InvalidOperationException("Failed to deserialize decompressed data");
        }

        public async Task<string> DecompressStringAsync(byte[] compressedData)
        {
            try
            {
                using var inputStream = new MemoryStream(compressedData);
                using var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress);
                using var outputStream = new MemoryStream();
                
                await brotliStream.CopyToAsync(outputStream);
                
                var decompressedBytes = outputStream.ToArray();
                return Encoding.UTF8.GetString(decompressedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decompressing data");
                throw;
            }
        }
    }
}
