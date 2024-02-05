using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Services.CachingServices
{
    public class RedisCachingService
    {
        private static readonly Lazy<RedisCachingService> InstanceHolder =
            new Lazy<RedisCachingService>(() => new RedisCachingService());

        private readonly ConnectionMultiplexer _client;
        private readonly IDictionary<string, IList<string>> _keys;
        private readonly string _keyPattern;

        private readonly Object _objectLock = new Object();

        public static RedisCachingService Instance
        {
            get { return InstanceHolder.Value; }
        }

        private RedisCachingService()
        {
            try
            {
                _client = ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true");
            }
            catch (RedisConnectionException)
            {
                _client = null;
            }

            _keys = new Dictionary<string, IList<string>>();
            _keyPattern = "{0}:{1}";
        }

        /// <summary>
        /// Returns cached data from Redis if exists, or get it from DB and store in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generalKey">Key that represents the general object on which concrete request depends</param>
        /// <param name="concreteKey">Key that represents the concrete request to the DB</param>
        /// <param name="getter">Function which returns requested data from DB</param>
        /// <returns></returns>
        public async Task<T> GetCachedAsync<T>(string generalKey, string concreteKey, Func<Task<T>> getter) where T : class
        {
            if (_client == null)
            {
                //TODO: Add Logging
                //LoggingHelper.GetLoggingHelper().LogInfo(String.Format("Can't get cached Redis data. Redis serice wasn't found. Key: {0}", key));
                return await getter();
            }

            T result;
            var redisDb = _client.GetDatabase();
            string key = GetRedisCacheKey(generalKey, concreteKey);

            var value = await redisDb.StringGetAsync(key);
            if (!value.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(value);

                return result;
            }

            result = await getter();

            await redisDb.StringSetAsync(key, JsonConvert.SerializeObject(result, Formatting.Indented,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }));

            return result;
        }

        /// <summary>
        /// Returns cached data from Redis if exists, or get it from DB and store in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generalKey">Key that represents the general object on which concrete request depends</param>
        /// <param name="concreteKey">Key that represents the concrete request to the DB</param>
        /// <param name="getter">Function which returns requested data from DB</param>
        /// <returns></returns>
        public T GetCached<T>(string generalKey, string concreteKey, Func<T> getter) where T : class
        {
            if (_client == null)
            {
                //TODO: Add Logging
                //LoggingHelper.GetLoggingHelper().LogInfo(String.Format("Can't get cached Redis data. Redis serice wasn't found. Key: {0}", generalKey));
                return getter();
            }

            T result;
            var redisDb = _client.GetDatabase();
            string key = GetRedisCacheKey(generalKey, concreteKey);

            var value = redisDb.StringGet(key);
            if (!value.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(value);

                return result;
            }

            result = getter();

            redisDb.StringSet(key, JsonConvert.SerializeObject(result, Formatting.Indented,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }));

            return result;
        }

        /// <summary>
        /// Removes all cached data from Redis which dpends on generalKey
        /// </summary>
        /// <param name="generalKey">Key that represents the general object on which concrete request depends</param>
        public void DeleteKey(string generalKey)
        {
            if (_client == null)
            {
                //TODO: Add Logging
                //LoggingHelper.GetLoggingHelper().LogInfo(String.Format("Can't delete Redis key. Redis serice wasn't found. Key: {0}", generalKey));
                return;
            }

            var redisDb = _client.GetDatabase();
            IEnumerable<string> relativeKeys = RemoveRedisCacheKeys(generalKey);

            foreach (var key in relativeKeys)
            {
                redisDb.KeyDelete(key);
            }
        }

        /// <summary>
        /// Removes all cached data from all Redis databases
        /// </summary>
        public void DeleteAllKeys()
        {
            if (_client == null)
            {
                //TODO: Add Logging
                //LoggingHelper.GetLoggingHelper().LogInfo("Can't delete Redis keys. Redis serice wasn't found.");
                return;
            }

            foreach (var endPoint in _client.GetEndPoints())
            {
                var server = _client.GetServer(endPoint);
                server.FlushAllDatabases();
            }

            lock (_objectLock)
            {
                _keys.Clear();
            }
        }

        #region Private Helpers

        private string GetRedisCacheKey(string generalKey, string concreteKey)
        {
            lock (_objectLock)
            {
                if (_keys.ContainsKey(generalKey))
                {
                    if (!_keys[generalKey].Contains(concreteKey))
                    {
                        _keys[generalKey].Add(concreteKey);
                    }
                }
                else
                {
                    _keys.Add(generalKey, new List<string> { concreteKey });
                }
            }

            return String.Format(_keyPattern, generalKey, concreteKey);
        }

        private IEnumerable<string> RemoveRedisCacheKeys(string generalKey)
        {
            var keys = new List<string>();

            lock (_objectLock)
            {
                if (_keys.ContainsKey(generalKey))
                {
                    keys.AddRange(_keys[generalKey].Select(concreteKey => String.Format(_keyPattern, generalKey, concreteKey)));
                    _keys[generalKey].Clear();
                }
            }

            return keys;
        }

        #endregion
    }
}
