using Android.Content;
using Android.Runtime;
using Android.Views;

using Android.Widget;
using System;

using AndroidX.ViewPager.Widget;
//using Android.Support.V4.View;

namespace BlogApp.Adapters
{

   
    class ViewPagerAdapter : PagerAdapter
    {

        Context context;
        Context _context;
        private LayoutInflater Inflater;
        

        public ViewPagerAdapter(Context context)
        {
            this.context = context;
        }

        private int[] images=
        {
            Resource.Drawable.p1,
            Resource.Drawable.p2,
            Resource.Drawable.p3
        };

        private string[] titles =
        {
            "Learn",
            "Create",
            "Enjoy"
        };

        private String[] descs ={
            "lorem  ipsum dolor contraint spaces dolor ipsum loremters termainal lorem ispsum contanirnts.",
            "lorem  ipsum dolor contraint spaces dolor ipsum loremters termainal lorem ispsum contanirnts.",
            "lorem  ipsum dolor contraint spaces dolor ipsum loremters termainal lorem ispsum contanirnts."
    };

        public  Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public  long GetItemId(int position)
        {
            return position;
        }

        public  View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ViewPagerAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ViewPagerAdapterViewHolder;

            if (holder == null)
            {
                holder = new ViewPagerAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                //view = inflater.Inflate(Resource.Layout.item, parent, false);
                //holder.Title = view.FindViewById<TextView>(Resource.Id.text);
                view.Tag = holder;
            }


            //fill in your items
            //holder.Title.Text = "new text here";

            return view;
        }

        public override bool  IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == ((LinearLayout)@object);
        }

        
        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {

          //  View v = container;


            //Inflater = LayoutInflater.From(container.Context);
            //View v = Inflater.Inflate(Resource.Layout.view_pager, null,true);

            var inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            View v = inflater.Inflate(Resource.Layout.view_pager, container,false);

            ImageView imageView = v.FindViewById<ImageView>(Resource.Id.imgViewPager);
            TextView txtTitle = v.FindViewById<TextView>(Resource.Id.txtTitleViewPager);
            TextView txtDesc = v.FindViewById<TextView>(Resource.Id.txtDescViewPager);

            imageView.SetImageResource(images[position]);
            txtTitle.Text = titles[position];
            txtDesc.Text=(descs[position]);

            //container.AddView;

            container.AddView(v);
            
            return v;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((LinearLayout)@object);
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return titles.Length;
            }
        }

    }

    class ViewPagerAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}