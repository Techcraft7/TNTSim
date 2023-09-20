# TNTSim
A Simulator/Calculator for m^3's Orbital Strike Cannon 2.0

## Building and running

> Requires .NET SDK 7+

### Release mode
```
dotnet publish -c Release --sc --ucr
```
This create a build for your machine.
If you want to specify an output folder, add `-o <folder name>` to the above command.
If you don't, it will go into the `bin/Release/<.NET version>/<os>-<arch>/publish` folder.

> You can ignore the `.pdb`, `.dbg`, or `.dSYM` files. They are just symbol files that are not needed. You can delete them.

### Debug mode and Testing
```
dotnet build
```
Or, to run the project:
```
dotnet run
```
