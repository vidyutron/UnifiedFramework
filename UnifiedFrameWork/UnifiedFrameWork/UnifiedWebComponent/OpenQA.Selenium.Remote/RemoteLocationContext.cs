using OpenQA.Selenium.Html5;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
	public class RemoteLocationContext : ILocationContext
	{
		private RemoteWebDriver driver;

		public Location PhysicalLocation
		{
			get
			{
				Response response = this.driver.InternalExecute(DriverCommand.GetLocation, null);
				Dictionary<string, object> dictionary = response.Value as Dictionary<string, object>;
				if (dictionary != null)
				{
					return new Location(double.Parse(dictionary["latitude"].ToString(), CultureInfo.InvariantCulture), double.Parse(dictionary["longitude"].ToString(), CultureInfo.InvariantCulture), double.Parse(dictionary["altitude"].ToString(), CultureInfo.InvariantCulture));
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "value cannot be null");
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("latitude", value.Latitude);
				dictionary.Add("longitude", value.Longitude);
				dictionary.Add("altitude", value.Altitude);
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("location", dictionary);
				this.driver.InternalExecute(DriverCommand.SetLocation, dictionary2);
			}
		}

		public RemoteLocationContext(RemoteWebDriver driver)
		{
			this.driver = driver;
		}
	}
}
