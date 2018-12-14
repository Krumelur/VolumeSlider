using MediaPlayer;
using System;
using System.ComponentModel;
using UIKit;
using SizeF = CoreGraphics.CGSize;
using Xamarin.Forms;
using VolumeSliderPlugin.Shared;
using VolumeSliderPlugin.iOS;
using Xamarin.Forms.Platform.iOS;
using System.Linq;

[assembly: ExportRenderer(typeof(VolumeSlider), typeof(VolumeSliderRenderer))]

namespace VolumeSliderPlugin.iOS
{
	/// <summary>
	/// Custom renderer for a volume selector on iOS.
	/// This renderer is based on the original Xamarin.Forms Slider renderer and uses source code portions from
	/// https://github.com/xamarin/Xamarin.Forms/blob/bd31e1e9fc8b2f9ad94cc99e0c7ab058174821f3/Xamarin.Forms.Platform.iOS/Renderers/SliderRenderer.cs
	/// </summary>
	public class VolumeSliderRenderer : ViewRenderer<VolumeSlider, MPVolumeView>
    {
        SizeF _fitSize;
        UIColor defaultmintrackcolor, defaultmaxtrackcolor, defaultthumbcolor;

        // The UISlider inside of the MPVolumeView. This is not meant to be accessed directly.
        // No guarantee this will work with later versions of iOS.
        UISlider InnerSlider => Control == null ? null : (UISlider)Control.Subviews.FirstOrDefault(v => v is UISlider);

        protected override void Dispose(bool disposing)
        {
            if (InnerSlider != null)
            {
                InnerSlider.ValueChanged -= OnControlValueChanged;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VolumeSlider> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new MPVolumeView());
                    InnerSlider.ValueChanged += OnControlValueChanged;

                    Control.SizeToFit();
                    _fitSize = Control.Bounds.Size;

                    defaultmintrackcolor = InnerSlider.MinimumTrackTintColor;
                    defaultmaxtrackcolor = InnerSlider.MaximumTrackTintColor;
                    defaultthumbcolor = InnerSlider.ThumbTintColor;

                    // except if your not running iOS 7... then it fails...
                    if (_fitSize.Width <= 0 || _fitSize.Height <= 0)
                    {
                        _fitSize = new SizeF(22, 22); // Per the glorious documentation known as the SDK docs
                    }
                }

                UpdateVolume();
                UpdateSliderColors();
            }

            base.OnElementChanged(e);
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
            if (Element != null)
            {
                if (Element.MinimumTrackColor == Color.Default)
                {
                    InnerSlider.MinimumTrackTintColor = defaultmintrackcolor;
                }
                else
                {
                    InnerSlider.MinimumTrackTintColor = Element.MinimumTrackColor.ToUIColor();
                }
            }
        }

        private void UpdateMaximumTrackColor()
        {
            if (Element != null)
            {
                if (Element.MaximumTrackColor == Color.Default)
                {
                    InnerSlider.MaximumTrackTintColor = defaultmaxtrackcolor;
                }
                else
                {
                    InnerSlider.MaximumTrackTintColor = Element.MaximumTrackColor.ToUIColor();
                }
            }
        }

        private void UpdateThumbColor()
        {
            if (Element != null)
            {
                if (Element.ThumbColor == Color.Default)
                {
                    InnerSlider.ThumbTintColor = defaultthumbcolor;
                }
                else
                {
                    InnerSlider.ThumbTintColor = Element.ThumbColor.ToUIColor();
                }
            }
        }

        async void UpdateThumbImage()
        {
            IImageSourceHandler handler;
            FileImageSource source = Element.ThumbImage;
            if (source != null && (handler = Xamarin.Forms.Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
            {
                UIImage uiimage;
                try
                {
                    uiimage = await handler.LoadImageAsync(source, scale: (float)UIScreen.MainScreen.Scale);
                }
                catch (OperationCanceledException)
                {
                    uiimage = null;
                }
                MPVolumeView volumeView = Control;
                if (volumeView != null && uiimage != null)
                {
                    volumeView.SetVolumeThumbImage(uiimage, UIControlState.Normal);
                }
            }
            else
            {
                MPVolumeView volumeView = Control;
                if (volumeView != null)
                {
                    volumeView.SetVolumeThumbImage(null, UIControlState.Normal);
                }
            }
            ((IVisualElementController)Element).NativeSizeChanged();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VolumeSlider.VolumeProperty.PropertyName)
            {
                UpdateVolume();
            }
            else if (e.PropertyName == VolumeSlider.MinimumTrackColorProperty.PropertyName)
            {
                UpdateMinimumTrackColor();
            }
            else if (e.PropertyName == VolumeSlider.MaximumTrackColorProperty.PropertyName)
            {
                UpdateMaximumTrackColor();
            }
            else if (e.PropertyName == VolumeSlider.ThumbImageProperty.PropertyName)
            {
                UpdateThumbImage();
            }
            else if (e.PropertyName == VolumeSlider.ThumbColorProperty.PropertyName)
            {
                UpdateThumbColor();
            }
        }

        /// <summary>
        /// Gets or sets the current volume from/on the native slider.
        /// </summary>
        double NormalizedVolume
        {
            get
            {
                if(InnerSlider == null)
                {
                    return 0f;
                }
                var normalizedVolume = InnerSlider.Value / (InnerSlider.MaxValue - InnerSlider.MinValue);
                return normalizedVolume;
            }
            set
            {
                if(InnerSlider == null)
                {
                    return;
                }
                var absoluteVolume = InnerSlider.MinValue + value * (InnerSlider.MaxValue - InnerSlider.MinValue);
                InnerSlider.Value = (float)absoluteVolume;
            }
        }

        void OnControlValueChanged(object sender, EventArgs eventArgs)
        {
            ((IElementController)Element).SetValueFromRenderer(VolumeSlider.VolumeProperty, NormalizedVolume);
        }

        void UpdateVolume()
        {
            if(InnerSlider == null)
            {
                return;
            }

            if (Math.Abs((float)Element.Volume - NormalizedVolume) > 0.01)
            {
                InnerSlider.Value = (float)Element.Volume;
            }
        }
    }
}