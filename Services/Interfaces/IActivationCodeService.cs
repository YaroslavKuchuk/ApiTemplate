using System;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IActivationCodeService
    {
        Task<bool> IsValidateCodeAsync(string code, string entity, DateTime expDate);
        Task<bool> IsActiveCode(string entity);
        Task<string> ReRenderCode(string entity);
    }
}
