using System.Net;
using System.Threading.Tasks;
using RakSharp;

namespace Lantern;

public static class Program {
    
    public async static Task Main() {

        var server = new Server(new IPEndPoint(IPAddress.Any, 19132));
        await server.Start();
        
        // test
    }
}