# VLSEdit
VLSEdit is an editor and interpreter for a new visual programming language called Visual Link Scripting (VLS). VLS represents logic operations as 'Boxes' that can be linked together to form complex programs.

- [VLSEdit's design and features](docco/design-report.pdf)
- [How to use VLSEdit](docco/usage-documentation.pdf)

## Building
VLSEdit requires .NET 10.0 and [Splashkit](splashkit.io). Once those have been installed, run `dotnet build` to build VLSEdit, or run `dotnet run edit <file>.vls` or `dotnet run run <file>.vls` to run it from the source.