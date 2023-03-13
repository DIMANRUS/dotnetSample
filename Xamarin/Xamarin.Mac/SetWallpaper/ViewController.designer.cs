// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SetWallpaper
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField TextFieldUrl { get; set; }

		[Action ("ChangeWallpaper:")]
		partial void ChangeWallpaper (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (TextFieldUrl != null) {
				TextFieldUrl.Dispose ();
				TextFieldUrl = null;
			}
		}
	}
}
