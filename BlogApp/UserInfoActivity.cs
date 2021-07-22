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
using Google.Android.Material.TextField;
using Java.IO;
using Org.Json;
using Refractored.Controls;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlogApp
{
    [Activity(Label = "UserInfoActivity")]
    public class UserInfoActivity : Activity
    {
        private TextInputLayout layoutName, layoutLastname;
        private TextInputEditText txtName, txtLastname;
        private TextView txtSelectPhoto;
        private Button btnContinue;
        private CircleImageView circleImageView;//Refractored.Controls._BaseCircleImageView CircleImageView
        private Bitmap bitmap = null;


        private static int GALLERY_ADD_PROFILE = 1;
        private static int RESULT_OK = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.activity_user_info);

            
            init();


             txtSelectPhoto.Click += delegate
                        {

                            

                            Intent i = new Intent(Intent.ActionPick);
                            i.SetType("image/*");
                            i.SetAction(Intent.ActionGetContent);

                            StartActivityForResult(i, GALLERY_ADD_PROFILE);
                        };

            btnContinue.Click += delegate
            {
                if (vlidate())
                {

                    saveUserInfo();
                    
                }

            };
        }

        private void saveUserInfo()
        {
            string name = txtName.Text.ToString().Trim();
            string lastname = txtLastname.Text.ToString().Trim();

            ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);
            



            string Token = pref.GetString("token","");

            string query = Constant.RSAVEUSERINFO;// + "?name=" + name + "&lastname=" + lastname;

            string Bearer = "Bearer " + Token;

            var client = new RestClient(query);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization",Bearer);
            
            
            //request.AddParameter("application/octet-stream", Bitmaptostring(bitmap) , ParameterType.RequestBody);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");

            request.
                AddParameter("application/x-www-form-urlencoded",
                $"name={name}&lastname={lastname}" +
                $"&photo={System.Web.HttpUtility.UrlEncode(Bitmaptostring(bitmap))}",
                ParameterType.RequestBody);

           // request.AddParameter("application/x-www-form-urlencoded; charset=UTF-8", "photo="+Bitmaptostring(bitmap), ParameterType.RequestBody);
            //request.AddBody("photo="+ParameterType.RequestBody);
            
            IRestResponse response = client.Execute(request);


            System.Console.WriteLine("[*] Response: " + response.Content);

            try
            {
                JSONObject jobect = new JSONObject(response.Content);

                if (jobect.GetBoolean("success"))
                {
                    ISharedPreferencesEditor editor = pref.Edit();
                    editor.PutString("photo", jobect.GetString("photo"));
                    editor.Apply();
                    StartActivity(typeof(HomeActivity));
                    Finish();
                }

            }
            catch (JSONException e)
            {

                System.Console.WriteLine("[-] :"+ e);
            }

        }

        private string Bitmaptostring(Bitmap bitmap)
        {
            if (bitmap!=null)
            {
                MemoryStream bytearrayoutputstream = new MemoryStream();

                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, bytearrayoutputstream);
                byte[] array = bytearrayoutputstream.ToArray();

                return Base64.EncodeToString(array,Base64Flags.Default);

                
            }

            return "";
            
        }

        private bool vlidate()
        {
            if (String.IsNullOrEmpty(txtName.Text.ToString()))
            {
                layoutName.ErrorEnabled = true;
                layoutName.Error = "Name is Required";
                return false;
            }

            if (String.IsNullOrEmpty(txtLastname.Text.ToString()))
            {
                layoutLastname.ErrorEnabled = true;
                layoutLastname.Error = "LastName is Required";
                return false;
            }
            return true;
        }

        private void init()
        {
            layoutLastname = FindViewById<TextInputLayout>(Resource.Id.txtLayoutLastnameameUserInfo);
            layoutName = FindViewById<TextInputLayout>(Resource.Id.txtLayoutNameUserInfo);
            txtName = FindViewById<TextInputEditText>(Resource.Id.txtNameUserInfo);
            txtLastname = FindViewById<TextInputEditText>(Resource.Id.txtLastnameUserInfo);
            txtSelectPhoto = FindViewById<TextView>(Resource.Id.txtSelectPhoto);
            btnContinue = FindViewById<Button>(Resource.Id.btnContinue);
            circleImageView = FindViewById<CircleImageView>(Resource.Id.imgUserInfo);

           
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == GALLERY_ADD_PROFILE && requestCode == RESULT_OK)
            {

                Android.Net.Uri imgUri = data.Data;
                circleImageView.SetImageURI(imgUri);

                try
                {
                    bitmap  = MediaStore.Images.Media.GetBitmap(ContentResolver, imgUri);
                }
                catch (System.IO.IOException e)
                {

                    System.Console.WriteLine("[-] Exception: "+e);
                }
            }
        }
    }
}