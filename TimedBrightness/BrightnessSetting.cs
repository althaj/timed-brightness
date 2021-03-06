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
    public class BrightnessSetting : IComparable
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
        public int Brightness { get; set; }

        public BrightnessSetting()
        {

        }

        public BrightnessSetting(TimeSpan time)
        {
            Hour = time.Hours;
            int minutes = Math.Clamp((int)(Math.Ceiling(time.Minutes / 10.0d) * 10), 0, 60);
            MinuteString = minutes.ToString();
        }

        public int CompareTo(object obj)
        {
            BrightnessSetting other = (BrightnessSetting)obj;

            if (Hour == other.Hour && Minute == other.Minute)
                return 0;

            if (Hour > other.Hour || Hour == other.Hour && Minute > other.Minute)
                return 1;

            return -1;
        }
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

            vh.BrightnessBar.Max = 255;
            vh.BrightnessBar.Progress = (item.Brightness);

            vh.DeleteButton.Click -= DeleteItemOnClick;
            vh.DeleteButton.Click += DeleteItemOnClick;

            vh.HourPicker.ValueChanged += HourPicker_ValueChanged;
            vh.MinutePicker.ValueChanged += MinutePicker_ValueChanged;
            vh.BrightnessBar.ProgressChanged += BrightnessBar_ProgressChanged;
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

            Handler h = new Handler(Looper.MainLooper);
            Action deleteAction = () =>
            {
                items.RemoveAt(view.GetChildAdapterPosition(layout));
                NotifyDataSetChanged();
            };

            h.PostDelayed(deleteAction, animation.Duration);
        }

        private void HourPicker_ValueChanged(object sender, NumberPicker.ValueChangeEventArgs e)
        {
            NumberPicker picker = (NumberPicker)sender;
            LinearLayout layout = picker.Parent as LinearLayout;
            RecyclerView view = layout.Parent as RecyclerView;

            try
            {
                int index = view.GetChildAdapterPosition(layout);
                items[index].Hour = picker.Value;
            }
            catch { }
        }

        private void MinutePicker_ValueChanged(object sender, NumberPicker.ValueChangeEventArgs e)
        {
            NumberPicker picker = (NumberPicker)sender;
            LinearLayout layout = picker.Parent as LinearLayout;
            RecyclerView view = layout.Parent as RecyclerView;

            try
            {
                int index = view.GetChildAdapterPosition(layout);
                items[index].MinuteString = BrightnessSetting.minuteValues[picker.Value];
            }
            catch { }
        }

        private void BrightnessBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            SeekBar seekBar = (SeekBar)sender;
            LinearLayout layout = seekBar.Parent as LinearLayout;
            RecyclerView view = layout.Parent as RecyclerView;

            try
            {
                int index = view.GetChildAdapterPosition(layout);
                items[index].Brightness = seekBar.Progress;
            }
            catch { }
        }
    }
}