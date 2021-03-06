module ProjUtils

open System.IO
open System.Xml.Linq
#nowarn "3391" // implicit something


let (|NotEmpty|Empty|) = function
    | "" -> Empty
    | s when isNull s -> Empty
    | other -> NotEmpty other

let (|StartsWith|_|) (s : string) (input : string) =
    if input.StartsWith s then
        Some ()
    else
        None

let (|EndsWith|_|) (s : string) (input : string) =
    if input.EndsWith s then
        Some ()
    else
        None

let createDirectoryBuild (file : string) =
    let content =
        match Path.GetFileName file with
        | StartsWith "Directory.Build." ->
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Project>\n\n</Project>" |> Some
        | EndsWith "proj" ->
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Project Sdk=\"Microsoft.NET.Sdk\">\n\n</Project>" |> Some
        | _ -> None
    match content with
    | Some content ->
        let fullPath = Path.GetFullPath file
        let dir = Path.GetDirectoryName fullPath
        Directory.CreateDirectory dir |> ignore
        File.WriteAllText(file, content)
        printfn $"Created in {fullPath}"
        0
    | None ->
        printfn $"Cannot detect the right format, create *proj or Directory.Build.* files"
        1

let assertFact fact message =
    if not fact then
        raise (System.Exception message)

let addPropertyOrItem (typeName : string) (file : string) property value attributes =
    let doc = XDocument.Load file

    let projects = doc.Elements "Project"
    
    let projectElement = 
        projects
        |> Seq.tryExactlyOne
        |> Option.defaultWith (fun _ -> failwith "Should be one project element!")
    
    let group =
        match projectElement.Elements () |> Seq.tryLast with
        | Some element when element.Name = $"{typeName}Group" -> element
        | _ ->
            let element = XElement($"{typeName}Group")
            projectElement.Add element
            element
    let attributesToInsert = 
        attributes
            |> Seq.map (fun (a : string, b : string) -> XAttribute(a, b))
    
    match value with
    | NotEmpty value ->
        group.Add (XElement(property, value, attributesToInsert))
    | Empty ->
        group.Add (XElement(property, attributesToInsert))
    
    doc.Save file

    printfn $"Saved to {Path.GetFullPath(file)}"