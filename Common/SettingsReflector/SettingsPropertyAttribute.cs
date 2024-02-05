using System;

namespace Common.SettingsReflector
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]

	public class SettingsPropertyAttribute : Attribute
	{
		public string Name { get; set; }

		public string DefaultValue { get; set; }

		public SettingsPropertyAttribute(string name)
		{
			Name = name;
		}
	}
}
