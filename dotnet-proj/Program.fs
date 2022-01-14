open CommandLine
open ProjUtils

// For more information see https://aka.ms/fsharp-console-apps
type Options = {
    [<CommandLine.Option('o', "object", Required = false, HelpText = "Path to Directory.Build.* or csproj etc.")>]
    project : string;

    [<CommandLine.Option('c', "create", Default = false, Required = false, HelpText = "Create a file with the given name")>]
    create : bool;

    [<CommandLine.Option('p', "property", Required = false, HelpText = "Path to property")>]
    property : string;

    [<CommandLine.Option('v', "value", Default = "", Required = false, HelpText = "Path to property")>]
    propertyValue : string;
}
    

let getActualProject project =
    // TODO: make auto-detect
    project


CommandLine.Parser.Default.ParseArguments<Options>(System.Environment.GetCommandLineArgs())
    .WithParsed<Options>(fun o ->
        let prj = getActualProject o.project

        if o.create then
            createDirectoryBuild prj
        else
            addProperty prj o.property o.propertyValue

    ) |> ignore

