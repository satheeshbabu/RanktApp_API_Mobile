using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Trakker.Api.Helpers
{
//    public class Cache
//    {
//        private static IMemoryCache _Instance;
//        private static readonly object SyncObject = new Object();
//
//        public static IMemoryCache Instance
//        {
//            get
//            {
//                if (_Instance == null)
//                {
//                    // Instance must not exist before locking
//                    lock (SyncObject)
//                    {
//                        // Double-check...in case another thread was creating the object just before
//                        if (_Instance == null)
//                        {
//                            // Create the Instance
//                            _Instance = new MemoryCache();
//                        }
//                    }
//                }
//                return _Instance;
//            }
//        }
//
//        public static void SetInstance(IMemoryCache cache)
//        {
//            // Instance must not exist before locking
//            lock (SyncObject)
//            {
//                // Create the Instance
//                _Instance = cache;
//            }
//        }
//
//    }
//
//    public static class CacheExtensions
//    {
//        public static IMemoryCache UseStart(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<IMemoryCache>();
//        }
//    }
    
}