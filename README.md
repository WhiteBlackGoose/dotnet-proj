## dotnet-proj

CLI tool to work with Directory.Build.props/csproj/fsproj etc.

### Install

```
dotnet tool install dotnet-proj-cli
```
or
```
dotnet tool install dotnet-proj-cli --global
```
(not to confuse with another tool, `dotnet-proj`!)

### Create

```
dotnet proj create Directory.Build.props
```

### Add property

```
dotnet proj add --object=./Directory.Build.props --property=TargetFramework --value=net5.0
```
