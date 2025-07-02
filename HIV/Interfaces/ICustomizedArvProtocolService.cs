// ICustomizedArvProtocolService.cs
using HIV.DTOs;
using HIV.DTOs.DTOARVs;

namespace HIV.Interfaces.ARVinterfaces
{
    public interface ICustomizedArvProtocolService
    {
        Task<List<PatientWithProtocolDto>> GetPatientsWithProtocolsAsync(int doctorId);
        Task<FullCustomProtocolDto> GetPatientCurrentProtocolAsync(int patientId);
        Task<FullCustomProtocolDto> CreateCustomProtocolAsync(int doctorId, int patientId, CreateCustomProtocolRequest request);
        Task<bool> UpdatePatientProtocolAsync(int patientId, UpdatePatientProtocolRequest request);
        Task<List<FullCustomProtocolDto>> GetPatientProtocolHistoryAsync(int patientId);
        Task<bool> UpdateProtocolAsync(int protocolId, UpdateCustomProtocolDto dto);
    }
}