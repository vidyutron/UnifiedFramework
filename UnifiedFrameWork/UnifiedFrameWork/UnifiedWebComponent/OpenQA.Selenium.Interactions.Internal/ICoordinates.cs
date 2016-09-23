using System;
using System.Drawing;

namespace OpenQA.Selenium.Interactions.Internal
{
	public interface ICoordinates
	{
		Point LocationOnScreen
		{
			get;
		}

		Point LocationInViewport
		{
			get;
		}

		Point LocationInDom
		{
			get;
		}

		object AuxiliaryLocator
		{
			get;
		}
	}
}
