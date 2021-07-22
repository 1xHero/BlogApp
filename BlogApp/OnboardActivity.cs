using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
//using Android.Support.V4.View;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using BlogApp.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogApp
{
    [Activity(Label = "OnboardActivity")]
    public class OnboardActivity : Activity, Android.Support.V4.View.ViewPager.IOnPageChangeListener
    {

        private ViewPager viewPager;
        private Button btnLeft, btnRight;
        private ViewPagerAdapter adapter;
        private LinearLayout dotsLayout;
        private TextView[] dots;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_on_board);

            init();
        }

        private void init()
        {
            viewPager = FindViewById<ViewPager>(Resource.Id.view_pager);
            btnLeft = FindViewById<Button>(Resource.Id.btnLeft);
            btnRight = FindViewById<Button>(Resource.Id.btnRight);
            dotsLayout = FindViewById<LinearLayout>(Resource.Id.dotsLayout);
            adapter = new ViewPagerAdapter(this);
            addDots(0);
            viewPager.AddOnPageChangeListener((ViewPager.IOnPageChangeListener)this);
            
            viewPager.Adapter = adapter;

            btnRight.Click += delegate {

                if (btnRight.Text.Equals("Next"))
                {
                    viewPager.SetCurrentItem(viewPager.CurrentItem + 1,true);
                }
                else
                {
                    StartActivity(typeof(AuthActivity));
                    Finish();
                }
            
            };

            btnLeft.Click += delegate
            {
                viewPager.SetCurrentItem(viewPager.CurrentItem + 2, true);
            };
        }

        //method to create dots from html code
        private void addDots(int position)
        {
            dotsLayout.RemoveAllViews();
            dots = new TextView[3];
            for(int i = 0; i < dots.Length; i++)
            {
                dots[i] = new TextView(this);
                dots[i].Text = Html.FromHtml("&#8226").ToString();
                dots[i].SetTextSize(Android.Util.ComplexUnitType.Px,35);//text size
                dots[i].SetTextColor(Resources.GetColor(Resource.Color.colorGrey));
                dotsLayout.AddView(dots[i]);
            }


            if (dots.Length > 0)
            {
                dots[position].SetTextColor(Resources.GetColor(Resource.Color.colorGrey));
            }
        }

        public void OnPageScrollStateChanged(int state)
        {
           // throw new NotImplementedException();
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //throw new NotImplementedException();
        }

        public void OnPageSelected(int position)
        {
            addDots(position);

            if (position == 0)
            {
                btnLeft.Enabled = true;
                btnLeft.Visibility = Android.Views.ViewStates.Visible;
                btnRight.Text = "Next";
            }
            else if (position == 1)
            {
                btnLeft.Enabled = false;
                btnLeft.Visibility = Android.Views.ViewStates.Gone;
                btnRight.Text = "Next";
            }
            else
            {
                btnLeft.Enabled = false;
                btnLeft.Visibility = Android.Views.ViewStates.Gone;
                btnRight.Text = "Finish";
            }
        }
    }
}