open CommandLine
open ProjUtils
open System.Collections.Generic
open System.IO
#nowarn "3391" // implicit something

[<Verb("create", HelpText = "Create new manifest file")>]
type CreateOptions = {
    [<Option('o', "object", Required = true, HelpText = "Path to Directory.Build.* or csproj etc.")>]
    project : string;
}

[<Verb("add", HelpText = "Add property to an existing file")>]
type AddOptions = {
    [<Option('o', "object", Required = false, HelpText = "Path to Directory.Build.* or csproj etc.")>]
    project : string;

    [<Option('p', "property", SetName = "Type", HelpText = "Property name")>]
    property : string;

    [<Option('i', "item", SetName = "Type", HelpText = "Property name")>]
    item : string;

    [<Option('c', "content", Default = "", Required = false, HelpText = "Path to property")>]
    content : string;

    [<Option('a', "attributes", HelpText = "Path to property")>]
    attributes : IEnumerable<string>;
}


let getActualProject = function
    | NotEmpty path ->
        [ path ]
    | Empty ->
        Directory.GetFiles "./"
        |> Seq.map (fun c -> c[2..])
        |> Seq.where (fun c ->
            c.StartsWith "Directory.Build."
            // TODO: is it safe to replace with ends with proj?
            || c.EndsWith ".csproj"
            || c.EndsWith ".fsproj"
            || c.EndsWith ".vbproj"
            || c.EndsWith ".ilproj"
            )
        |> List.ofSeq



let args = System.Environment.GetCommandLineArgs()[1..]

CommandLine.Parser.Default.ParseArguments<CreateOptions, AddOptions>(args)
    .MapResult(
        (fun (create : CreateOptions) ->
            createDirectoryBuild create.project
        ),
        (fun (add : AddOptions) ->
            match getActualProject add.project with
            | [ prj ] ->
                let pairs =
                    add.attributes
                    |> Seq.chunkBySize 2
                    |> Seq.choose (function [|x;y|] -> Some (x,y) | _ -> None)
                match (add.property, add.item) with
                | (Empty, NotEmpty item) ->
                    addPropertyOrItem "Item" prj item add.content pairs
                    0
                | (NotEmpty property, Empty) ->
                    addPropertyOrItem "Property" prj property add.content pairs
                    0
                | _ ->
                    printfn $"Specify exactly one: --item or --property"
                    1
            | [] ->
                printfn $"No files detected, try specifying explicitly"
                1
            | tooMany ->
                printfn $"Ambiguous choice! {tooMany}"
                1
        ),
        fun _ -> 1
    )
|> exit