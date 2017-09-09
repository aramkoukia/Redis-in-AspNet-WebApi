using Newtonsoft.Json;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi2.Controllers
{
    public class RedisController : ApiController
    {
        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        // GET: api/Redis/name
        public async Task<Person> Get(string name)
        {
            IDatabase cache = Connection.GetDatabase();
            var person = await cache.StringGetAsync(name);
            return JsonConvert.DeserializeObject<Person>(person);
        }

        // POST: api/Redis
        public async Task Post(int age, string name)
        {
            Person person = new Person();
            person.Age = age;
            person.Name = name;
            IDatabase cache = Connection.GetDatabase();
            await cache.StringSetAsync(name, JsonConvert.SerializeObject(person));
        }        
    }

    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }
}
