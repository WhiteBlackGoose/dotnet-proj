module ProjUtils

open System.IO
open System.Xml.Linq
#nowarn "3391" // implicit something

let createDirectoryBuild file =
    let content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Project>\n\n</Project>"
    File.WriteAllText(file, content)
    let fullFile = Path.GetFullPath file
    printfn $"Created in {fullFile}"

let assertFact fact message =
    if not fact then
        raise (System.Exception message)

let addPropertyOrItem (typeName : string) (file : string) property value attributes =
    let doc = XDocument.Load file

    let projects = doc.Elements "Project"
    
    assertFact (Seq.length projects = 1) "Should be one project element!"
    let projectElement = projects |> Seq.head
    
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
    if value <> "" then
        group.Add (XElement(property, value, attributesToInsert))
    else
        group.Add (XElement(property, attributesToInsert))
    
    doc.Save file

    printfn $"Saved to {Path.GetFullPath(file)}"