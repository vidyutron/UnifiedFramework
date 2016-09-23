using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports.Model
{
	internal class MediaList
	{
		public List<ScreenCapture> ScreenCapture;

		public List<Screencast> Screencast;

		public MediaList()
		{
			this.ScreenCapture = new List<ScreenCapture>();
			this.Screencast = new List<Screencast>();
		}
	}
}
