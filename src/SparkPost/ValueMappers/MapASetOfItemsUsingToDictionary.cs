﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SparkPost.ValueMappers
{
    public class MapASetOfItemsUsingToDictionary : IValueMapper
    {
        private readonly Dictionary<Type, MethodInfo> converters;
        private readonly IDataMapper dataMapper;

        public MapASetOfItemsUsingToDictionary(IDataMapper dataMapper)
        {
            this.dataMapper = dataMapper;
            converters = MapASingleItemUsingToDictionary.GetTheConverters(dataMapper);
        }

        public bool CanMap(Type propertyType, object value)
        {
            return value != null && propertyType.Name.EndsWith("List`1") &&
                   propertyType.GetGenericArguments().Count() == 1 &&
                   converters.ContainsKey(propertyType.GetGenericArguments().First());
        }

        public object Map(Type propertyType, object value)
        {
            var converter = converters[propertyType.GetGenericArguments().First()];

            var list = (value as IEnumerable<object>).ToList();

            if (list.Any())
                value = list.Select(x => converter.Invoke(dataMapper, BindingFlags.Default, null,
                    new[] {x}, CultureInfo.CurrentCulture)).ToList();
            else
                value = null;

            return value;
        }
    }
}