using System.Threading.Tasks;
using OWCE.DependencyInterfaces;

namespace OWCE.iOS.DependencyImplementations
{
    public class PermissionPrompt : IPermissionPrompt
    {
        public Task<bool> PromptBLEPermission()
        {
            return Task.FromResult(true);
        }
    }
}
