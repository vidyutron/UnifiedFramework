using System;

namespace OpenQA.Selenium.Support.UI
{
	public abstract class LoadableComponent<T> : ILoadableComponent where T : LoadableComponent<T>
	{
		public virtual string UnableToLoadMessage
		{
			get;
			set;
		}

		protected bool IsLoaded
		{
			get
			{
				bool result = false;
				try
				{
					result = this.EvaluateLoadedStatus();
				}
				catch (WebDriverException)
				{
					return false;
				}
				return result;
			}
		}

		public virtual T Load()
		{
			if (this.IsLoaded)
			{
				return (T)((object)this);
			}
			this.TryLoad();
			if (!this.IsLoaded)
			{
				throw new LoadableComponentException(this.UnableToLoadMessage);
			}
			return (T)((object)this);
		}

		ILoadableComponent ILoadableComponent.Load()
		{
			return this.Load();
		}

		protected virtual void HandleLoadError(WebDriverException ex)
		{
		}

		protected abstract void ExecuteLoad();

		protected abstract bool EvaluateLoadedStatus();

		protected T TryLoad()
		{
			try
			{
				this.ExecuteLoad();
			}
			catch (WebDriverException ex)
			{
				this.HandleLoadError(ex);
			}
			return (T)((object)this);
		}
	}
}
