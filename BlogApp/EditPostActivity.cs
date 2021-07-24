using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using BlogApp.Fragments;
using BlogApp.Models;
using Org.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogApp
{
    [Activity(Label = "EditPostActivity")]
    public class EditPostActivity : AppCompatActivity
    {
        int position = 0, id = 0;
        EditText txtDesc;
        Button btnSave;
        [Obsolete]
        ProgressDialog dialog;


        IRestResponse response;
        ISharedPreferences pref;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.activity_edit_post);
            init();
        }

        [Obsolete]
        private void init()
        {
            var cancelEdit = FindViewById<ImageButton>(Resource.Id.canceledit);
            txtDesc = FindViewById<EditText>(Resource.Id.txtDescEditPost);
            btnSave = FindViewById<Button>(Resource.Id.btnEditPost);
            dialog = new ProgressDialog(this);
            dialog.SetCancelable(false);

            position = Intent.GetIntExtra("position", 0);
            id = Intent.GetIntExtra("postId", 0);
            txtDesc.Text = Intent.GetStringExtra("text");

            btnSave.Click += (s,e)=>
            {
                savePost();
            };

            cancelEdit.Click += (s, e) =>
            {
                base.OnBackPressed();
            };

        }

        [Obsolete]
        private void savePost()
        {
            dialog.SetMessage("Saving ...");
            dialog.Show();

             pref = PreferenceManager.GetDefaultSharedPreferences(this);

            string Token = pref.GetString("token", "");

            string query = Constant.UPDATE_POST;

            string Bearer = "Bearer " + Token;

            var client = new RestClient(query);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", Bearer);

            request.
                AddParameter("application/x-www-form-urlencoded",
                $"id={id}"+
                $"&desc={txtDesc.Text}" 
               // +$"&photo={System.Web.HttpUtility.UrlEncode(Bitmaptostring(bitmap))}"
               
                ,ParameterType.RequestBody);

            

            response = client.Execute(request);


            System.Console.WriteLine("[*] Response: " + response.Content);

            try
            {
                JSONObject jobject = new JSONObject(response.Content);
                if (jobject.GetBoolean("success"))
                {

                    Post post = HomeFragment.list[position];

                    post.Desc = txtDesc.Text.ToString();

                    HomeFragment.list.Insert(position, post);

                    HomeFragment.recyclerview.GetAdapter().NotifyItemInserted(position);
                    HomeFragment.recyclerview.GetAdapter().NotifyDataSetChanged();
                    Toast.MakeText(this, "Posted Edited", ToastLength.Short).Show();
                    Finish();
                }
            }
            catch (JSONException e)
            {

                e.PrintStackTrace();
            }

        }


    }
}