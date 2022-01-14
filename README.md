## dotnet-proj

CLI tool to work with Directory.Build.props/csproj/fsproj etc.

### Install

```
dotnet tool install dotnet-proj
```
or
```
dotnet tool install dotnet-proj --global
```

### Create

```
dotnet proj --create --object=./Directory.Build.props
```

### Add property

```
dotnet proj --object=./Directory.Build.props --property=TargetFramework --value=net5.0
```
