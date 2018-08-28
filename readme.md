This project is a POC to determine the plausibility of writing a .NET Standard library and using it across devices INCLUDING WebAssembly (WASM).  The root library, Asteroids.Standard, encapsulates all logic for rendering the classic '80s video game.  None of this is meant to be production-worthy.  It is more me just messing around trying to see what works.

The original code was adopted from a very cool WinForms project on  CodePlex by Howard Uman, circa 2004:

https://www.codeproject.com/articles/7428/c-asteroids

It was chosen because it was already in C# and very straight forward in terms of inheritance and logic.  Separating the logic from the UI layer was relatively simply.

Currently, the project is made of the following:

- Asteroids.Standard - .Net Standard Library containing the game engine.

- Asteroids.WinForms - Reconstructed WinForms GUI that uses the game engine that mostly follows the same technique used by Uman via two [PictureBoxes](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.picturebox) as the renders.

- Asteroids.Wpf - Equivalent WPF GUI to the WinForms applications that uses a WPF [Canvas](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.canvas) as the main render.

- Asteroids.Xamarin - The core Xamarin application that uses SkiaSharp for 2D rendering via a [SKCanvasView](https://docs.microsoft.com/en-us/dotnet/api/skiasharp.views.forms.skcanvasview).

- Asteroids.Xamarin.Android - Android GUI that uses the core Xamarin library.

- Asteroid.Xamarin.UWP - UWP GUI that uses the core Xamarin library.

- Asteroids.Ooui - This is a WASM project that uses the very cool [OOUI](https://github.com/praeclarum/Ooui) library to allow cross-compiling the C# code to WASM so it can be rendered in a browser (see below for more info).

There is no Xamarin iOS app at this point only because Apple does not allow development on non-macs (which is what I am on) without connected hardware.  But there is no technical reason for it not to be possible.

All applications are written in Visual Studio so they can be launch simply by doing `Debug -> Start New Instance` except Asteroids.Ooui as explained below.   The Android application will need some additional configuration like any other Xamarin project, e.g. I test in an Oreo VM running on my dev machine.

To run the OOUI application, it requires some command line (VS could probably be configured somehow but CLI seems easier).  For this project, run the following in the Asteroids.Ooui folder to build it:

`dotnet build`

You then need to serve it via a web server.  OOUI GitHub page talks about different options but I like to use the [dotnet-serve plugin](https://github.com/natemcmaster/dotnet-serve).  It can be installed with:

`dotnet tool install --global dotnet-serve`

To start the serve and load the WASM app, run this:

`dotnet serve -p 8000 -d bin/Debug/netcoreapp2.1/dist`

Now, in your any WASM-supported browser, go to http://localhost:8000/ and you should see the game load.

The WinForms and WPF apps a fully functioning at this point in terms of keyboard and sound support.  The others are still a WIP since each requires unique configuration.