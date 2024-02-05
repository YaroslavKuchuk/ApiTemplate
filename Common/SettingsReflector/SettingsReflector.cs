using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Common.SettingsReflector
{
	public class SettingsReflector : ISettingsReflector
	{
		private static readonly ConcurrentDictionary<Type, SettingsTypeContainer> SettinsCache = new ConcurrentDictionary<Type, SettingsTypeContainer>();
	    private static readonly object _sync = new object();

        public static string[] GetKeys<T>(bool useLowerCase = true)
		{
			var container = GetOrCreateContainer<T>();
			return container.GetKeys(useLowerCase);
		}

		public static T CreateNewObject<T>(Dictionary<string, string> properties) where T : new()
		{
			var settingObject = new T();
			var container = GetOrCreateContainer<T>();

			foreach (var propertyPair in container.Properties)
			{
				string value;

				if (!properties.TryGetValue(propertyPair.Value.Name, out value))
					value = propertyPair.Value.DefaultValue;

				propertyPair.Key.SetValue(settingObject, ConvertValue(propertyPair.Key, value));
			}

			return settingObject;
		}

		private static object ConvertValue(PropertyInfo propertyInfo, string value)
		{
			var type = propertyInfo.PropertyType;

			if (string.IsNullOrWhiteSpace(value))
				return type.IsValueType ? Activator.CreateInstance(type) : null;

			return Convert.ChangeType(value, type);
		}

		private static SettingsTypeContainer GetOrCreateContainer<T>()
		{
			SettingsTypeContainer container;
		    lock (_sync)
		    {
		        if (SettinsCache.TryGetValue(typeof(T), out container))
		            return container;
		        container = CreateNewContainer<T>();
		    }
		    return container;
		}

		private static SettingsTypeContainer CreateNewContainer<T>()
		{
			var type = typeof(T);
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var propertiesMap = new ConcurrentDictionary<PropertyInfo, SettingsPropertyAttribute>();

			foreach (var propertyInfo in properties)
			{
				var attribute = propertyInfo.GetCustomAttribute<SettingsPropertyAttribute>();

				if (attribute != null)
					propertiesMap.GetOrAdd(propertyInfo, attribute);

			}
			var container = new SettingsTypeContainer(type, propertiesMap);
		    lock (_sync)
		    {
		        if (!SettinsCache.ContainsKey(type))
		        {
		            SettinsCache.GetOrAdd(type, container);
		        }
		    }
            return container;
		}
	}
}

