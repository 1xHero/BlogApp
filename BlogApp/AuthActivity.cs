using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlogApp.Fragments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogApp
{
    [Activity(Label = "AuthActivity")]
    public class AuthActivity : Activity
    {
        [Obsolete]
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_auth);
                     
            FragmentManager.BeginTransaction().Replace(Resource.Id.frameAuthContainer, new SigninFragment()).Commit();


        }
    }
}