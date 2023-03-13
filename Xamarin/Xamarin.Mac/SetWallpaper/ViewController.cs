using System;
using AppKit;
using Foundation;

namespace SetWallpaper
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()=>
            base.ViewDidLoad();

        public override NSObject RepresentedObject
        {
            get=>base.RepresentedObject;
            set=>
                base.RepresentedObject = value;
        }
        partial void ChangeWallpaper(NSObject sender)
        {
            foreach (NSScreen screen in NSScreen.Screens)
            {
                NSWorkspace.SharedWorkspace.SetDesktopImageUrl(new NSUrl("file", "localhost", TextFieldUrl.StringValue), screen, options: new NSDictionary(), new NSError());
            }
        }
    }
}
