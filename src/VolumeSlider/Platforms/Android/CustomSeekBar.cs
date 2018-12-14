using Android.Content;
using Android.Views;
using Android.Widget;

namespace VolumeSliderPlugin.Droid
{
	/// <summary>
	/// Custom SeekBar class. This code is identical to the one found inside the Xamarin Forms Github repo.
	/// </summary>
    class CustomSeekBar : SeekBar
    {
        public CustomSeekBar(Context context) : base(context)
        {
            //this should work, but it doesn't.
            DuplicateParentStateEnabled = false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    isTouching = true;
                    break;
                case MotionEventActions.Up:
                    Pressed = false;
                    break;
            }

            return base.OnTouchEvent(e);
        }

        public override bool Pressed
        {
            get
            {
                return base.Pressed;
            }
            set
            {
                if (isTouching)
                {
                    base.Pressed = value;
                    isTouching = value;
                }

            }
        }

        bool isTouching = false;
    }
}