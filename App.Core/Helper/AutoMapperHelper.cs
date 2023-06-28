using System;
using System.Collections.Generic;
using AutoMapper;

namespace App.Core.Helper
{
    public class AutoMapperHelper
    {
        /// <summary>
        /// Mapping with same class between object and map
        /// </summary>
        public static TDest Map<TSource, TDest>(TSource source)
            where TSource : class
            where TDest : class
        {
            if (source == null) return null;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDest>();
            });

            IMapper mapper = config.CreateMapper();
            var e = mapper.Map<TSource, TDest>(source);
            return e;
        }

        /// <summary>
        /// Mapping with same class between list object and map
        /// </summary>
        public static TDest MapList<TSource, TDest>(TSource source)
            where TSource : class
            where TDest : class
        {
            if (source == null) return null;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDest>();
            });

            IMapper mapper = config.CreateMapper();
            var e = mapper.Map<TSource, TDest>(source);
            return e;
        }

        /// <summary>
        /// Mapping with different class between object and map
        /// </summary>
        public static TDest Map<TSourceMap, TDestMap, TSource, TDest>(TSource source)
            where TSource : class
            where TDest : class
        {
            if (source == null) return null;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSourceMap, TDestMap>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<TSource, TDest>(source);
        }

        /// <summary>
        /// Mapping with different class between object and map
        /// </summary>
        public static void MapSameType<TSource>(object source, object dest, Type type)
        {
            if (source == null) return;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TSource>();
            });

            IMapper mapper = config.CreateMapper();
            mapper.Map(source, dest, type, type);
        }
    }
}
