using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Google.Android.Material.TextField;
using Volley.Toolbox;
using Org.Json;
using RestSharp;
using Android.Preferences;


namespace BlogApp.Fragments
{
    [Obsolete]
    class SigninFragment : Fragment, Android.Text.ITextWatcher
    {
        private View view;
        private TextInputLayout layoutEmail, layoutPassword;
        private TextInputEditText txtEmail, txtPassword;
        private TextView txtSignUp;
        private Button btnSignIn;

        private ProgressDialog dialog;

        public SigninFragment()
        {

        }

        public void AfterTextChanged(Android.Text.IEditable s)
        {
            //   throw new NotImplementedException();
        }

        public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
        {
            //  throw new NotImplementedException();
        }

        [Obsolete]
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.layout_sign_in, container, false);
            init();
            return view;
        }

        public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
        {
        }

        private void init()
        {
            dialog = new ProgressDialog(Context);
            dialog.SetCancelable(false);

            layoutPassword = view.FindViewById<TextInputLayout>(Resource.Id.txtLayoutPasswordSignIn);
            layoutEmail = view.FindViewById<TextInputLayout>(Resource.Id.txtLayoutEmailSignIn);
            txtPassword = view.FindViewById<TextInputEditText>(Resource.Id.txtPasswordSignIn);
            txtSignUp = view.FindViewById<TextView>(Resource.Id.txtSignUp);
            txtEmail = view.FindViewById<TextInputEditText>(Resource.Id.txtEmailSignIn);
            btnSignIn = view.FindViewById<Button>(Resource.Id.btnSignIn);

            

            txtSignUp.Click += (s, e) =>
            {
                Activity.FragmentManager.BeginTransaction().Replace(Resource.Id.frameAuthContainer, new SignupFragment()).Commit();
            };

            btnSignIn.Click += (s,e)=>
            {
                if (validate())
                {
                    login(txtEmail.Text.ToString(), txtPassword.Text.ToString());
                }
            };

            txtEmail.TextChanged += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(txtEmail.Text.ToString()))
                {
                    layoutEmail.ErrorEnabled = false;
                    //  layoutEmail.Error = "Email is Required";
                }
            };



            txtPassword.TextChanged += (sender, e) =>
            {
                if (txtPassword.Text.ToString().Length > 7)
                {
                    layoutPassword.ErrorEnabled = false;
                }
            };


        }

        private bool validate()
        {
            if (String.IsNullOrEmpty(txtEmail.Text.ToString()))
            {
                layoutEmail.ErrorEnabled = true;
                layoutEmail.Error = "Email is Required";
                return false;
            }
            if (txtPassword.Text.ToString().Length < 8)
            {
                layoutPassword.ErrorEnabled = true;
                layoutPassword.Error = "Required at least 8 characters";
                return false;
            }

            return true;

        }

        private void login(string e,string p)
        {
            dialog.SetMessage("Loggin in");
            dialog.Indeterminate = true;
            dialog.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            dialog.Show();

          

            string query = Constant.LOGIN + "?email=" + e + "&password=" + p;
                
            
                var client = new RestClient(query);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                //request.AddHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwOlwvXC8xMjcuMC4wLjE6ODAwMFwvYXBpXC9sb2dpbiIsImlhdCI6MTYyNjQxNzUyNywiZXhwIjoxNjI2NDIxMTI3LCJuYmYiOjE2MjY0MTc1MjcsImp0aSI6IkxaM0g4b2ViQWM4ODNGS1ciLCJzdWIiOjEsInBydiI6Ijg3ZTBhZjFlZjlmZDE1ODEyZmRlYzk3MTUzYTE0ZTBiMDQ3NTQ2YWEifQ.b0oi6Rhi94cslo_42fzjUi4PXoZgrZNoIrBplRSmSzI");
                IRestResponse response = client.Execute(request);


                Console.WriteLine("[*] Response: "+response.Content);

            try
            {


               

                 JSONObject jobject = new JSONObject(response.Content);

                if (jobject.GetBoolean("success"))
                {
                JSONObject user = jobject.GetJSONObject("user");


                ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(Context);
                ISharedPreferencesEditor editor = pref.Edit();

                editor.PutString("token", jobject.GetString("token"));
                editor.PutString("name", user.GetString("name"));
                editor.PutInt("id", user.GetInt("id"));
                editor.PutString("lastname", user.GetString("lastname"));
                editor.PutString("photo", user.GetString("photo"));
                editor.PutBoolean("isLoggedIn", true);
                editor.Apply();

                Context.StartActivity(typeof(HomeActivity));
                ((AuthActivity)Context).Finish();

                Toast.MakeText(Context, "Login Success", ToastLength.Short).Show();
                   // dialog.Dismiss();
                }
                else if(!jobject.GetBoolean("success"))
                {
                    Toast.MakeText(Context, "Login Failed,Passowrd or Email Wrong", ToastLength.Long).Show();
                    
                   dialog.Dismiss();
                }

        }
            catch (JSONException ee)
            {
                ee.PrintStackTrace();

                Console.WriteLine("[-] Exception: "+ee);
            }

            dialog.Dismiss();

            

        }


    }
}