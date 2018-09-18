# Asteroids WebAssembly

This project is a POC to determine the plausibility of writing a .NET Standard library and using it across devices INCLUDING WebAssembly (WASM).  The root library, Asteroids.Standard, encapsulates all logic for rendering the classic '80s video game.  None of this is meant to be production-worthy.  It is more me just messing around trying to see what works.

<div style="text-align: center;">
    <a href="Documents/Screeny.gif" target="_blank">
        <img src="Documents/Screeny.gif" alt="Screen Shot" >
    </a>  
</div>

The original code was adopted from a very cool WinForms project on  CodePlex by Howard Uman, circa 2004:

https://www.codeproject.com/articles/7428/c-asteroids

It was chosen because it was already in C# and very straight forward in terms of inheritance and logic.  Separating the logic from the UI layer was relatively simply.

Currently, the project is made of the following:

- Asteroids.Standard - .Net Standard Library containing the game engine.

- Asteroids.WinForms - Reconstructed WinForms GUI that uses the game engine with a [PictureBox](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.picturebox) as the main renderer.

- Asteroids.Wpf - Equivalent WPF GUI to the WinForms applications that uses a WPF [Canvas](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.canvas) as the main renderer.

- Asteroids.Xamarin - The core Xamarin application that uses SkiaSharp for 2D rendering via a [SKCanvasView](https://docs.microsoft.com/en-us/dotnet/api/skiasharp.views.forms.skcanvasview).

- Asteroids.Xamarin.Android - Android GUI that uses the core Xamarin library.

- Asteroid.Xamarin.UWP - UWP GUI that uses the core Xamarin library.

- Asteroids.Ooui (broken) - WASM project that uses the very cool [OOUI](https://github.com/praeclarum/Ooui) library to allow cross-compiling the C# code to WASM so it can be rendered in a browser (see below for more info).

- Asteroids.Blazor - WASM project that uses Microsoft's experimental [Blazor](https://github.com/aspnet/blazor) to allow cross-compiling the C# code to WASM so it can be rendered in a browser (see below for more info).

- Asteroids.BlazorComponents - Blazor library that contains the actual game engine instantiated object and associated HTML and JavaScript bridge to allow rendering in the browser.

## General Notes

All applications are written in Visual Studio so they can be launch simply by doing `Debug -> Start New Instance` except Asteroids.Ooui as explained below.   The Android application will need some additional configuration like any other Xamarin project, e.g. I test in an Oreo VM running on my dev machine.

The WinForms, WPF, and Blazor apps are fully functional at this point in terms of keyboard and sound support.  The others are still a WIP since each requires unique configuration.

## Xamarin Notes

There is no Xamarin iOS app at this point only because Apple does not allow development on non-macs (which is what I am on) without connected hardware.  But there is no technical reason for it not to be possible.

The UWP application is set to require the Windows 10 Fall Creators Update at a minimum.  This is necessary to run the latest .NET Core and Standard versions.

## OOUI Notes

Currently, the OOUI project is not building properly.  That is, it builds but logs errors as warnings but they are actually fatal and the project will not run.  This is because of the async-await pattern and the need to reference `Task` types.  Seems there are still some issues with Mono and type mapping they are working on.  Hopefully, that will be in their next release.

To run the OOUI application, when working, it requires some command line (VS could probably be configured somehow but CLI seems easier).  For this project, run the following in the Asteroids.Ooui folder to build it:

`dotnet build`

You then need to serve it via a web server.  OOUI GitHub page talks about different options but I like to use the [dotnet-serve plugin](https://github.com/natemcmaster/dotnet-serve).  It can be installed with:

`dotnet tool install --global dotnet-serve`

To then serve and load the WASM app, run this:

`dotnet serve -p 8000 -d bin/Debug/netcoreapp2.1/dist`

Now, in your any WASM-supported browser, go to http://localhost:8000/ and you should see the game load.

## Blazor Notes

Microsoft has labeled Blazor as "experimental" (as of the time of this writing) and, as such, has allow themselves the luxury to break anything at any time :).  So version control will be important when trying to build and run this app.

To build the app, simply do it from Visual Studio - just make sure you have all dependencies listed on their [GitHub](https://github.com/aspnet/blazor) page. Visual Studio seems to hang occasionally when trying to build.  Usually, canceling and retrying fixes it.  Doing it from CLI in the Asteroids.Blazor project folder seems to be the most reliable:

`dotnet build`

To run the application, simply hit F5 or ctrl+F5 in Visual Studio or from the CLI:

`dotnet run`

If you get an HTTP 502.5 error, chances are the is a mismatch in the installed SDK vs what is listed in the global.json file in Asteroids.Blazor:

```json
"sdk": {
    "version": "2.1.400.1"
  }
```

To find your version run:

`dotnet --version`

and update the json to match.