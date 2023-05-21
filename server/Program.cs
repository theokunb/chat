using System.Threading.Tasks;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            Start().Wait();
        }

        private static async Task Start()
        {
            using(Server server = new Server())
            {
                await server.ListenAsync();
            }
        }
    }
}