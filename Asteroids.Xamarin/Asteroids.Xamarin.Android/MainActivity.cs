using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Asteroids.Standard.Enums;
using GameMode = Asteroids.Standard.Enums.GameMode;

namespace Asteroids.Xamarin.Droid
{
    [Activity(Label = "Asteroids.Xamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public sealed class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private global::Xamarin.Forms.Application _app;
        private MainPage _page;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            _app = new App();
            LoadApplication(_app);
            _page = (MainPage)_app.MainPage;
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            PlayKey? key = null;

            switch (keyCode)
            {
                case Keycode.Escape:
                case Keycode.Q:
                    // Escape during a title screen exits the game
                    if (_page.GameMode == GameMode.Title)
                    {
                        System.Environment.Exit(0);   
                    }
                    else
                    {
                        key = PlayKey.Escape;
                    }

                    break;

                case Keycode.DpadLeft:
                    key = PlayKey.Left;
                    break;

                case Keycode.DpadRight:
                    key = PlayKey.Right;
                    break;

                case Keycode.DpadUp:
                    key = PlayKey.Up;
                    break;

                case Keycode.DpadDown:
                    key = PlayKey.Down;
                    break;

                case Keycode.Space:
                    key = PlayKey.Space;
                    break;

                case Keycode.P:
                    key = PlayKey.P;
                    break;
            }

            if (key.HasValue)
                _page.KeyDown(key.Value);

            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            PlayKey? key = null;

            switch (keyCode)
            {
                case Keycode.Escape:
                case Keycode.Q:
                    key = PlayKey.Escape;
                    break;

                case Keycode.DpadLeft:
                    key = PlayKey.Left;
                    break;

                case Keycode.DpadRight:
                    key = PlayKey.Right;
                    break;

                case Keycode.DpadUp:
                    key = PlayKey.Up;
                    break;

                case Keycode.DpadDown:
                    key = PlayKey.Down;
                    break;

                case Keycode.Space:
                    key = PlayKey.Space;
                    break;

                case Keycode.P:
                    key = PlayKey.P;
                    break;
            }

            if (key.HasValue)
                _page.KeyUp(key.Value);

            return base.OnKeyUp(keyCode, e);
        }
    }
}