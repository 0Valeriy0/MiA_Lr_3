using MiA_Lr_3.Models;

namespace MiA_Lr_3.Services.Interface
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}