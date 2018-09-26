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
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;

namespace Asteroids.BlazorComponents.Components
{
    public class GraphicsContainerComponent : BlazorComponent, IGraphicContainer
    {
        #region Blazor Parameters

        /// <summary>
        /// Primary HTML canvas to render the game in.
        /// </summary>
        protected ElementRef CanvasElement;

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
        /// Proxy to JavaScript command to draw on the main canvas.
        /// </summary>
        private InteropCanvas _interopCanvas;

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

            InteropCanvas.SizeChanged += InteropCanvas_SizeChanged;
            InteropCanvas.Initialized += InteropCanvas_Loaded;
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
        ///  Initializes the <see cref="InteropCanvas"/>.
        /// </summary>
        /// <remarks>
        ///  Base <see cref="BlazorComponent.OnAfterRenderAsync"/> returns null so do not call.
        /// </remarks>
        protected override async Task OnAfterRenderAsync()
        {
            //This can be called more then once
            if (_interopCanvas != null)
                return;

            _interopCanvas = new InteropCanvas();
            await _interopCanvas.Initialize(CanvasElement);
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
        /// Paint or repaint the canvas with the collections of lines and polygons (unfilled).
        /// </summary>
        /// <param name="lines">Collection of <see cref="IGraphicLine"/>.</param>
        /// <param name="polygons">Collection of <see cref="IGraphicPolygon"/>.</param>
        public async Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            await _interopCanvas.Clear();
            await _interopCanvas.DrawLines(lines);
            await _interopCanvas.DrawPolygons(polygons);
            await _interopCanvas.Paint();
        }

        #endregion

        #region JavaScript Canvas Handlers

        /// <summary>
        /// Initializes the <see cref="IGameController"/>.
        /// </summary>
        private async void InteropCanvas_Loaded(object sender, Rectangle e)
        {
            await _controller.Initialize(this, e);
        }

        /// <summary>
        /// Resizes the <see cref="IGameController"/>.
        /// </summary>
        private void InteropCanvas_SizeChanged(object sender, Rectangle e)
        {
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
