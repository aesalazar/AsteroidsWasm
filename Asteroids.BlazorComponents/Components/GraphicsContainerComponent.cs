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
        /// Provides a unique HTML id for the main canvas element.
        /// </summary>
        [Parameter]
        protected string CanvasId { get; set; } = 
            nameof(Asteroids) 
            + nameof(BlazorComponents)
            + nameof(GraphicsContainerComponent);

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
            _controller = new GameController(this, actionSound => 
                _interopSounds.Play(actionSound.ToString().ToLowerInvariant())
            );

            _controller.Initialize(new Rectangle(0, 0, CanvasWidth, CanvasHeight));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Loads the sound streams in JavaScript.
        /// </summary>
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

        #endregion

        #region Implementation of IGraphicContainer

        /// <summary>
        /// Wires the key press handlers and initializes the <see cref="InteropCanvas"/>.
        /// </summary>
        /// <param name="controller">Calling <see cref="GameController"/>.</param>
        /// <param name="rectangle">Required <see cref="Rectangle"/> size.</param>
        public void Initialize(GameController controller, Rectangle rectangle)
        {
            InteropKeyPress.KeyUp += OnKeyUp;
            InteropKeyPress.KeyDown += OnKeyDown;

            _interopCanvas = new InteropCanvas();
            SetDimensions(rectangle);

            Task.Factory.StartNew(() => _interopCanvas.Initialize(CanvasId));
        }

        /// <summary>
        /// Sets the height and width of the parent canvas.
        /// </summary>
        public void SetDimensions(Rectangle rectangle)
        {
            CanvasWidth = rectangle.Width;
            CanvasHeight = rectangle.Height;
        }

        /// <summary>
        /// Clears the <see cref="InteropCanvas"/> and calls the <see cref="GameController"/>
        /// to redraw the screen.
        /// </summary>
        public void Activate()
        {
            _interopCanvas.Clear(() => _controller.Repaint(this));
        }

        /// <summary>
        /// Send message to the <see cref="InteropCanvas"/> to draw a line.
        /// </summary>
        /// <param name="colorHex">HTML color hex, e.g. #000000</param>
        /// <param name="point1">Starting vertex point.</param>
        /// <param name="point2">Ending vertex point.</param>
        public void DrawLine(string colorHex, Point point1, Point point2)
        {
            _interopCanvas.DrawLine(colorHex, point1, point2);
        }

        /// <summary>
        /// Send message to the <see cref="InteropCanvas"/> to draw a polygon.
        /// </summary>
        /// <param name="colorHex">HTML color hex, e.g. #000000</param>
        /// <param name="points">Collection of vertex points.</param>
        public void DrawPolygon(string colorHex, IEnumerable<Point> points)
        {
            _interopCanvas.DrawPolygon(colorHex, points);
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
