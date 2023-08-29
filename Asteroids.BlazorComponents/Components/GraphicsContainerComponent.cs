using Asteroids.BlazorComponents.JsInterop;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Asteroids.BlazorComponents.Components
{
    /// <summary>
    /// Implementation of <see cref="IGraphicContainer"/> to provide rendering of 
    /// vectors and audio to the <see cref="SvgContentContainer"/>.
    /// </summary>
    public abstract class GraphicsContainerComponent : ComponentBase, IGraphicContainer, IDisposable
    {
        #region Blazor Parameters

        /// <summary>
        /// Child <see cref="SvgContentContainer"/> to draw into.
        /// </summary>
        protected SvgContentContainer ChildSvgContainer;

        /// <summary>
        /// Available width in the current window for the main container.
        /// </summary>
        [Parameter]
        public int ElementHeight { get; set; }

        /// <summary>
        /// Available height in the current window for the main container.
        /// </summary>
        [Parameter]
        public int ElementWidth { get; set; }

        /// <summary>
        /// JavaScript runtime bridge to provide to proxies.
        /// </summary>
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        
        #endregion

        #region Constructor and Fields

        /// <summary>
        /// Main game controller providing all business logic.
        /// </summary>
        private readonly IGameController _controller;

        /// <summary>
        /// Interop handler between .NET and JavaScript runtime for key press events.
        /// </summary>
        private InteropKeyPress _interopKeyPress;

        /// <summary>
        /// Proxy to JavaScript command to draw on the main Window.
        /// </summary>
        private InteropWindow _interopWindow;

        /// <summary>
        /// Proxy to JavaScript sound collection.
        /// </summary>
        private InteropSounds _interopSounds;

        /// <summary>
        /// Creates new instance of <see cref="GraphicsContainerComponent"/>.
        /// </summary>
        public GraphicsContainerComponent()
        {
            _controller = new GameController();
            _controller.SoundPlayed += OnSoundPlayed;
        }

        #endregion

        #region Overrides

        /// <summary>
        ///  Initializes the <see cref="InteropWindow"/>.
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            //This can be called more then once
            if (!firstRender)
                return;

            //Load the sound interop in JavaScript
            _interopSounds = new InteropSounds(JsRuntime);

            if (!await _interopSounds.Initialize(_controller.ActionSounds))
                Console.WriteLine($"ERROR '{nameof(InteropSounds)}': Could not initialize sounds in JavaScript.");

            //Load the window interop in JavaScript
            _interopWindow = new InteropWindow(JsRuntime);
            _interopWindow.Initialized += InteropWindow_Loaded;
            _interopWindow.SizeChanged += InteropWindow_SizeChanged;
            await _interopWindow.Initialize();

            //Force a refresh
            await InvokeAsync(StateHasChanged);
        }

        #endregion

        #region Implementation of IGraphicContainer

        /// <summary>
        /// Wires the key press handlers.
        /// </summary>
        /// <param name="drawColorMap">Collection (read-only) of <see cref="DrawColor"/> used by the game engine and associated HEX-based (HTML) color strings.</param>
        public async Task Initialize(IDictionary<DrawColor, string> drawColorMap)
        {
            await InvokeAsync(() => ChildSvgContainer.Initialize(drawColorMap));

            _interopKeyPress = new InteropKeyPress(JsRuntime);
            await _interopKeyPress.Initialize();
            _interopKeyPress.KeyUp += OnKeyUp;
            _interopKeyPress.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// Paint or repaint the Window with the collections of lines and polygons (unfilled).
        /// </summary>
        /// <param name="lines">Collection of <see cref="IGraphicLine"/>.</param>
        /// <param name="polygons">Collection of <see cref="IGraphicPolygon"/>.</param>
        public Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            InvokeAsync(() => ChildSvgContainer.Draw(lines, polygons));
            return Task.CompletedTask;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _controller.SoundPlayed -= OnSoundPlayed;
        
            if (_interopKeyPress != null )
            {
                _interopKeyPress.KeyUp -= OnKeyUp;
                _interopKeyPress.KeyDown -= OnKeyDown;
                _interopKeyPress.Dispose();
            }

            if (_interopWindow  != null )
            {
                _interopWindow.Initialized -= InteropWindow_Loaded;
                _interopWindow.SizeChanged -= InteropWindow_SizeChanged;
                _interopWindow.Dispose();
            }
        }

        #endregion

        #region JavaScript Window Handlers

        /// <summary>
        /// Initializes the <see cref="IGameController"/>.
        /// </summary>
        private async void InteropWindow_Loaded(object sender, Rectangle e)
        {
            ElementWidth = e.Width;
            ElementHeight = e.Height;

            await _controller.Initialize(this, e);
        }

        /// <summary>
        /// Resizes the <see cref="IGameController"/>.
        /// </summary>
        private void InteropWindow_SizeChanged(object sender, Rectangle e)
        {
            ElementWidth = e.Width;
            ElementHeight = e.Height;

            _controller.ResizeGame(e);
            InvokeAsync(StateHasChanged);
        }

        #endregion

        #region Key press handlers and Sounds

        /// <summary>
        /// Sends the equivalent <see cref="PlayKey"/> from a Key Down event to the <see cref="IGameController"/>.
        /// </summary>
        private void OnKeyDown(object sender, ConsoleKey e)
        {
            PlayKey key;
            switch (e)
            {
                case ConsoleKey.Escape:
                    //Do not send exit command
                    if (_controller.GameStatus != GameMode.Game)
                        return;

                    key = PlayKey.Escape;
                    break;

                case ConsoleKey.LeftArrow:
                    key = PlayKey.Left;
                    break;

                case ConsoleKey.RightArrow:
                    key = PlayKey.Right;
                    break;

                case ConsoleKey.UpArrow:
                    key = PlayKey.Up;
                    break;

                case ConsoleKey.DownArrow:
                    key = PlayKey.Down;
                    break;

                case ConsoleKey.Spacebar:
                    key = PlayKey.Space;
                    break;

                case ConsoleKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyDown(key);

        }

        /// <summary>
        /// Sends the equivalent <see cref="PlayKey"/> from a Key Up event to the <see cref="IGameController"/>.
        /// </summary>
        private void OnKeyUp(object sender, ConsoleKey e)
        {
            PlayKey key;
            switch (e)
            {
                case ConsoleKey.Escape:
                    //Do not send exit command
                    if (_controller.GameStatus != GameMode.Game)
                        return;

                    key = PlayKey.Escape;
                    break;

                case ConsoleKey.LeftArrow:
                    key = PlayKey.Left;
                    break;

                case ConsoleKey.RightArrow:
                    key = PlayKey.Right;
                    break;

                case ConsoleKey.UpArrow:
                    key = PlayKey.Up;
                    break;

                case ConsoleKey.DownArrow:
                    key = PlayKey.Down;
                    break;

                case ConsoleKey.Spacebar:
                    key = PlayKey.Space;
                    break;

                case ConsoleKey.P:
                    key = PlayKey.P;
                    break;

                default:
                    return;
            }

            _controller.KeyUp(key);
        }

        /// <summary>
        /// Handles playing of <see cref="ActionSound"/>s.
        /// </summary>
        private async void OnSoundPlayed(object sender, ActionSound sound)
        {
            if (!await _interopSounds.Play(sound))
                Console.WriteLine($"ERROR '{nameof(InteropSounds)}':Could not play sound: {sound}");
        }

        #endregion
    }
}
