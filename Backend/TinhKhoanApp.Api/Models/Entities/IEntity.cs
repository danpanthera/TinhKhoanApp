namespace TinhKhoanApp.Api.Models.Entities
{
    /// <summary>
    /// Base interface cho tất cả entities
    /// Đảm bảo consistency across all table entities
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Primary key - identity column
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Created timestamp - khi record được tạo
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Updated timestamp - lần cuối record được update
        /// </summary>
        DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Base interface cho temporal table entities
    /// Các bảng có temporal table support thêm system versioning columns
    /// </summary>
    public interface ITemporalEntity : IEntity
    {
        /// <summary>
        /// System start time - temporal table system column
        /// </summary>
        DateTime SysStartTime { get; set; }

        /// <summary>
        /// System end time - temporal table system column
        /// </summary>
        DateTime SysEndTime { get; set; }
    }
}
