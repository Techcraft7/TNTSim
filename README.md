# TNTSim
A Simulator/Calculator for m^3's Orbital Strike Cannon 2.0

## Building and running

> Requires .NET SDK 7+

### Release mode
```
dotnet publish -c Release --sc --ucr -p:OptimizationPreference=Speed -p:PublishAot=true -p:PublishTrimmed=true
```
This create a build for your machine.
If you want to specify an output folder, add `-o <folder name>` to the above command.

### Debug mode and Testing
```
dotnet build
```
Or, to run the project:
```
dotnet run
```
