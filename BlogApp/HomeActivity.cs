using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlogApp.Fragments;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogApp
{
    [Activity]
    public class HomeActivity : Activity
    {

        FloatingActionButton fab;
        static int Gallery_ADD_POST = 2;

        [Obsolete]
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.activity_home);
            this.Title = "";


            this.FragmentManager.BeginTransaction().Replace(Resource.Id.frameHomeContainer, new HomeFragment()).Commit();

            init();
        }

        private void init()
        {
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            fab.Click += delegate
            {
                Intent i = new Intent(Intent.ActionPick);
                i.SetType("image/*");
                StartActivityForResult(i, Gallery_ADD_POST);
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if(requestCode== Gallery_ADD_POST && resultCode == Result.Ok)
            {
               
              Android.Net.Uri imgUri = data.Data;

                Intent i = new Intent(this,typeof(AddPostActivity));

                i.SetData(imgUri);
                StartActivity(i);
            }
        }
    }
}