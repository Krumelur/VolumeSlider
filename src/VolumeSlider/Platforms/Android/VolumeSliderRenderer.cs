using System;
using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using VolumeSliderPlugin.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using Android.Media;
using VolumeSliderPlugin.Shared;

[assembly: ExportRenderer(typeof(VolumeSlider), typeof(VolumeSliderRenderer))]

namespace VolumeSliderPlugin.Droid
{
	/// <summary>
	/// Custom renderer for a volume selector on Android.
	/// This renderer is based on the original Xamarin.Forms Slider renderer and uses source code portions from
	/// https://github.com/xamarin/Xamarin.Forms/blob/bd31e1e9fc8b2f9ad94cc99e0c7ab058174821f3/Xamarin.Forms.Platform.Android/Renderers/SliderRenderer.cs
	/// It tracks Android native volume controls (eg the hardware buttons) and updates itself.
	/// </summary>
	public class VolumeSliderRenderer : ViewRenderer<VolumeSlider, SeekBar>, SeekBar.IOnSeekBarChangeListener
	{
		bool _isTrackingChange;
		ColorStateList defaultprogresstintlist, defaultprogressbackgroundtintlist;
		ColorFilter defaultthumbcolorfilter;
		Drawable defaultthumb;
		PorterDuff.Mode defaultprogresstintmode, defaultprogressbackgroundtintmode;
		MediaRouter _mediaRouter;
		readonly CustomMediaRouterCallback _mediaRouterCallback = new CustomMediaRouterCallback();
		SeekBar NativeSeekbar => Control;

		public VolumeSliderRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		/// <summary>
		/// Gets or sets the current volume from/on the native slider.
		/// The value is normalized from the slider's [0..1000] range to [0..1] so it can be used as a relative value.
		/// </summary>
		double NormalizedVolume
		{
			get => Control.Progress / 1000.0;
			set => Control.Progress = (int)(value * 1000.0);
		}

		/// <summary>
		/// Gets called if the value of the native SeekBar is changing.
		/// </summary>
		/// <param name="seekBar"></param>
		/// <param name="progress"></param>
		/// <param name="fromUser"></param>
		void SeekBar.IOnSeekBarChangeListener.OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
		{
			if (_isTrackingChange)
			{
				((IElementController)Element).SetValueFromRenderer(VolumeSlider.VolumeProperty, NormalizedVolume);

				var audioManager = (AudioManager)Context.GetSystemService(Context.AudioService);
				// If the native SeekBar's value is changing, we're updating the devices audio volume
				// relative to the maximum volume.
				var maxVolume = audioManager.GetStreamMaxVolume(Stream.Music);
				var volumeIndex = (int)Math.Round(NormalizedVolume * (double)maxVolume);
				audioManager.SetStreamVolume(Stream.Music, volumeIndex, VolumeNotificationFlags.Vibrate);
			}
		}

		void SeekBar.IOnSeekBarChangeListener.OnStartTrackingTouch(SeekBar seekBar) => _isTrackingChange = true;

		void SeekBar.IOnSeekBarChangeListener.OnStopTrackingTouch(SeekBar seekBar) => _isTrackingChange = false;

		protected override SeekBar CreateNativeControl() => new CustomSeekBar(Context)
		{
			// Android's SeekBar is using integers for the range. To get fine enough granularity, we're scaling the range from 0 to 1000.
			Max = 1000
		};

		protected override void OnElementChanged(ElementChangedEventArgs<VolumeSlider> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var seekBar = CreateNativeControl();
				SetNativeControl(seekBar);
				seekBar.SetOnSeekBarChangeListener(this);

				if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
				{
					defaultthumbcolorfilter = seekBar.Thumb.ColorFilter;
					defaultprogresstintmode = seekBar.ProgressTintMode;
					defaultprogressbackgroundtintmode = seekBar.ProgressBackgroundTintMode;
					defaultprogresstintlist = seekBar.ProgressTintList;
					defaultprogressbackgroundtintlist = seekBar.ProgressBackgroundTintList;
					defaultthumb = seekBar.Thumb;
				}

				_mediaRouterCallback.VolumeChanged += HandleDeviceVolumeChanged;
				_mediaRouter = (MediaRouter)Context.GetSystemService(Context.MediaRouterService);
				_mediaRouter.AddCallback(MediaRouteType.LiveAudio, _mediaRouterCallback);
			}

			if(e.NewElement == null)
			{
				_mediaRouterCallback.VolumeChanged -= HandleDeviceVolumeChanged;
				_mediaRouter.RemoveCallback(_mediaRouterCallback);
			}
			
			// If the Forms Volume property is changing, we're updating the 
			VolumeSlider slider = e.NewElement;
			NormalizedVolume = slider.Volume;
			if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
			{
				UpdateSliderColors();
			}
		}

		void HandleDeviceVolumeChanged(object sender, VolumeChangedEventArgs e)
		{
			Console.WriteLine($"Device volume changed to {e.Volume} for stream type {e.PlaybackStream}");
			if (e.PlaybackStream != Stream.Music)
			{
				// Would be possible to make the volume control work for other stream types.
				return;
			}

			// Convert system volume back to normalized volume value.
			var audioManager = (AudioManager)Context.GetSystemService(Context.AudioService);
			var maxVolume = audioManager.GetStreamMaxVolume(e.PlaybackStream);
			NormalizedVolume = (double)e.Volume / (double)maxVolume;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			VolumeSlider view = Element;
			if(e.PropertyName == VolumeSlider.VolumeProperty.PropertyName && NormalizedVolume != view.Volume)
			{ 
				NormalizedVolume = view.Volume;
			}

			if (Build.VERSION.SdkInt > BuildVersionCodes.Kitkat)
			{
				if (e.PropertyName == Slider.MinimumTrackColorProperty.PropertyName)
				{
					UpdateMinimumTrackColor();
				}
				else if (e.PropertyName == Slider.MaximumTrackColorProperty.PropertyName)
				{
					UpdateMaximumTrackColor();
				}
				else if (e.PropertyName == Slider.ThumbImageProperty.PropertyName)
				{
					UpdateThumbImage();
				}
				else if (e.PropertyName == Slider.ThumbColorProperty.PropertyName)
				{
					UpdateThumbColor();
				}
			}
		}

		private void UpdateSliderColors()
		{
			UpdateMinimumTrackColor();
			UpdateMaximumTrackColor();

			if (!string.IsNullOrEmpty(Element.ThumbImage))
			{
				UpdateThumbImage();
			}
			else
			{
				UpdateThumbColor();
			}
		}

		private void UpdateMinimumTrackColor()
		{
			if (Element == null)
			{
				return;
			}

			if (Element.MinimumTrackColor == Color.Default)
			{
				Control.ProgressTintList = defaultprogresstintlist;
				Control.ProgressTintMode = defaultprogresstintmode;
			}
			else
			{
				Control.ProgressTintList = ColorStateList.ValueOf(Element.MinimumTrackColor.ToAndroid());
				Control.ProgressTintMode = PorterDuff.Mode.SrcIn;
			}
		}

		private void UpdateMaximumTrackColor()
		{
			if (Element == null)
			{
				return;
			}

			if (Element.MaximumTrackColor == Color.Default)
			{
				Control.ProgressBackgroundTintList = defaultprogressbackgroundtintlist;
				Control.ProgressBackgroundTintMode = defaultprogressbackgroundtintmode;
			}
			else
			{
				Control.ProgressBackgroundTintList = ColorStateList.ValueOf(Element.MaximumTrackColor.ToAndroid());
				Control.ProgressBackgroundTintMode = PorterDuff.Mode.SrcIn;
			}
		}

		private void UpdateThumbColor()
		{
			if (Element == null)
			{
				return;
			}

			if (Element.ThumbColor == Color.Default)
			{
				Control.Thumb.SetColorFilter(defaultthumbcolorfilter);
			}
			else
			{
				Control.Thumb.SetColorFilter(Element.ThumbColor.ToAndroid(), PorterDuff.Mode.SrcIn);
			}
		}

		private void UpdateThumbImage()
		{
			if (Element == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(Element.ThumbImage))
			{
				Control.SetThumb(defaultthumb);
			}
			else
			{
				Control.SetThumb(Context.GetDrawable(Element.ThumbImage));
			}

		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			BuildVersionCodes androidVersion = Build.VERSION.SdkInt;
			if (androidVersion < BuildVersionCodes.JellyBean)
			{
				return;
			}

			// Thumb only supported JellyBean and higher

			if (Control == null)
			{
				return;
			}

			SeekBar seekbar = Control;

			Drawable thumb = seekbar.Thumb;
			int thumbTop = seekbar.Height / 2 - thumb.IntrinsicHeight / 2;

			thumb.SetBounds(thumb.Bounds.Left, thumbTop, thumb.Bounds.Left + thumb.IntrinsicWidth, thumbTop + thumb.IntrinsicHeight);
		}

		protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _mediaRouter.RemoveCallback(_mediaRouterCallback);
        }
	}
}