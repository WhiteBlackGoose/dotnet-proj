open CommandLine
open System.IO

// For more information see https://aka.ms/fsharp-console-apps
type Options = {
    [<CommandLine.Option('o', "object", Required = false, HelpText = "Path to Directory.Build.* or csproj etc.")>]
    project : string;

    [<CommandLine.Option('c', "create", Default = false, Required = false, HelpText = "Create a file with the given name")>]
    create : bool;

    [<CommandLine.Option('p', "property", Required = false, HelpText = "Path to property")>]
    proeprtyValue : string;
}
    

let getActualProject project =
    // TODO: make auto-detect
    project

let createDirectoryBuild file =
    let content = "<Project>\n\n</Project>"
    File.WriteAllText(file, content)
    let fullFile = Path.GetFullPath file
    printfn $"Written to {fullFile}"

CommandLine.Parser.Default.ParseArguments<Options>(System.Environment.GetCommandLineArgs())
    .WithParsed<Options>(fun o ->
        let prj = getActualProject o.project

        if o.create then
            createDirectoryBuild prj

    ) |> ignore

