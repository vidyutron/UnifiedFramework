using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Interactions.Internal
{
	internal class SingleKeyAction : KeyboardAction
	{
		private static readonly List<string> ModifierKeys = new List<string>
		{
			Keys.Shift,
			Keys.Control,
			Keys.Alt
		};

		private string key;

		protected string Key
		{
			get
			{
				return this.key;
			}
		}

		protected SingleKeyAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string key) : base(keyboard, mouse, actionTarget)
		{
			if (!SingleKeyAction.ModifierKeys.Contains(key))
			{
				throw new ArgumentException("key must be a modifier key (Keys.Shift, Keys.Control, or Keys.Alt)", "key");
			}
			this.key = key;
		}
	}
}
