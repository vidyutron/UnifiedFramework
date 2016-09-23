using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteMouse : IMouse
	{
		private RemoteWebDriver driver;

		public RemoteMouse(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public void Click(ICoordinates where)
		{
			this.MoveIfNeeded(where);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("button", 0);
			this.driver.InternalExecute(DriverCommand.MouseClick, dictionary);
		}

		public void DoubleClick(ICoordinates where)
		{
			this.driver.InternalExecute(DriverCommand.MouseDoubleClick, null);
		}

		public void MouseDown(ICoordinates where)
		{
			this.driver.InternalExecute(DriverCommand.MouseDown, null);
		}

		public void MouseUp(ICoordinates where)
		{
			this.driver.InternalExecute(DriverCommand.MouseUp, null);
		}

		public void MouseMove(ICoordinates where)
		{
			if (where == null)
			{
				throw new ArgumentNullException("where", "where coordinates cannot be null");
			}
			string value = where.AuxiliaryLocator.ToString();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("element", value);
			this.driver.InternalExecute(DriverCommand.MouseMoveTo, dictionary);
		}

		public void MouseMove(ICoordinates where, int offsetX, int offsetY)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (where != null)
			{
				string value = where.AuxiliaryLocator.ToString();
				dictionary.Add("element", value);
			}
			else
			{
				dictionary.Add("element", null);
			}
			dictionary.Add("xoffset", offsetX);
			dictionary.Add("yoffset", offsetY);
			this.driver.InternalExecute(DriverCommand.MouseMoveTo, dictionary);
		}

		public void ContextClick(ICoordinates where)
		{
			this.MoveIfNeeded(where);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("button", 2);
			this.driver.InternalExecute(DriverCommand.MouseClick, dictionary);
		}

		private void MoveIfNeeded(ICoordinates where)
		{
			if (where != null)
			{
				this.MouseMove(where);
			}
		}
	}
}
