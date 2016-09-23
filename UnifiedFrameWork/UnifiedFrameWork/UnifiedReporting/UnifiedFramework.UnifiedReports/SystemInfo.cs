using UnifiedFramework.UnifiedReports.Model;
using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports
{
	internal class SystemInfo
	{
		private SystemProperties properties;

		public void Clear()
		{
			this.properties.Info.Clear();
		}

		public Dictionary<string, string> GetInfo()
		{
            return this.properties != null ? this.properties.Info : null;
		}

		public void SetInfo(Dictionary<string, string> SystemInfo)
		{
			foreach (KeyValuePair<string, string> current in SystemInfo)
			{
				this.properties.Info.Add(current.Key, current.Value);
			}
		}

		public void SetInfo(string Param, string Value)
		{
			this.properties.Info.Add(Param, Value);
		}

		private void SetInfo()
		{
			if (this.properties == null)
			{
				this.properties = new SystemProperties();
			}
			this.properties.Info.Add("User Name", Environment.UserName);
			this.properties.Info.Add("Machine Name", Environment.MachineName);
			this.properties.Info.Add("Domain", Environment.UserDomainName);
			this.properties.Info.Add("OS", Environment.OSVersion.Platform + " " + 
                Environment.OSVersion.Version);
		}

		public SystemInfo()
		{
			this.SetInfo();
		}
	}
}
