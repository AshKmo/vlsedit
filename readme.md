# VLSEdit
VLSEdit is an editor and interpreter for a new visual programming language called Visual Link Scripting (VLS). VLS represents logic operations as 'Boxes' that can be linked together to form complex programs.

- [How to use VLSEdit](docco/usage-documentation.pdf)

## Running
To run VLSEdit, first [install SplashKit](https://splashkit.io/installation/), then download the [atest release ZIP](https://github.com/AshKmo/vlsedit/releases/). Open up a terminal and navigate to the VLSEdit folder, then run `VLSEdit.exe edit myprogram.vls` to create or edit a program. For instance, run `VLSEdit.exe edit examples/factorial.vls` to edit the factorial calculator program. To run your program, run `VLSEdit.exe run myprogram.vls`.

## Building
VLSEdit requires .NET 10.0 and [SplashKit](https://splashkit.io). Once those have been installed, run `dotnet build` to build VLSEdit, or run `dotnet run edit <file>.vls` or `dotnet run run <file>.vls` to run it from the source.
