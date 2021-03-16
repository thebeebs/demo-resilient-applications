using System.Collections.Generic;
using System.Threading.Tasks;

namespace app
{
    public interface IJokeClient
    {
        Task<IEnumerable<Joke>> Jokes();
    }
}