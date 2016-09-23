using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OpenQA.Selenium.Firefox
{
	internal class Preferences
	{
		private Dictionary<string, string> preferences = new Dictionary<string, string>();

		private Dictionary<string, string> immutablePreferences = new Dictionary<string, string>();

		public Preferences(Dictionary<string, object> defaultImmutablePreferences, Dictionary<string, object> defaultPreferences)
		{
			if (defaultImmutablePreferences != null)
			{
				foreach (KeyValuePair<string, object> current in defaultImmutablePreferences)
				{
					this.SetPreferenceValue(current.Key, current.Value);
					this.immutablePreferences.Add(current.Key, current.Value.ToString());
				}
			}
			if (defaultPreferences != null)
			{
				foreach (KeyValuePair<string, object> current2 in defaultPreferences)
				{
					this.SetPreferenceValue(current2.Key, current2.Value);
				}
			}
		}

		internal void SetPreference(string key, string value)
		{
			this.SetPreferenceValue(key, value);
		}

		internal void SetPreference(string key, int value)
		{
			this.SetPreferenceValue(key, value);
		}

		internal void SetPreference(string key, bool value)
		{
			this.SetPreferenceValue(key, value);
		}

		internal string GetPreference(string preferenceName)
		{
			if (this.preferences.ContainsKey(preferenceName))
			{
				return this.preferences[preferenceName];
			}
			return string.Empty;
		}

		internal void AppendPreferences(Dictionary<string, string> preferencesToAdd)
		{
			foreach (KeyValuePair<string, string> current in preferencesToAdd)
			{
				if (this.IsSettablePreference(current.Key))
				{
					this.preferences[current.Key] = current.Value;
				}
			}
		}

		internal void WriteToFile(string filePath)
		{
			using (TextWriter textWriter = File.CreateText(filePath))
			{
				foreach (KeyValuePair<string, string> current in this.preferences)
				{
					string text = current.Value.Replace("\\", "\\\\");
					textWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "user_pref(\"{0}\", {1});", new object[]
					{
						current.Key,
						text
					}));
				}
			}
		}

		private static bool IsWrappedAsString(string value)
		{
			return value.StartsWith("\"", StringComparison.OrdinalIgnoreCase) && value.EndsWith("\"", StringComparison.OrdinalIgnoreCase);
		}

		private bool IsSettablePreference(string preferenceName)
		{
			return !this.immutablePreferences.ContainsKey(preferenceName);
		}

		private void SetPreferenceValue(string key, object value)
		{
			if (!this.IsSettablePreference(key))
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Preference {0} may not be overridden: frozen value={1}, requested value={2}", new object[]
				{
					key,
					this.immutablePreferences[key],
					value.ToString()
				});
				throw new ArgumentException(message);
			}
			string text = value as string;
			if (text != null)
			{
				if (Preferences.IsWrappedAsString(text))
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Preference values must be plain strings: {0}: {1}", new object[]
					{
						key,
						value
					}));
				}
				this.preferences[key] = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", new object[]
				{
					value
				});
				return;
			}
			else
			{
				if (value is bool)
				{
					this.preferences[key] = Convert.ToBoolean(value, CultureInfo.InvariantCulture).ToString().ToLowerInvariant();
					return;
				}
				if (value is int || value is long)
				{
					this.preferences[key] = Convert.ToInt32(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
					return;
				}
				throw new WebDriverException("Value must be string, int or boolean");
			}
		}
	}
}
