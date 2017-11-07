using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Trakker.Api.Singletons
{
    public class TrakkerCache
    {
        public static List<string> _CacheList;
        
        private static readonly object SyncObject = new Object();
        
        public static List<string> CacheList
        {
            get
            {
                if (_CacheList == null)
                {
                    // Instance must not exist before locking
                    lock (SyncObject)
                    {
                        // Double-check...in case another thread was creating the object just before
                        if (_CacheList == null)
                        {
                            // Create the Instance
                            _CacheList = new List<string>();
                        }
                    }
                }
                return _CacheList;
            }
        }
        
        public static void InitializeCacheList()
        {
            // Instance must not exist before locking
            lock (SyncObject)
            {
                // Create the Instance
                _CacheList = new List<string>();
            }
        }

        public static void SaveCacheEntry(string cacheEntry)
        {
            _CacheList.Add(cacheEntry);
        }

        //Can add by TYPE, create convention
        public static void ClearCache(IMemoryCache cache)
        {
            foreach (var cacheItem in _CacheList)
            {
                cache.Remove(cacheItem);
            }
        }




    }
}