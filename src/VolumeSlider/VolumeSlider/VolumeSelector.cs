using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace VolumeSlider.Forms.Plugin.Shared
{
    /// <summary>
    /// Slider based volume selector control.
    /// </summary>
    public class VolumeSelector : View
    {
        /// <summary>
        /// Gets raised if the volume was changed (either by moving the slider or because the system volume was changed in response to hardware keys or softkeys).
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> VolumeChanged;

        /// <summary>
        /// Bindable property to set and get the volume. Volume is normalized to values between 0 and 1.
        /// </summary>
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

        /// <summary>
        /// The minimum track color property.
        /// </summary>
        public static readonly BindableProperty MinimumTrackColorProperty = BindableProperty.Create(nameof(MinimumTrackColor), typeof(Color), typeof(VolumeSelector), Color.Default);

        /// <summary>
        /// The maximum track color property.
        /// </summary>
        public static readonly BindableProperty MaximumTrackColorProperty = BindableProperty.Create(nameof(MaximumTrackColor), typeof(Color), typeof(VolumeSelector), Color.Default);

        /// <summary>
        /// The thumb color property.
        /// </summary>
        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(VolumeSelector), Color.Default);

        /// <summary>
        /// The thumb image property.
        /// </summary>
        public static readonly BindableProperty ThumbImageProperty = BindableProperty.Create(nameof(ThumbImage), typeof(FileImageSource), typeof(VolumeSelector), default(FileImageSource));

        /// <summary>
        /// Get or set the volume. Values should be in range from 0 to 1.
        /// </summary>
        public double Volume
        {
            get => (double)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum color of the slider.
        /// </summary>
        public Color MinimumTrackColor
        {
            get => (Color)GetValue(MinimumTrackColorProperty);
            set => SetValue(MinimumTrackColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum color of the slider.
        /// </summary>
        public Color MaximumTrackColor
        {
            get => (Color)GetValue(MaximumTrackColorProperty);
            set => SetValue(MaximumTrackColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the slider thumb.
        /// </summary>
        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the slider's thumb image.
        /// </summary>
        public FileImageSource ThumbImage
        {
            get => (FileImageSource)GetValue(ThumbImageProperty);
            set => SetValue(ThumbImageProperty, value);
        }
    }
}
