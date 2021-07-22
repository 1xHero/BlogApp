using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Java.Lang;
using System;

namespace BlogApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Handler h = new Handler();
            Action myAction = () =>
            {
                ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);

                bool isLoggedIn = pref.GetBoolean("isLoggedIn", false);

                if (isLoggedIn)
                {
                    StartActivity(typeof(HomeActivity));
                    Finish();
                }
                else
                {
                    isFirstTime();
                }

                
            };

            h.PostDelayed(myAction, 1500);

        }

        private void isFirstTime()
        {


            ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);

            bool isFirstTime = pref.GetBoolean("isFirstTime", true);

            if (isFirstTime)
            {
                ISharedPreferencesEditor editor = pref.Edit();
                editor.PutBoolean("isFirstTime", false);
                editor.Apply();

                StartActivity(typeof(OnboardActivity));
               Finish();
            }
            else
            {
                StartActivity(typeof(AuthActivity));
                Finish();
            }
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}