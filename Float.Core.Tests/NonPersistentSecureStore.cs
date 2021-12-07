using System.Threading.Tasks;
using System.Collections.Generic;
using Float.Core.Persistence;

namespace Float.Core.Tests
{
    public class NonPersistentSecureStore : ISecureStore
    {
        readonly IDictionary<string, string> values = new Dictionary<string, string>();

        public bool Delete(string key)
        {
            values.Remove(key);
            return true;
        }

        public string Get(string key)
        {
            return values[key];
        }

        public Task<string> GetAsync(string key)
        {
            return Task.FromResult(Get(key));
        }

        public bool Put(string key, string str)
        {
            values[key] = str;
            return true;
        }
    }
}
