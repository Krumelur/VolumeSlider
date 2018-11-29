using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace VolumeSlider
{
    [RenderWith(typeof(VolumeSliderRenderer))]
    public class VolumeSlider : View
    {
        public enum StreamType
        {
            Music = 0,
            VoiceCall,
            System,
            Ring,
            Alarm,
            Accessibility
        }

        public event EventHandler<ValueChangedEventArgs> VolumeChanged;

        public static readonly BindableProperty VolumeProperty = BindableProperty.Create("Volume", typeof(double), typeof(VolumeSlider), 0.5d, BindingMode.TwoWay,
            coerceValue: (bindable, value) =>
        {
            var slider = (VolumeSlider)bindable;
            return ((double)value).Clamp(0, 1);
        },
            propertyChanged: (bindable, oldValue, newValue) =>
        {
            var slider = (VolumeSlider)bindable;
            slider.VolumeChanged?.Invoke(slider, new ValueChangedEventArgs((double)oldValue, (double)newValue));
        });

        public static readonly BindableProperty StreamTypeProperty = BindableProperty.Create("Stream", typeof(StreamType), typeof(VolumeSlider), StreamType.Music, BindingMode.TwoWay);

        public static readonly BindableProperty MinimumTrackColorProperty = BindableProperty.Create(nameof(MinimumTrackColor), typeof(Color), typeof(VolumeSlider), Color.Default);

        public static readonly BindableProperty MaximumTrackColorProperty = BindableProperty.Create(nameof(MaximumTrackColor), typeof(Color), typeof(VolumeSlider), Color.Default);

        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(VolumeSlider), Color.Default);

        public static readonly BindableProperty ThumbImageProperty = BindableProperty.Create(nameof(ThumbImage), typeof(FileImageSource), typeof(VolumeSlider), default(FileImageSource));


        public double Volume
        {
            get => (double)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public StreamType Stream
        {
            get => (StreamType)GetValue(StreamTypeProperty);
            set => SetValue(StreamTypeProperty, value);
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
            set => SetValue(ThumbImageProperty, value);
        }
    }
}
