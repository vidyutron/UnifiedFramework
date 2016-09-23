using System;

namespace OpenQA.Selenium.Interactions.Internal
{
	internal class MouseAction : WebDriverAction
	{
		private IMouse mouse;

		protected ICoordinates ActionLocation
		{
			get
			{
				if (base.ActionTarget == null)
				{
					return null;
				}
				return base.ActionTarget.Coordinates;
			}
		}

		protected IMouse Mouse
		{
			get
			{
				return this.mouse;
			}
		}

		public MouseAction(IMouse mouse, ILocatable target) : base(target)
		{
			this.mouse = mouse;
		}

		protected void MoveToLocation()
		{
			if (this.ActionLocation != null)
			{
				this.mouse.MouseMove(this.ActionLocation);
			}
		}
	}
}
