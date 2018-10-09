using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Asteroids.BlazorComponents.Classes;
using Asteroids.BlazorComponents.JsInterop;
using Asteroids.Standard;
using Asteroids.Standard.Enums;
using Asteroids.Standard.Interfaces;
using Asteroids.Standard.Sounds;
using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;

namespace Asteroids.BlazorComponents.Components
{
    public class GraphicsContainerComponent : BlazorComponent, IGraphicContainer
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
        protected int ElementHeight { get; set; }

        /// <summary>
        /// Available height in the current window for the main container.
        /// </summary>
        [Parameter]
        protected int ElementWidth { get; set; }

        /// <summary>
        /// Proxy to JavaScript SessionStorage collection.
        /// </summary>
        [Inject]
        protected SessionStorage sessionStorage { get; set; }

        #endregion

        #region Constructor and Fields

        /// <summary>
        /// Main game controller providing all business logic.
        /// </summary>
        private readonly IGameController _controller;

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

            InteropWindow.Initialized += InteropWindow_Loaded;
            InteropWindow.SizeChanged += InteropWindow_SizeChanged;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Loads the sound streams in JavaScript.
        /// </summary>
        /// <remarks>
        ///  Base <see cref="BlazorComponent.OnInitAsync"/> returns null so do not call.
        /// </remarks>
        protected override async Task OnInitAsync()
        {
            //First load the stream to storage
            await LoadSoundStreams();

            //Load the sounds in JavaScript
            _interopSounds = new InteropSounds();
            var sounds = Enum
                .GetNames(typeof(ActionSound))
                .Select(s => s.ToLowerInvariant());

            await _interopSounds.LoadSounds(sounds);
        }

        /// <summary>
        ///  Initializes the <see cref="InteropWindow"/>.
        /// </summary>
        /// <remarks>
        ///  Base <see cref="BlazorComponent.OnAfterRenderAsync"/> returns null so do not call.
        /// </remarks>
        protected override async Task OnAfterRenderAsync()
        {
            //This can be called more then once
            if (_interopWindow != null)
                return;

            _interopWindow = new InteropWindow();
            await _interopWindow.Initialize();
        }

        #endregion

        #region Implementation of IGraphicContainer

        /// <summary>
        /// Wires the key press handlers.
        /// </summary>
        public Task Initialize()
        {
            InteropKeyPress.KeyUp += OnKeyUp;
            InteropKeyPress.KeyDown += OnKeyDown;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Paint or repaint the Window with the collections of lines and polygons (unfilled).
        /// </summary>
        /// <param name="lines">Collection of <see cref="IGraphicLine"/>.</param>
        /// <param name="polygons">Collection of <see cref="IGraphicPolygon"/>.</param>
        public Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            ChildSvgContainer.Draw(lines, polygons);
            return Task.CompletedTask;
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
        /// Loads sound <see cref="System.IO.Stream"/>s stored in <see cref="ActionSounds.SoundDictionary"/>
        /// to HTML Session Storage via <see cref="sessionStorage"/>.
        /// </summary>
        private async Task LoadSoundStreams()
        {
            foreach (var kvp in ActionSounds.SoundDictionary)
            {
                await sessionStorage.SetItem(
                    kvp.Key.ToString().ToLower()
                    , kvp.Value.ToBase64()
                );
            }
        }

        /// <summary>
        /// Handles playing of <see cref="ActionSound"/>s.
        /// </summary>
        private async void OnSoundPlayed(object sender, ActionSound sound)
        {
            await _interopSounds.Play(sound.ToString().ToLowerInvariant());
        }

        #endregion
    }
}
