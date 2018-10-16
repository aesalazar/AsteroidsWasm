using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Asteroids.Xamarin.Classes
{
    public class CustomEntry : Entry
    {
        public Action DonePressed = delegate { };

    }
}
