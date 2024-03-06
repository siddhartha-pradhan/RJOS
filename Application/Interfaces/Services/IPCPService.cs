using Application.DTOs.PCP;

namespace Application.Interfaces.Services;

public interface IPCPService
{
    Task<PCPResponseDTO> GetPCPQuestionsByClass(int classId, int type);
}