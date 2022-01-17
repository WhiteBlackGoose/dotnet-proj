open CommandLine
open ProjUtils
open System.Collections.Generic
#nowarn "3391" // implicit something

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

    [<CommandLine.Option('p', "property", SetName = "Type", HelpText = "Property name")>]
    property : string;

    [<CommandLine.Option('i', "item", SetName = "Type", HelpText = "Property name")>]
    item : string;

    [<CommandLine.Option('c', "content", Default = "", Required = false, HelpText = "Path to property")>]
    content : string;

    [<CommandLine.Option('a', "attributes", HelpText = "Path to property")>]
    attributes : IEnumerable<string>;
}


let getActualProject project =
    // TODO: make auto-detect
    project


let args = System.Environment.GetCommandLineArgs()[1..]

CommandLine.Parser.Default.ParseArguments<CreateOptions, AddOptions>(args)
    .MapResult(
        (fun (create : CreateOptions) ->
            let prj = getActualProject create.project
            createDirectoryBuild prj
        ),
        (fun (add : AddOptions) ->
            let (|NotEmpty|Empty|) = function
                | "" -> Empty
                | s when isNull s -> Empty
                | other -> NotEmpty other
            

            let prj = getActualProject add.project
            let pairs =
                (add.attributes |> Seq.skip 1)
                |> Seq.zip add.attributes
                |> Seq.chunkBySize 2
                |> Seq.map (fun arr -> arr[0])
            match (add.property, add.item) with
            | (Empty, NotEmpty item) -> addPropertyOrItem "Item" prj item add.content pairs
            | (NotEmpty property, Empty) -> addPropertyOrItem "Property" prj property add.content pairs
            | _ -> printfn $"Specify exactly one: --item or --property"
        ),
        fun _ -> ()
    ) |> ignore

