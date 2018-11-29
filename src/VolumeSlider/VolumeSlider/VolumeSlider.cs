using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace VolumeSlider.Forms.Plugin.Shared
{
    public class VolumeSelector : View
    {
        public event EventHandler<ValueChangedEventArgs> VolumeChanged;

        public static readonly BindableProperty VolumeProperty = BindableProperty.Create("Volume", typeof(double), typeof(VolumeSelector), 0.5d, BindingMode.TwoWay,
            coerceValue: (bindable, value) =>
        {
            var slider = (VolumeSelector)bindable;
            return ((double)value).Clamp(0, 1);
        },
            propertyChanged: (bindable, oldValue, newValue) =>
        {
            var slider = (VolumeSelector)bindable;
            slider.VolumeChanged?.Invoke(slider, new ValueChangedEventArgs((double)oldValue, (double)newValue));
        });

        public static readonly BindableProperty MinimumTrackColorProperty = BindableProperty.Create(nameof(MinimumTrackColor), typeof(Color), typeof(VolumeSelector), Color.Default);

        public static readonly BindableProperty MaximumTrackColorProperty = BindableProperty.Create(nameof(MaximumTrackColor), typeof(Color), typeof(VolumeSelector), Color.Default);

        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(VolumeSelector), Color.Default);

        public static readonly BindableProperty ThumbImageProperty = BindableProperty.Create(nameof(ThumbImage), typeof(FileImageSource), typeof(VolumeSelector), default(FileImageSource));


        public double Volume
        {
            get => (double)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public Color MinimumTrackColor
        {
            get => (Color)GetValue(MinimumTrackColorProperty);
            set => SetValue(MinimumTrackColorProperty, value);
        }

        public Color MaximumTrackColor
        {
            get => (Color)GetValue(MaximumTrackColorProperty);
            set => SetValue(MaximumTrackColorProperty, value);
        }

        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        public FileImageSource ThumbImage
        {
            get => (FileImageSource)GetValue(ThumbImageProperty);
            set => SetValue(ThumbImageProperty, value);
        }
    }
}
