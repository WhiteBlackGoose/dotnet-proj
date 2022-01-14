open CommandLine
open ProjUtils

// For more information see https://aka.ms/fsharp-console-apps
[<Verb("create", HelpText = "Create new manifest file")>]
type CreateOptions = {
    [<CommandLine.Option('o', "object", Required = true, HelpText = "Path to Directory.Build.* or csproj etc.")>]
    project : string;
}

[<Verb("add", HelpText = "Add property to an existing file")>]
type AddOptions = {
    [<CommandLine.Option('o', "object", Required = false, HelpText = "Path to Directory.Build.* or csproj etc.")>]
    project : string;

    [<CommandLine.Option('p', "property", Required = true, HelpText = "Property name")>]
    property : string;

    [<CommandLine.Option('v', "value", Default = "", Required = false, HelpText = "Path to property")>]
    value : string;
}
    

let getActualProject project =
    // TODO: make auto-detect
    project


CommandLine.Parser.Default.ParseArguments<CreateOptions, AddOptions>(System.Environment.GetCommandLineArgs())
    .MapResult(
        (fun create ->
            let prj = getActualProject create.project
            createDirectoryBuild prj
        ),
        (fun (add : AddOptions) ->
            let prj = getActualProject add.project
            addProperty prj add.property add.value
        ),
        fun _ -> ()
    ) |> ignore

