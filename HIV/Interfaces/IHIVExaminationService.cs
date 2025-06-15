using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface IHIVExaminationService
    {
        /// <summary>
        /// Tạo kết quả xét nghiệm HIV mới
        /// </summary>
        Task<HIVExaminationResponseDTO> CreateHIVExaminationAsync(CreateHIVExaminationDTO createDto);

        /// <summary>
        /// Cập nhật kết quả xét nghiệm HIV
        /// </summary>
        Task<HIVExaminationResponseDTO> UpdateHIVExaminationAsync(int examId, UpdateHIVExaminationDTO updateDto);

        /// <summary>
        /// Lấy thông tin chi tiết một kết quả xét nghiệm
        /// </summary>
        Task<HIVExaminationResponseDTO> GetHIVExaminationByIdAsync(int examId);

        /// <summary>
        /// Lấy lịch sử xét nghiệm của bệnh nhân
        /// </summary>
        Task<List<HIVExaminationResponseDTO>> GetPatientHIVExaminationHistoryAsync(int patientId);

        /// <summary>
        /// Lấy danh sách tất cả kết quả xét nghiệm (có phân trang)
        /// </summary>
        Task<List<HIVExaminationSummaryDTO>> GetAllHIVExaminationsAsync(int page, int pageSize);

        /// <summary>
        /// Lấy danh sách bác sĩ để chọn trong form
        /// </summary>
        Task<List<DoctorSimpleDTO>> GetDoctorsForSelectionAsync();

        /// <summary>
        /// Xóa kết quả xét nghiệm (soft delete)
        /// </summary>
        Task<bool> DeleteHIVExaminationAsync(int examId);

        /// <summary>
        /// Kiểm tra quyền truy cập kết quả xét nghiệm
        /// </summary>
        Task<bool> CanAccessExaminationAsync(int examId, int userId);
    }
}