using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace Basic_Project {
    [Activity(MainLauncher = true, Name = "com.dimanrus.activitysplash", NoHistory = true)]
    public class SplashActivity : AppCompatActivity {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState) =>
            base.OnCreate(savedInstanceState, persistentState);
        protected override void OnResume() {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}