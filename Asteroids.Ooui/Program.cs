using Asteroids.Ooui.Classes;
using Asteroids.Standard;
using Ooui;

namespace Asteroids.Ooui
{
    class Program
    {
        private const int radius = 10;

        private static GraphicsContainer _container;
        private static GameController _gameController;

        static void Main(string[] args)
        {
            // Create the UI            
            _container = new GraphicsContainer();
            _container.Style.BackgroundColor = new Color(0, 0, 0, 255);

            _gameController = new GameController(_container);

            Div d = new Div();
            d.AppendChild(_container);

            UI.Port = 8082;
            UI.Publish("/", d);

            _gameController.Initialize(new System.Drawing.Rectangle(0, 0, 600, 600));

            //_container.Click += (s, e) =>
            //{
            //    var pt1 = new System.Drawing.Point((int)e.OffsetX, (int)e.OffsetY);
            //    var pt2 = new System.Drawing.Point((int)e.OffsetX + radius, (int)e.OffsetY + radius);
            //    _container.DrawLine("#FF0000", pt1, pt2);
            //};

            //_container.DoubleClick += (s, e) =>
            //{
            //    Console.WriteLine(s);
            //    Console.WriteLine(e);

            //    const int off = radius / 2;
            //    var x = (int)e.OffsetX;
            //    var y = (int)e.OffsetY;

            //    _container.DrawPolygon(
            //        "#FFFF00"
            //        , new List<Point>
            //        {
            //            new Point(x - off, y - off ),
            //            new Point(x + off, y - off ),
            //            new Point(x + off, y + off ),
            //            new Point(x - off, y + off ),
            //            new Point(x - off, y - off ),
            //        }
            //    );
            //};

        }

        //private static void DrawLine(double x1, double y1, double x2, double y2)
        //{
        //    context.LineWidth = 5;
        //    context.StrokeStyle = "#FF0000"; //Red
        //    context.FillStyle = "#00FFFF"; //Cyne

        //    context.BeginPath();
        //    context.LineTo(x1, y1);
        //    context.LineTo(x2, y2);
        //    context.Stroke();
        //    context.ClosePath();
        //    context.Fill();
        //}

        //private static void DrawX(double x, double y)
        //{
        //    const int off = radius / 2;

        //    context.LineWidth = 2;
        //    context.StrokeStyle = "#FF0000"; //Red
        //    context.FillStyle = "#00FFFF"; //Cyan

        //    context.BeginPath();
        //    context.LineTo(x - off, y - off);
        //    context.LineTo(x + off, y + off);

        //    context.MoveTo(x + off, y - off);
        //    context.LineTo(x - off, y + off);

        //    context.Stroke();
        //    context.ClosePath();
        //    context.Fill();
        //}

        //private static void DrawCircle(double x, double y)
        //{
        //    context.LineWidth = 5;
        //    context.StrokeStyle = "#00FF00"; //Lime
        //    context.FillStyle = "#008000"; //Green

        //    context.BeginPath();
        //    context.Arc(x, y, radius, 0, 2 * Math.PI, true);
        //    context.ClosePath();
        //    context.Fill();
        //}

        //private static void DrawRect(double x, double y)
        //{
        //    context.LineWidth = 2;
        //    context.StrokeStyle = "#FFFF00"; //Yellow
        //    context.FillStyle = "#800000"; //Maroon

        //    context.BeginPath();
        //    context.StrokeRect(x - radius, y - radius, 2 * radius, 2 * radius);
        //    context.ClosePath();
        //    context.Fill();
        //}

    }
}
