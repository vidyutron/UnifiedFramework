using UnifiedFramework.UnifiedReports.Model;
using UnifiedFramework.UnifiedReports.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnifiedFramework.UnifiedReports
{
	public class UnifiedTest
	{
		private Test test;

		internal UnifiedTest(string TestName, string Description)
		{
			this.test = new Test();
			this.test.Name = TestName;
			this.test.Description = Description;
			this.test.StartedTime = DateTime.Now;
		}

		public void Log(LogStatus LogStatus, string StepName, string Details)
		{
			Log log = new Log();
			log.Timestamp = DateTime.Now;
			log.LogStatus = LogStatus;
			log.StepName = StepName;
			log.Details = Details;
			this.test.Logs.Add(log);
		}

		public void Log(LogStatus LogStatus, string Details)
		{
			this.Log(LogStatus, string.Empty, Details);
		}

		public string AddScreenCapture(string ImagePath)
		{
			string text;
			if (this.IsPathRelative(ImagePath))
			{
				text = ImageHtml.GetSource(ImagePath).Replace("file:///", "");
			}
			else
			{
				text = ImageHtml.GetSource(ImagePath);
			}
			ScreenCapture screenCapture = new ScreenCapture();
			screenCapture.Source = text;
			screenCapture.TestName = this.test.Name;
			this.test.ScreenCapture.Add(screenCapture);
			return text;
		}

		public string AddScreencast(string screencastPath)
		{
			if (this.IsPathRelative(screencastPath))
			{
				screencastPath = ScreencastHtml.GetSource(screencastPath).Replace("file:///", "");
			}
			else
			{
				screencastPath = ScreencastHtml.GetSource(screencastPath);
			}
			Screencast screencast = new Screencast();
			screencast.Source = screencastPath;
			screencast.TestName = this.test.Name;
			this.test.Screencast.Add(screencast);
			return screencastPath;
		}

		public UnifiedTest AssignCategory(params string[] CategoryName)
		{
			(from c in CategoryName.ToList<string>()
			select c.ToString()).Distinct<string>().ToList<string>().ForEach(delegate(string c)
			{
				this.test.CategoryList.Add(new Category(c));
			});
			return this;
		}

		public UnifiedTest AppendChild(UnifiedTest Node)
		{
			Node.GetTest().EndedTime = DateTime.Now;
			Node.GetTest().IsChildNode = true;
			Node.GetTest().TrackLastRunStatus();
			this.test.HasChildNodes = true;
			List<string> list = new List<string>();
			foreach (TestAttribute current in this.test.CategoryList)
			{
				if (!list.Contains(current.GetName()))
				{
					list.Add(current.GetName());
				}
			}
			foreach (TestAttribute current in Node.GetTest().CategoryList)
			{
				if (!list.Contains(current.GetName()))
				{
					this.test.CategoryList.Add(current);
				}
			}
			this.test.NodeList.Add(Node.GetTest());
			return this;
		}

		internal Test GetTest()
		{
			return this.test;
		}

		private bool IsPathRelative(string FilePath)
		{
			return FilePath.StartsWith("http") || !Path.IsPathRooted(FilePath);
		}
	}
}
