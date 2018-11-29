using Android.Media;

namespace VolumeSlider.Forms.Plugin.Droid
{
	/// <summary>
	/// Event arguments for volume changes.
	/// </summary>
	internal class VolumeChangedEventArgs
	{
		/// <summary>
		/// The absolute volume level as reported by the device.
		/// </summary>
		public int Volume { get; set; }
		/// <summary>
		/// The stream the volume was reported for.
		/// </summary>
		public Stream PlaybackStream { get; set; }
	}
}