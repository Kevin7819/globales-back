using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api_Orbis_Project.Services
{
    public interface IExpoPushService
    {
        Task SendAsync(IEnumerable<string> tokens, string title, string message, object? data = null);
    }
}