# Asteroids in .NET Framework, .NET 5, and Blazor WebAssembly

## Live Demo: https://aesalazar.github.io/AsteroidsWasm/

This project is a POC to determine the plausibility of writing a .NET Standard library and using it across devices INCLUDING WebAssembly (WASM).  The root library, Asteroids.Standard, encapsulates all logic for rendering the classic '80s video game.  None of this is meant to be production-worthy.  It is more me just messing around trying to see what works.

<div style="text-align: center;">
    <a href="Documents/Screeny.gif" target="_blank">
        <img src="Documents/Screeny.gif" alt="Screen Shot" >
    </a>  
</div>

The original code was adopted from a very cool WinForms project on CodePlex by Howard Uman, circa 2004:

https://www.codeproject.com/articles/7428/c-asteroids

Which now resides here:

https://github.com/unhuman/csharp-asteroids

It was chosen because it was already in C# and very straight forward in terms of inheritance and logic.  Separating the logic from the UI layer was relatively simple.

Currently, the project is made of the following:

- Asteroids.Standard - .Net Standard Library containing the game engine.

- Asteroids.WinForms - Reconstructed WinForms GUI that uses the game engine with a [PictureBox](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.picturebox) as the main renderer.  This is using the .NET Framework 4.8.

- Asteroids.WinForms.Core - Identical in code to the Asteroids.WinForms project but using .NET 5 (see below for more info).

- Asteroids.Wpf - Equivalent WPF GUI to the WinForms applications that uses a WPF [WriteableBitmap](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap) as the main renderer with help from the [WritableBitmapEx](https://github.com/teichgraf/WriteableBitmapEx/) library.  This is using the .NET Framework 4.8.

- Asteroids.Wpf.Core - Identical in code to the Asteroids.Wpf project but using .NET 5 (see below for more info).

- Asteroids.Xamarin - The core Xamarin application that uses SkiaSharp for 2D rendering via a [SKCanvasView](https://docs.microsoft.com/en-us/dotnet/api/skiasharp.views.forms.skcanvasview).

- Asteroids.Xamarin.Android - Android GUI that uses the core Xamarin library.

- Asteroid.Xamarin.UWP - UWP GUI that uses the core Xamarin library.

- Asteroids.Blazor.Wasm - WebAssembly project that uses Microsoft's [Blazor Client](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) to allow cross-compiling the C# code to WASM so it can be rendered in a browser (see below for more info).

- Asteroids.Blazor.Server - Similar to the Wasm project but instead uses Microsoft's [Blazor Server](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.1#blazor-server) to execute the application server-side (see below for more info).

- Asteroids.Blazor.Electron - Similar to the above Blazor Server project but running inside [Electron](https://www.electronjs.org/) to execute the code as a Desktop application.

- Asteroids.BlazorComponents - Blazor Class Library that contains the actual game engine instantiated object and associated HTML and JavaScript bridge to allow rendering in the browser.

## General Notes

All applications are written in Visual Studio and can be launch simply by doing `Debug -> Start New Instance`.  All are fully functional in terms of sound and keyboard support.  

Note that the Blazor, WinForms .NET 5 and Wpf .NET 5 projects require Visual Studio 2019 or the latest Visual Studio Code to edit and compile; otherwise it can be done via Command Line.

Performance varies among the technologies with WinForms in .NET 5 being the clear winner for desktop and Firefox for Blazor/Web.  Wpf in .NET 5 is a close second for desktop, however, the UWP app is also quite fast and has better sound support in that more then one can play at a time, out of the box.

## .NET 5 Notes

All .NET 5 applications including Blazor are tested on version `5.0.100` of the SDK so remember to have it installed. You can check what versions are installed (you can have multiple) by entering in a command prompt:

`dotnet --info` or `dotnet --version`

To run all projects in this solution requires the installation of Visual Studio `16.8` minimum or the latest Visual Studio Code.

## Xamarin Notes

The Android application will need some additional configuration like any other Xamarin project, e.g. I test in an Android `9.0` (Pie) VM running on my dev machine.

There is no Xamarin iOS app at this point only because Apple does not allow development on non-macs (which is what I am on) without connected hardware.  But there is no technical reason for it not to be possible.

The UWP application is set to require the Windows 10 Fall Creators Update at a minimum.  This is necessary to run the latest .NET 5 and Standard versions.

## Blazor Notes

Microsoft has made Blazor officially part of .NET Core.  It was first included in `3.0 Preview 4`.  Prior to that it was a separate library/install.

The difference between the Wasm and Server projects is the hosting model.  With Blazor, you have the option to either fully host the code and application Client-Side via WebAssembly or Server-Side with updates pushed to the Client via [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1).  In a production application, the choice would be highly dependent on the situation and more information can be found on Microsoft's [Hosting Models](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.1) Page.

To build the app, simply do it from Visual Studio - just make sure you have all dependencies listed on their Getting Stated page at [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/client).

Building from CLI in the `Asteroids.Blazor.Wasm` project folder is also an option:

```
dotnet build -c Release
```

To run the application, simply hit `F5` or `ctrl+F5` in Visual Studio or from the CLI:

```
dotnet run
```

The app can be published with:

```
dotnet publish -c Release
```

## Blazor Electron Notes

The Blazor Electron project is probably the most experimental of all in this repo.  It uses the [Electron.NET](https://github.com/ElectronNET/Electron.NET) wrapper in conjunction with a Blazor Server app to show a the game in a Desktop application.  

It requires the **global** installation of the ElectronNet.CLI before it can be ran:

```
dotnet tool install ElectronNET.CLI -g
```

Once done, it can be ran like any other project from within Visual Studio.  Note that breakpoints do not work with this project but `/watch` option is enabled with the CLI so changes made to the source code will trigger an automatic refresh when running (i.e. hot reload).