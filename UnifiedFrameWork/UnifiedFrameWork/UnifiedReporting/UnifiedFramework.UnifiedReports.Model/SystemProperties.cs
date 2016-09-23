using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports.Model
{
	internal class SystemProperties
	{
		public Dictionary<string, string> Info;

		public SystemProperties()
		{
			this.Info = new Dictionary<string, string>();
		}
	}
}
