// Learn more about F# at http://fsharp.org
open System
open System.Text.RegularExpressions
open System.Linq
open FSharp.Data

exception UnsupportedHtmlNodeError of HtmlNode

let numberingPattern = new Regex(@"\d+(\.\d+)+ ", RegexOptions.Compiled)
// TODO: Invoke HTML decoding & LaTeX encoding.
let translateText (htmlText: string) = htmlText

let rec translateHeader (node: HtmlNode) =
    let anchorNode = node.Descendants("a").Single()
    let anchorLabel = anchorNode.AttributeValue("name")

    let headerText =
        anchorNode.InnerText().Trim()
        |> fun text -> numberingPattern.Replace(text, "", 1)
        |> translateText

    let directive =
        match node with
        | n when n.HasName "h1" -> "chapter"
        | n when n.HasName "h2" -> "section"
        | n when n.HasName "h3" -> "subsection"
        | n when n.HasName "h4" -> "subsubsection"
        | _ -> raise (UnsupportedHtmlNodeError(node))

    sprintf "\\%s{%s\\label{%s}}\n" directive headerText anchorLabel

let translateCode (node: HtmlNode) = ""

let rec translate (node: HtmlNode) =
    match node with
    | n when n.HasName "h1" || n.HasName "h2" || n.HasName "h3" || n.HasName "h4" ->
        translateHeader n
    | n when n.HasName "p" -> "Paragraph"
    | n when n.HasName "table" -> "Table"
    | n when n.HasName "div" && n.HasClass("sectiontoc") -> ""
    | n when
        n.HasName "div"
        && n.Elements().Length = 1
        && n.Elements().Single().HasName("pre")
        ->
        translateCode n
    | n when n.HasName "ul" -> "Unordered List" // TODO: Remove toc
    | n when n.HasName "center" -> "Centering Node" // TODO: Dive into it.
    | n when n.HasName "" -> "Ignored"
    | n -> raise (UnsupportedHtmlNodeError(n))

[<EntryPoint>]
let main argv =
    let doc = HtmlDocument.Load("SWIG.html")

    doc.Body().Elements()
    |> Seq.map translate
    |> Seq.iter (fun line -> printfn "%s" line)

    0 // return an integer exit code
