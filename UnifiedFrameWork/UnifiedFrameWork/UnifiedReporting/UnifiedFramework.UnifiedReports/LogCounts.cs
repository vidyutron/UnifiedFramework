using UnifiedFramework.UnifiedReports.Model;
using System;
using System.Linq;

namespace UnifiedFramework.UnifiedReports
{
	internal class LogCounts
	{
		public int Pass = 0;

		public int Fail = 0;

		public int Fatal = 0;

		public int Error = 0;

		public int Warning = 0;

		public int Info = 0;

		public int Skip = 0;

		public int Unknown = 0;

		public LogCounts GetLogCounts(Test test)
		{
			this.Pass += (from l in test.Logs
			where l.LogStatus == LogStatus.Pass
			select l).Count<Log>();
			this.Fail += (from l in test.Logs
			where l.LogStatus == LogStatus.Fail
			select l).Count<Log>();
			this.Fatal += (from l in test.Logs
			where l.LogStatus == LogStatus.Fatal
			select l).Count<Log>();
			this.Error += (from l in test.Logs
			where l.LogStatus == LogStatus.Error
			select l).Count<Log>();
			this.Warning += (from l in test.Logs
			where l.LogStatus == LogStatus.Warning
			select l).Count<Log>();
			this.Info += (from l in test.Logs
			where l.LogStatus == LogStatus.Info
			select l).Count<Log>();
			this.Skip += (from l in test.Logs
			where l.LogStatus == LogStatus.Skip
			select l).Count<Log>();
			this.Unknown += (from l in test.Logs
			where l.LogStatus == LogStatus.Unknown
			select l).Count<Log>();
			test.NodeList.ForEach(delegate(Test n)
			{
				this.GetLogCounts(n);
			});
			return this;
		}
	}
}
