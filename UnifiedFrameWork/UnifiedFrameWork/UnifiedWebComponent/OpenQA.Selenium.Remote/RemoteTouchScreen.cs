using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	public class RemoteTouchScreen : ITouchScreen
	{
		private RemoteWebDriver driver;

		public RemoteTouchScreen(RemoteWebDriver driver)
		{
			this.driver = driver;
		}

		public void SingleTap(ICoordinates where)
		{
			if (where == null)
			{
				throw new ArgumentNullException("where", "where coordinates cannot be null");
			}
			string value = where.AuxiliaryLocator.ToString();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("element", value);
			this.driver.InternalExecute(DriverCommand.TouchSingleTap, dictionary);
		}

		public void Down(int locationX, int locationY)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("x", locationX);
			dictionary.Add("y", locationY);
			this.driver.InternalExecute(DriverCommand.TouchPress, dictionary);
		}

		public void Up(int locationX, int locationY)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("x", locationX);
			dictionary.Add("y", locationY);
			this.driver.InternalExecute(DriverCommand.TouchRelease, dictionary);
		}

		public void Move(int locationX, int locationY)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("x", locationX);
			dictionary.Add("y", locationY);
			this.driver.InternalExecute(DriverCommand.TouchMove, dictionary);
		}

		public void Scroll(ICoordinates where, int offsetX, int offsetY)
		{
			if (where == null)
			{
				throw new ArgumentNullException("where", "where coordinates cannot be null");
			}
			string value = where.AuxiliaryLocator.ToString();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("element", value);
			dictionary.Add("xoffset", offsetX);
			dictionary.Add("yoffset", offsetY);
			this.driver.InternalExecute(DriverCommand.TouchScroll, dictionary);
		}

		public void Scroll(int offsetX, int offsetY)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("xoffset", offsetX);
			dictionary.Add("yoffset", offsetY);
			this.driver.InternalExecute(DriverCommand.TouchScroll, dictionary);
		}

		public void DoubleTap(ICoordinates where)
		{
			if (where == null)
			{
				throw new ArgumentNullException("where", "where coordinates cannot be null");
			}
			string value = where.AuxiliaryLocator.ToString();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("element", value);
			this.driver.InternalExecute(DriverCommand.TouchDoubleTap, dictionary);
		}

		public void LongPress(ICoordinates where)
		{
			if (where == null)
			{
				throw new ArgumentNullException("where", "where coordinates cannot be null");
			}
			string value = where.AuxiliaryLocator.ToString();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("element", value);
			this.driver.InternalExecute(DriverCommand.TouchLongPress, dictionary);
		}

		public void Flick(int speedX, int speedY)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("xspeed", speedX);
			dictionary.Add("yspeed", speedY);
			this.driver.InternalExecute(DriverCommand.TouchFlick, dictionary);
		}

		public void Flick(ICoordinates where, int offsetX, int offsetY, int speed)
		{
			if (where == null)
			{
				throw new ArgumentNullException("where", "where coordinates cannot be null");
			}
			string value = where.AuxiliaryLocator.ToString();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("element", value);
			dictionary.Add("xoffset", offsetX);
			dictionary.Add("yoffset", offsetY);
			dictionary.Add("speed", speed);
			this.driver.InternalExecute(DriverCommand.TouchFlick, dictionary);
		}
	}
}
