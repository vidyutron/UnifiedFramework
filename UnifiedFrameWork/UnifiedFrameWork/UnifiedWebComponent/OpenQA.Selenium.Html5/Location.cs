using System;
using System.Globalization;

namespace OpenQA.Selenium.Html5
{
	public class Location
	{
		private readonly double latitude;

		private readonly double longitude;

		private readonly double altitude;

		public double Latitude
		{
			get
			{
				return this.latitude;
			}
		}

		public double Longitude
		{
			get
			{
				return this.longitude;
			}
		}

		public double Altitude
		{
			get
			{
				return this.altitude;
			}
		}

		public Location(double latitude, double longitude, double altitude)
		{
			this.latitude = latitude;
			this.longitude = longitude;
			this.altitude = altitude;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Latitude: {0}, Longitude: {1}, Altitude: {2}", new object[]
			{
				this.latitude,
				this.longitude,
				this.altitude
			});
		}
	}
}
