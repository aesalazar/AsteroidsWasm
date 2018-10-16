using Asteroids.Standard.Enums;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Asteroids.Xamarin.UWP
{
    public sealed partial class MainPage
    {
        private Xamarin.MainPage _page;
        private global::Xamarin.Forms.Application _app;

        public MainPage()
        {
            InitializeComponent();

            _app = new Xamarin.App();
            LoadApplication(_app);

            //Get the controller from the page
            _page = (Xamarin.MainPage)_app.MainPage;

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
        }

        /// <summary>
        /// Proceses incoming keyboard events.
        /// </summary>
        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType == CoreAcceleratorKeyEventType.SystemKeyUp || 
                args.EventType == CoreAcceleratorKeyEventType.KeyUp)
            {
                ProcessKeyUp(args.VirtualKey);
            }
            else if (args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown || 
                     args.EventType == CoreAcceleratorKeyEventType.KeyDown)
            {
                ProcessKeyDown(args.VirtualKey);
            }
        }

        private void ProcessKeyDown(VirtualKey virtualKey)
        {
            PlayKey key;
            switch (virtualKey)
            {
                case VirtualKey.Escape:
                    // Escape during a title screen exits the game
                    if (_page.GameMode == GameMode.Title)
                    {
                        Application.Current.Exit();
                        return;
                    }

                    key = PlayKey.Escape;
                    break;

                case VirtualKey.Left:
                    key = PlayKey.Left;
                    break;

                case VirtualKey.Right:
                    key = PlayKey.Right;
                    break;

                case VirtualKey.Up:
                    key = PlayKey.Up;
                    break;

                case VirtualKey.Down:
                    key = PlayKey.Down;
                    break;

                case VirtualKey.Space:
                    key = PlayKey.Space;
                    break;

                case VirtualKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _page.KeyDown(key);
        }

        private void ProcessKeyUp(VirtualKey virtualKey)
        {
            PlayKey key;
            switch (virtualKey)
            {
                case VirtualKey.Escape:
                    key = PlayKey.Escape;
                    break;

                case VirtualKey.Left:
                    key = PlayKey.Left;
                    break;

                case VirtualKey.Right:
                    key = PlayKey.Right;
                    break;

                case VirtualKey.Up:
                    key = PlayKey.Up;
                    break;

                case VirtualKey.Down:
                    key = PlayKey.Down;
                    break;

                case VirtualKey.Space:
                    key = PlayKey.Space;
                    break;

                case VirtualKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _page.KeyUp(key);
        }
    }
}
