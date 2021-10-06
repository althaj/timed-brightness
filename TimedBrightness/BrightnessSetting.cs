using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TimedBrightness
{
    public class BrightnessSetting
    {
        public static readonly string[] minuteValues = new string[] { "00", "10", "20", "30", "40", "50" };

        public int Hour { get; set; }
        public int Minute
        {
            get
            {
                int result = 0;
                int.TryParse(MinuteString, out result);
                return result;
            }
        }
        public int MinuteIndex
        {
            get
            {
                return Array.IndexOf(minuteValues, MinuteString);
            }
        }
        public string MinuteString { get; set; }
        public float Brightness { get; set; }
    }

    public class BrightnessSettingViewHolder : RecyclerView.ViewHolder
    {
        public NumberPicker HourPicker { get; private set; }
        public NumberPicker MinutePicker { get; private set; }
        public SeekBar BrightnessBar { get; private set; }
        public ImageButton DeleteButton { get; private set; }

        public BrightnessSettingViewHolder(View view) : base(view)
        {
            HourPicker = view.FindViewById<NumberPicker>(Resource.Id.hourPicker);
            MinutePicker = view.FindViewById<NumberPicker>(Resource.Id.minutePicker);
            BrightnessBar = view.FindViewById<SeekBar>(Resource.Id.brightnessBar);
            DeleteButton = view.FindViewById<ImageButton>(Resource.Id.deleteButton);
        }
    }

    public class BrightnessSettingAdapter : RecyclerView.Adapter
    {
        List<BrightnessSetting> items;
        Activity context;

        public BrightnessSettingAdapter(Activity context, List<BrightnessSetting> items)
        {
            this.context = context;
            this.items = items;
        }

        public override int ItemCount => items != null ? items.Count() : 0;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BrightnessSettingViewHolder vh = holder as BrightnessSettingViewHolder;
            BrightnessSetting item = items[position];

            vh.HourPicker.Value = 0;
            vh.HourPicker.MinValue = 0;
            vh.HourPicker.MaxValue = 23;
            vh.HourPicker.Value = item.Hour;

            vh.MinutePicker.MinValue = 0;
            vh.MinutePicker.MaxValue = 5;
            vh.MinutePicker.SetDisplayedValues(BrightnessSetting.minuteValues);
            vh.MinutePicker.Value = item.MinuteIndex;

            vh.BrightnessBar.Max = 100;
            vh.BrightnessBar.Progress = (int)(item.Brightness * 100);

            vh.DeleteButton.Click -= DeleteItemOnClick;
            vh.DeleteButton.Click += DeleteItemOnClick;

            vh.ItemView.LayoutParameters.Height = 128;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item, parent, false);
            BrightnessSettingViewHolder vh = new BrightnessSettingViewHolder(itemView);
            return vh;
        }

        private void DeleteItemOnClick(object sender, EventArgs e)
        {
            LinearLayout layout = (sender as View).Parent as LinearLayout;
            RecyclerView view = layout.Parent as RecyclerView;

            Animation animation = AnimationUtils.LoadAnimation(context, Resource.Animation.abc_popup_exit);
            animation.Duration = 300;
            layout.StartAnimation(animation);

            Handler h = new Handler();
            Action deleteAction = () =>
            {
                items.RemoveAt(view.GetChildAdapterPosition(layout));
                NotifyDataSetChanged();
            };

            h.PostDelayed(deleteAction, animation.Duration);
        }
    }
}