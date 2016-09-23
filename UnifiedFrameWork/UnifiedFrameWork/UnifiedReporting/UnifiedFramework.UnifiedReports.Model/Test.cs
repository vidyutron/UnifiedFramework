using System;
using System.Collections.Generic;

namespace UnifiedFramework.UnifiedReports.Model
{
	internal class Test
	{
		public bool HasEnded = false;

		public bool IsChildNode = false;

		public bool HasChildNodes = false;

		public DateTime StartedTime;

		public DateTime EndedTime;

		public List<TestAttribute> CategoryList;

		public List<TestAttribute> AuthorList;

		public List<Log> Logs;

		public List<Test> NodeList;

		public List<ScreenCapture> ScreenCapture;

		public List<Screencast> Screencast;

		public LogStatus Status = LogStatus.Unknown;

		public string Description;

		public string InternalWarning;

		public string Name;

		internal void PrepareFinalize()
		{
			this.updateTestStatusRecursively(this);
			if (this.Status == LogStatus.Info)
			{
				this.Status = LogStatus.Pass;
			}
		}

		internal void TrackLastRunStatus()
		{
			this.Logs.ForEach(delegate(Log l)
			{
				this.findStatus(l.LogStatus);
			});
			if (this.Status == LogStatus.Info)
			{
				this.Status = LogStatus.Pass;
			}
		}

		private void updateTestStatusRecursively(Test test)
		{
			test.Logs.ForEach(delegate(Log l)
			{
				this.findStatus(l.LogStatus);
			});
			if (test.HasChildNodes)
			{
				test.NodeList.ForEach(delegate(Test n)
				{
					this.updateTestStatusRecursively(n);
				});
			}
		}

		private void findStatus(LogStatus logStatus)
		{
			if (this.Status != LogStatus.Fatal)
			{
				if (logStatus == LogStatus.Fatal)
				{
					this.Status = logStatus;
				}
				else if (this.Status != LogStatus.Fail)
				{
					if (logStatus == LogStatus.Fail)
					{
						this.Status = logStatus;
					}
					else if (this.Status != LogStatus.Error)
					{
						if (logStatus == LogStatus.Error)
						{
							this.Status = logStatus;
						}
						else if (this.Status != LogStatus.Warning)
						{
							if (logStatus == LogStatus.Warning)
							{
								this.Status = logStatus;
							}
							else if (this.Status != LogStatus.Pass)
							{
								if (logStatus == LogStatus.Pass)
								{
									this.Status = LogStatus.Pass;
								}
								else if (this.Status != LogStatus.Skip)
								{
									if (logStatus == LogStatus.Skip)
									{
										this.Status = LogStatus.Skip;
									}
									else if (this.Status != LogStatus.Info)
									{
										if (logStatus == LogStatus.Info)
										{
											this.Status = LogStatus.Info;
										}
										else
										{
											this.Status = LogStatus.Unknown;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public Test()
		{
			this.InternalWarning = string.Empty;
			this.CategoryList = new List<TestAttribute>();
			this.AuthorList = new List<TestAttribute>();
			this.Logs = new List<Log>();
			this.NodeList = new List<Test>();
			this.ScreenCapture = new List<ScreenCapture>();
			this.Screencast = new List<Screencast>();
		}
	}
}
