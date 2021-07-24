using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using BlogApp.Adapters;
using BlogApp.Fragments;
using BlogApp.Models;
using Org.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlogApp
{
    [Activity(Label = "AddPostActivity")]
    public class AddPostActivity : Activity
    {
        ImageButton btnBack;
        TextView btnChangePhoto;

        Button btnPost;
        ImageView imgPost;
        EditText txtDesc;
        Bitmap bitmap = null;
        IRestResponse response;
        ISharedPreferences pref;
        ISharedPreferencesEditor editor;
        [Obsolete]
        ProgressDialog dialog;
        static int Gallery_CHANGE_POST = 3;






        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_add_post);

            init();

        }

        private void init()
        {
            btnPost = FindViewById<Button>(Resource.Id.btnAddPost);
            imgPost = FindViewById<ImageView>(Resource.Id.imgAddPost);
            txtDesc = FindViewById<EditText>(Resource.Id.txtDescAddPost);

            btnBack = FindViewById<ImageButton>(Resource.Id.btnBackPostAdd);
            btnChangePhoto = FindViewById<TextView>(Resource.Id.ChangePhotoAddPost);

            dialog = new ProgressDialog(this);
            dialog.SetCancelable(false);

            imgPost.SetImageURI(Intent.Data);
            try
            {
                bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver,Intent.Data);
            }
            catch (System.IO.IOException e)
            {

                System.Console.WriteLine("[-] Exception: " + e);
            }

            btnPost.Click +=delegate
            {
                if(!String.IsNullOrEmpty(txtDesc.Text.ToString()))
                {
                    post();
                    
                }
                else
                {
                    Toast.MakeText(this, "Post Description is required", ToastLength.Short).Show();
                }

            };

            btnBack.Click += (s, e) =>
            {
                base.OnBackPressed();
            };

            btnChangePhoto.Click += (s, e) =>
            {
                Intent i = new Intent(Intent.ActionPick);
                i.SetType("image/*");
                StartActivityForResult(i, Gallery_CHANGE_POST);
            };
        }




        [Obsolete]
        private void post()
        {
            dialog.SetMessage("Posting");
            dialog.Show();


            pref = PreferenceManager.GetDefaultSharedPreferences(this);

            string Token = pref.GetString("token", "");

            string query = Constant.ADD_POST;

            string Bearer = "Bearer " + Token;

            var client = new RestClient(query);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", Bearer);


            //request.AddParameter("application/octet-stream", Bitmaptostring(bitmap) , ParameterType.RequestBody);

            //request.AddHeader("content-type", "application/x-www-form-urlencoded");

            request.
                AddParameter("application/x-www-form-urlencoded",
                $"desc={txtDesc.Text}" +
                $"&photo={System.Web.HttpUtility.UrlEncode(Bitmaptostring(bitmap))}",
                ParameterType.RequestBody);

            // request.AddParameter("application/x-www-form-urlencoded; charset=UTF-8", "photo="+Bitmaptostring(bitmap), ParameterType.RequestBody);
            //request.AddBody("photo="+ParameterType.RequestBody);

            response = client.Execute(request);


            System.Console.WriteLine("[*] Response: " + response.Content);



            try
            {
                JSONObject jobject = new JSONObject(response.Content);
                if (jobject.GetBoolean("success"))
                {

                        JSONObject postObject = jobject.GetJSONObject("post");
                        JSONObject userObject = postObject.GetJSONObject("user");

                        User user = new User();

                        user.Id = userObject.GetInt("id");
                        user.UserName = userObject.GetString("name") + " " + userObject.GetString("lastname");
                        user.Photo = (userObject.GetString("photo"));

                        Post post = new Post();
                        post.Id = postObject.GetInt("id");
                        post.User = user;
                        post.Like = 0;
                        post.Comments = 0;
                        post.Date = postObject.GetString("created_at");
                        post.Desc = postObject.GetString("desc");
                        post.Photo = postObject.GetString("photo");
                        post.SelfLike = false;


                    HomeFragment.list.Insert(0, post);

                    HomeFragment.recyclerview.GetAdapter().NotifyItemInserted(0);
                    HomeFragment.recyclerview.GetAdapter().NotifyDataSetChanged();
                    Toast.MakeText(this, "Posted", ToastLength.Short).Show();
                    Finish();
                 
                }
            }
            catch (JSONException e)
            {

                // throw e;
                e.PrintStackTrace();
                
            }



            dialog.Dismiss();
        }



        private string Bitmaptostring(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                MemoryStream bytearrayoutputstream = new MemoryStream();

                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, bytearrayoutputstream);
                byte[] array = bytearrayoutputstream.ToArray();

                return Base64.EncodeToString(array, Base64Flags.Default);


            }

            return "";

        }

        [Obsolete]
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == Gallery_CHANGE_POST && resultCode == Result.Ok)
            {

                Android.Net.Uri imgUri = data.Data;

                imgPost.SetImageURI(imgUri);

                try
                {
                    bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, imgUri);
                }
                catch (System.IO.IOException e)
                {

                    System.Console.WriteLine("[-] Exception: " + e);
                }
            }
        }
    }
}