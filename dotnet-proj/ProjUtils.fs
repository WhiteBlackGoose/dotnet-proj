module ProjUtils

open System.IO
open System.Xml.Linq

let createDirectoryBuild file =
    let content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Project>\n\n</Project>"
    File.WriteAllText(file, content)
    let fullFile = Path.GetFullPath file
    printfn $"Written to {fullFile}"

let assertFact fact message =
    if not fact then
        raise (System.Exception message)

let addProperty (file : string) property value =
    let doc = XDocument.Load file

    let projects = doc.Elements "Project"
    
    assertFact (Seq.length projects = 1) "Should be one project element!"
    let projectElement = projects |> Seq.head
    
    let propertyGroup =
        match projectElement.Elements () |> Seq.tryLast with
        | Some element when element.Name = "PropertyGroup" -> element
        | _ ->
            let element = XElement("PropertyGroup")
            projectElement.Add element
            element
    if value <> "" then
        propertyGroup.Add (XElement(property, value))
    else
        propertyGroup.Add (XElement(property))
    
    doc.Save file

    printfn $"Saved to {Path.GetFullPath(file)}"