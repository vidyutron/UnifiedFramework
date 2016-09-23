using System;
using System.Runtime.InteropServices;

namespace OpenQA.Selenium
{
	internal static class NativeMethods
	{
		[Flags]
		internal enum HandleInformation
		{
			None = 0,
			Inherit = 1,
			ProtectFromClose = 2
		}

		[DllImport("kernel32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetHandleInformation(IntPtr hObject, NativeMethods.HandleInformation dwMask, NativeMethods.HandleInformation dwFlags);
	}
}
