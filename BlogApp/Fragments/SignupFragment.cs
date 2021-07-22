using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;
using Org.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volley;
using Volley.Toolbox;



namespace BlogApp.Fragments
{
    class SignupFragment : Fragment, Android.Text.ITextWatcher
    {
        private View view;
        private TextInputLayout layoutEmail, layoutPassword, layoutConfirm;
        private TextInputEditText txtEmail, txtPassword, txtConfirm;
        private TextView txtSignIn;
        private Button btnSignUp;
        private ProgressDialog dialog;

        public SignupFragment()
        {
        }

        public void AfterTextChanged(Android.Text.IEditable s)
        {
            //  throw new NotImplementedException();
        }

        public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
        {
            //  throw new NotImplementedException();
        }

        [Obsolete]
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.layout_sign_up, container, false);
            init();
            return view;
        }

        public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
        {
            if (!String.IsNullOrEmpty(txtEmail.Text.ToString()))
            {
                layoutEmail.ErrorEnabled = true;
                //  layoutEmail.Error = "Email is Required";
            }

            if (txtPassword.Text.ToString().Length > 7)
            {
                layoutPassword.ErrorEnabled = false;
            }

            if (txtConfirm.Text.ToString().Equals(txtPassword.Text.ToString()))
            {
                layoutConfirm.ErrorEnabled = false;
            }
        }

        [Obsolete]
        private void init()
        {
            layoutPassword = view.FindViewById<TextInputLayout>(Resource.Id.txtLayoutPasswordSignUp);
            layoutEmail = view.FindViewById<TextInputLayout>(Resource.Id.txtLayoutEmailSignUp);
            layoutConfirm = view.FindViewById<TextInputLayout>(Resource.Id.txtLayoutConfrimSignUp);
            txtPassword = view.FindViewById<TextInputEditText>(Resource.Id.txtPasswordSignUp);
            txtConfirm = view.FindViewById<TextInputEditText>(Resource.Id.txtConfirmSignUp);

            txtSignIn = view.FindViewById<TextView>(Resource.Id.txtSignIn);
            txtEmail = view.FindViewById<TextInputEditText>(Resource.Id.txtEmailSignUp);
            btnSignUp = view.FindViewById<Button>(Resource.Id.btnSignUp);
            dialog = new ProgressDialog(Context);
            dialog.SetCancelable(false);

            txtSignIn.Click += delegate
            {
                Activity.FragmentManager.BeginTransaction().Replace(Resource.Id.frameAuthContainer, new SigninFragment()).Commit();
            };

            btnSignUp.Click += delegate
            {
                if (validate())
                {
                    register(txtEmail.Text.ToString(),txtPassword.Text.ToString());
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
                if (txtPassword.Text.ToString().Length > 8)
                {
                    layoutPassword.ErrorEnabled = false;
                }
            };
            txtConfirm.TextChanged += (sender, e) =>
            {
                if (txtConfirm.Text.ToString().Equals(txtPassword.Text.ToString()))
                {
                    layoutConfirm.ErrorEnabled = false;
                }
            };

        }

        private void register(string email,string password)
        {
            dialog.SetMessage("Registering");
            dialog.Show();



            string query = Constant.REGISTER + "?email=" + email + "&password=" + password;



            var client = new RestClient(query);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwOlwvXC8xMjcuMC4wLjE6ODAwMFwvYXBpXC9sb2dpbiIsImlhdCI6MTYyNjQxNzUyNywiZXhwIjoxNjI2NDIxMTI3LCJuYmYiOjE2MjY0MTc1MjcsImp0aSI6IkxaM0g4b2ViQWM4ODNGS1ciLCJzdWIiOjEsInBydiI6Ijg3ZTBhZjFlZjlmZDE1ODEyZmRlYzk3MTUzYTE0ZTBiMDQ3NTQ2YWEifQ.b0oi6Rhi94cslo_42fzjUi4PXoZgrZNoIrBplRSmSzI");
            IRestResponse response = client.Execute(request);


            Console.WriteLine("[*] Response: " + response.Content);
       

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

                    Context.StartActivity(typeof(UserInfoActivity));
                    ((AuthActivity)Context).Finish();

                    Toast.MakeText(Context, "Register Success", ToastLength.Short).Show();

                }
                else if (!jobject.GetBoolean("success"))
                {
                    Toast.MakeText(Context, "Register Failed,Passowrd or Email Wrong", ToastLength.Long).Show();

                    dialog.Dismiss();
                }

            }
            catch (JSONException ee)
            {
                ee.PrintStackTrace();

                Console.WriteLine("[-] Exception: " + ee);
            }

            dialog.Dismiss();

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

            if (!txtConfirm.Text.ToString().Equals(txtPassword.Text.ToString()))
            {
                layoutConfirm.ErrorEnabled = true;
                layoutConfirm.Error = "Password does not match";
                return false;
            }


            return true;

        }
    }
     

}