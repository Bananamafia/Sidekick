using System.Threading;
using System.Threading.Tasks;

namespace Sidekick.Domain.Process
{
    public interface INativeProcess
    {
        Mutex Mutex { get; set; }
        bool IsPathOfExileInFocus { get; }
        bool IsSidekickInFocus { get; }
        Task CheckPermission();
        string ClientLogPath { get; }
    }
}
