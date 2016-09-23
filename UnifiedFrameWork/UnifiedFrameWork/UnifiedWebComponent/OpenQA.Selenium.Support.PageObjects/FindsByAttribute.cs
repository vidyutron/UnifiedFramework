using System;
using System.ComponentModel;

namespace OpenQA.Selenium.Support.PageObjects
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public sealed class FindsByAttribute : Attribute, IComparable
	{
		private By finder;

		[DefaultValue(How.Id)]
		public How How
		{
			get;
			set;
		}

		public string Using
		{
			get;
			set;
		}

		[DefaultValue(0)]
		public int Priority
		{
			get;
			set;
		}

		public Type CustomFinderType
		{
			get;
			set;
		}

		internal By Finder
		{
			get
			{
				if (this.finder == null)
				{
					this.finder = ByFactory.From(this);
				}
				return this.finder;
			}
			set
			{
				this.finder = value;
			}
		}

		public static bool operator ==(FindsByAttribute one, FindsByAttribute two)
		{
			return object.ReferenceEquals(one, two) || (one != null && two != null && one.Equals(two));
		}

		public static bool operator !=(FindsByAttribute one, FindsByAttribute two)
		{
			return !(one == two);
		}

		public static bool operator >(FindsByAttribute one, FindsByAttribute two)
		{
			if (one == null)
			{
				throw new ArgumentNullException("one", "Object to compare cannot be null");
			}
			return one.CompareTo(two) > 0;
		}

		public static bool operator <(FindsByAttribute one, FindsByAttribute two)
		{
			if (one == null)
			{
				throw new ArgumentNullException("one", "Object to compare cannot be null");
			}
			return one.CompareTo(two) < 0;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "Object to compare cannot be null");
			}
			FindsByAttribute findsByAttribute = obj as FindsByAttribute;
			if (findsByAttribute == null)
			{
				throw new ArgumentException("Object to compare must be a FindsByAttribute", "obj");
			}
			if (this.Priority != findsByAttribute.Priority)
			{
				return this.Priority - findsByAttribute.Priority;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			FindsByAttribute findsByAttribute = obj as FindsByAttribute;
			return !(findsByAttribute == null) && findsByAttribute.Priority == this.Priority && !(findsByAttribute.Finder != this.Finder);
		}

		public override int GetHashCode()
		{
			return this.Finder.GetHashCode();
		}
	}
}
