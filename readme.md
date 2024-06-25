# Asteroids in .NET Framework, .NET 8, and Blazor WebAssembly

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

- Asteroids.WinForms - Reconstructed WinForms GUI that uses the game engine with a [PictureBox](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.picturebox) as the main renderer.

- Asteroids.Wpf - Equivalent WPF GUI to the WinForms applications that uses a WPF [WriteableBitmap](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap) as the main renderer with help from the [WritableBitmapEx](https://github.com/teichgraf/WriteableBitmapEx/) library.

- Asteroids.Xamarin - The core Xamarin application that uses SkiaSharp for 2D rendering via a [SKCanvasView](https://docs.microsoft.com/en-us/dotnet/api/skiasharp.views.forms.skcanvasview).

- Asteroids.Xamarin.Android - Android GUI that uses the core Xamarin library.

- Asteroid.Xamarin.UWP - UWP GUI that uses the core Xamarin library.

- Asteroids.Blazor.Wasm - WebAssembly project that uses Microsoft's [Blazor Client](https://dotnet.microsoft.com/apps/aspnet/web-apps/client) to allow cross-compiling the C# code to WASM so it can be rendered in a browser (see below for more info).

- Asteroids.Blazor.Server - Similar to the Wasm project but instead uses Microsoft's [Blazor Server](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.1#blazor-server) to execute the application server-side (see below for more info).

- Asteroids.Blazor.Electron - Similar to the above Blazor Server project but running inside [Electron](https://www.electronjs.org/) to execute the code as a Desktop application.

- Asteroids.Blazor.Maui - Similar to the Blazor WASM project but instead uses [Blazor Hybrid MAUI](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/?view=aspnetcore-7.0) to host the same game engine as a desktop or mobile app (see below for more info).

- Asteroids.BlazorComponents - Blazor Class Library that contains the actual game engine instantiated object and associated HTML and JavaScript bridge to allow rendering in the browser.

## General Notes

All applications are written in Visual Studio and can be launch simply by doing `Debug -> Start New Instance`.  All are fully functional in terms of sound and keyboard support.  

Note that all projects in this repository were updated using Visual Studio 2022 but should be editable in the latest Visual Studio Code; otherwise it can be done via Command Line.

Performance is getting fairly close between the different platforms as they continue to improve.  For desktop, MAUI is smooth for game place and sounds play well. But an intresting note is that the Desktop Electron app also plays quite well.

One really interesting comparison to make is the differene between .net 8 and .net framewwork 4.8 when running the WinForms and WPF applications.  The performance difference is very dramatic with the framework versions being almost unplayable.

## .NET 8 Notes

All .NET 8 applications including Blazor are tested on version `8.0.204` (runtime `8.0.4`) of the SDK so remember to have it installed. You can check what versions are installed (you can have multiple) by entering in a command prompt:

`dotnet --info` or `dotnet --version`

You can install the latest via `Winget` using:

`winget install dotnet-sdk-8`

To run all projects in this solution requires the installation of Visual Studio 2022 or the latest Visual Studio Code.

## Xamarin Notes

The Android application will need some additional configuration like any other Xamarin project, e.g. I test in an Android `14.0` VM running on my dev machine.

There is no Xamarin iOS app at this point only because Apple does not allow development on non-macs (which is what I am on) without connected hardware.  But there is no technical reason for it not to be possible.

The UWP application is set to require the Windows 10 Fall Creators Update at a minimum.  This is necessary to run the latest .NET 8 and Standard versions.

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

Once done, it can be ran like any other project from within Visual Studio.

NOTE: Electron.NET requires node.js and npm so make sure to have them installed.  If you do not just grab the latest LTS:

https://nodejs.org/en/

NOTE 2:  Sometimes I get an "is being used by another process" error when attempting to start from Visual Studio.  I have not been able to narrow it down but it seems to be a known issue with the package.  You can easily tell by trying to delete the obj or bin folders and getting file lock errors.  If it happens, look in Task Manager for any running instances of `electron.exe` .  Kill them and it should resolve.

## Blazor MAUI Notes

This is the most recent addition to the collection.  It was created using the Visual Studio Wizard for a Blazor MAUI project.  The Weather demo components were removed along with a general cleanup.  Then a reference to the Astorids.BlazorComponents project was made and added to the main layout.  It is very similar to how the Asterdoids.Blazor.Wasm project works.

I left all of the Platform resources as is (Windows, Android, iOS, etc.).  I have only tested Windows and Android, both of which started up flawlessly.  Android used the same simiulator I had created for the Asteroids.Xamarin.Andoird project.  I didnt have an Apple developer subscription available but I image it will work with little effort.

If you get an error when trying to start the MAUI Android app, you may need to install `WASI` workload:

```
dotnet workload install wasi-experimental
```