using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Com.Unity3d.Player;
using Com.Company.Product;

namespace UnityAsLibraryForXamarinAndroid
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            FindViewById<Button>(Resource.Id.button1).Click += (s, e) => StartActivity(typeof(UnityActivity));
        }
    }

    [Activity]
    public class UnityActivity : OverrideUnityActivity {
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Button btn1 = new Button(this) { 
                Text = "Зелёный"
            };
            btn1.SetY(0);
            btn1.SetX(10);
            btn1.Click += (s, e) => UnityPlayer.UnitySendMessage("Cube", "SetColorGreen", "");
            Button btn2 = new Button(this) {
                Text = "Красный"
            };
            btn2.SetY(200);
            btn2.SetX(10);
            btn2.Click += (s, e) => UnityPlayer.UnitySendMessage("Cube", "SetColorRed", "");
            UnityFrameLayout.AddView(btn1, 500, 100);
            UnityFrameLayout.AddView(btn2, 500, 100);
        }
        protected override void ShowMainActivity(string p0) {
            throw new System.NotImplementedException();
        }
    }
}