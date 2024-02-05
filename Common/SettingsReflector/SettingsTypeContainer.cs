using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Common.SettingsReflector
{
	public class SettingsTypeContainer
	{
		public ConcurrentDictionary<PropertyInfo, SettingsPropertyAttribute> Properties { get; private set; }

		public Type Type { get; private set; }

		public SettingsTypeContainer(Type type, ConcurrentDictionary<PropertyInfo, SettingsPropertyAttribute> properties)
		{
			Properties = properties;
			Type = type;
		}

		public string[] GetKeys(bool useLowerCase = true)
		{
			var keys = Properties.Select(e => e.Value.Name);
			return (useLowerCase ? keys.Select(e => e.ToLower()) : keys).ToArray();
		}
	}
}
