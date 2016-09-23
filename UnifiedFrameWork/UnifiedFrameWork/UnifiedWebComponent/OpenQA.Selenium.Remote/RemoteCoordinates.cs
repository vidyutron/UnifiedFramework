using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Drawing;

namespace OpenQA.Selenium.Remote
{
	internal class RemoteCoordinates : ICoordinates
	{
		private RemoteWebElement element;

		public Point LocationOnScreen
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Point LocationInViewport
		{
			get
			{
				return this.element.LocationOnScreenOnceScrolledIntoView;
			}
		}

		public Point LocationInDom
		{
			get
			{
				return this.element.Location;
			}
		}

		public object AuxiliaryLocator
		{
			get
			{
				return this.element.InternalElementId;
			}
		}

		public RemoteCoordinates(RemoteWebElement element)
		{
			this.element = element;
		}
	}
}
