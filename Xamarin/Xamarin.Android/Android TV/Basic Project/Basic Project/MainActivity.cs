using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;

namespace Basic_Project {
    [Activity(Theme = "@style/AppTheme", Name = "com.dimanrus.activity")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)=>
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}