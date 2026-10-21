using EmployeeServiceDesk.Domain.Entities;

namespace EmployeeServiceDesk.Application.ServiceInterface
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
    }
}