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
        public async Task<IHttpActionResult> Get(string name)
        {
            IDatabase cache = Connection.GetDatabase();
            var person = await cache.StringGetAsync(name);
            if (person.HasValue)
                return Ok(JsonConvert.DeserializeObject<Person>(person));
            else
                return Ok(new Person());
        }

        // POST: api/Redis
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Person person)
        {
            IDatabase cache = Connection.GetDatabase();
            await cache.StringSetAsync(person.Name, JsonConvert.SerializeObject(person));
            return Ok();
        }        
    }

    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }
}
