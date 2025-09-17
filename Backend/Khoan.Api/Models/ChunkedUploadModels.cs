namespace Khoan.Api.Models
{
    /// <summary>
    /// Request để tạo upload session cho chunked upload
    /// </summary>
    public class CreateSessionRequest
    {
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int TotalChunks { get; set; }
        public string DataType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request để upload chunk
    /// </summary>
    public class UploadChunkRequest
    {
        public string SessionId { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public IFormFile Chunk { get; set; } = null!;
    }

    /// <summary>
    /// Upload session info
    /// </summary>
    public class UploadSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int TotalChunks { get; set; }
        public string DataType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<int> UploadedChunks { get; set; } = new();
        public string? TempDirectoryPath { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Chunk upload result
    /// </summary>
    public class ChunkUploadResult
    {
        public bool Success { get; set; }
        public int ChunkIndex { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public long ChunkSize { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Upload info response (for resume functionality)
    /// </summary>
    public class UploadInfoResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int TotalChunks { get; set; }
        public List<int> UploadedChunks { get; set; } = new();
        public int RemainingChunks { get; set; }
        public double ProgressPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}
