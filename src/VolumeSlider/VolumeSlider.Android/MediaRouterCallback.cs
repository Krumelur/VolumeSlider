using System;
using Android.Media;

namespace VolumeSlider.Forms.Plugin.Droid
{
	/// <summary>
	/// Handles callbacks from Android when changing the volume using the hardware buttons of the device
	/// or the volume controls in the notification center.
	/// </summary>
	internal class CustomMediaRouterCallback : MediaRouter.SimpleCallback
    {
		/// <summary>
		/// Event will be triggered if a volume change is detected.
		/// </summary>
		internal event EventHandler<VolumeChangedEventArgs> VolumeChanged;

        public override void OnRouteVolumeChanged(MediaRouter router, MediaRouter.RouteInfo info)
        {
			VolumeChanged?.Invoke(router, new VolumeChangedEventArgs {
				Volume = info.Volume,
				PlaybackStream = info.PlaybackStream,
			});
        }
    }
}