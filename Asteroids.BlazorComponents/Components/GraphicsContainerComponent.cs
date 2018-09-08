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
        /// Provides the HTML width to the main canvas element.
        /// </summary>
        [Parameter]
        protected int CanvasWidth { get; set; } = 650;

        /// <summary>
        /// Provides the HTML height to the main canvas element.
        /// </summary>
        [Parameter]
        protected int CanvasHeight { get; set; } = 500;

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
        private readonly GameController _controller;

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
            _controller = new GameController(this, async actionSound =>
                await _interopSounds.Play(actionSound.ToString().ToLowerInvariant())
            );

            Task.Factory.StartNew(async () =>
                await _controller.Initialize(new Rectangle(0, 0, CanvasWidth, CanvasHeight))
            );
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
        /// <param name="controller">Calling <see cref="GameController"/>.</param>
        /// <param name="rectangle">Required <see cref="Rectangle"/> size.</param>
        public async Task Initialize(Rectangle rectangle)
        {
            InteropKeyPress.KeyUp += OnKeyUp;
            InteropKeyPress.KeyDown += OnKeyDown;

            await SetDimensions(rectangle);
        }

        /// <summary>
        /// Sets the height and width of the parent canvas.
        /// </summary>
        public Task SetDimensions(Rectangle rectangle)
        {
            CanvasWidth = rectangle.Width;
            CanvasHeight = rectangle.Height;
            return Task.CompletedTask;
        }

        public async Task Draw(IEnumerable<IGraphicLine> lines, IEnumerable<IGraphicPolygon> polygons)
        {
            await _interopCanvas.Clear();
            await _interopCanvas.DrawLines(lines);
            await _interopCanvas.DrawPolygons(polygons);
            await _interopCanvas.Paint();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the equivalent <see cref="PlayKey"/> from a Key Down event to the <see cref="GameController"/>.
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
        /// Sends the equivalent <see cref="PlayKey"/> from a Key Up event to the <see cref="GameController"/>.
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
        /// Loads sound <see cref="Stream"/>s stored in <see cref="ActionSounds.SoundDictionary"/>
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

        #endregion

    }
}
