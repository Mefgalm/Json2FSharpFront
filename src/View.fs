module View

open Types
open Fable.Helpers.React
open Fulma
open Elmish
open Fable.Core.JsInterop
open Fable.Helpers.React.Props
open Fable.FontAwesome
open Fable.Import


type KeyValue<'key, 'value> =
    { Key: 'key
      Value: 'value }

let collectionGenerationSet = Set.ofList [
        { Key = Types.List; Value = "List" }
        { Key = Types.Array; Value = "Array" }
        { Key = Types.CharpList; Value = "List<T>" }
    ]

let outputFeatureSet = Set.ofList [
        { Key = JustTypes; Value = "Just Types" }
        { Key = NewtosoftAttributes; Value = "Newtosoft Attributes" }
    ]

let dropDown<'b, 'a when 'a : equality> className (item: 'a) (items: seq<KeyValue<'a, 'b>>) toView dispatch msg =
    let getKey {Key = key} = key

    Dropdown.dropdown [ Dropdown.IsHoverable ]
      [ div [ ClassName className ]
            [ Button.button []
                [ span []
                    [ str ((items |> Seq.find(getKey >> (=) item)) |> toView) ]
                  Icon.icon [ Icon.Size IsSmall ]
                    [ Fa.i [ Fa.Solid.AngleDown ] [] ] ] ]
        Dropdown.menu []
            [ Dropdown.content []
                [ yield! items 
                            |> Seq.map(fun x -> div []                                              
                                                    [ Dropdown.Item.a 
                                                        [ Dropdown.Item.IsActive (getKey x = item)
                                                          Dropdown.Item.Props [ OnClick (fun _ -> dispatch (msg (x |> getKey)) ) ] ] 
                                                        [ str (x |> toView) ] ]) ] ] ]
     
let header =
    div [ ClassName "header" ]
        [ label [ ClassName "project-name" ] [ str "Json2FSharp" ]
          div [ ClassName "links" ]
              [ div [ ClassName "github-front" ]
                    [ label [] [ str "Source code " ]
                      a [ ClassName "link"
                          Href "https://github.com/Mefgalm/Json2FSharpFront"
                          Target "_blank" ]
                        [ str "front-end" ] ]
                div [ ClassName "github-back" ]
                    [ label [] [ str "Source code " ]
                      a [ ClassName "link"
                          Href "https://github.com/Mefgalm/Json2FSharpBack"
                          Target "_blank" ]
                        [ str "back-end" ] ] ] ]

let inputBlock (model: Model) dispatch =
    div [ ClassName "input-area" ]
        [ div [ ClassName "input-block" ]
              [ label [] [ str "Root Object " ]
                input [ ClassName "input-root-name"
                        Value model.RootObjectName
                        OnChange (fun ev -> !!ev.target?value |> RootNameChanged |> dispatch)] ]
          textarea [ ClassName "input-text-area"
                     OnChange (fun ev -> !!ev.target?value |> BuildTypes |> dispatch)] [] ]

let outputBlock model =
    div [ ClassName "output-area" ]
        [ textarea [ ClassName "output-text-area"
                     ReadOnly true                
                     Value model.Output ] [] ]

let settings model dispatch  =
    div [ ClassName "settings" ]
        [ div [ ClassName "setting-row" ]
              [ label [] [ str "Collection generation" ]
                dropDown "dropdown" model.CollGeneration collectionGenerationSet (fun { Value = x } -> x.ToString()) dispatch CollectionGenerationSelected ]
          div [ ClassName "setting-row" ]
              [ label [] [ str "Output features" ]
                dropDown "dropdown" model.OutputFeature outputFeatureSet (fun { Value = x } -> x.ToString()) dispatch OutputFeatureSelected ] ]

let drag = !^(fun e ->                
                let outputAreaWidth = Browser.window.innerWidth - e?pageX
                let inputAreaWidth = Browser.window.innerWidth - outputAreaWidth
                Browser.document.getElementsByClassName("input-area").[0]?style?width <- sprintf "%ipx" (inputAreaWidth |> int)
                Browser.document.getElementsByClassName("output-area").[0]?style?width <- sprintf "%ipx" (outputAreaWidth |> int))

let dragBar =
    div [ ClassName "drag-bar"
          OnMouseDown (fun _ -> Browser.document.addEventListener ("mousemove", drag))
          OnMouseUp (fun _ -> Browser.document.removeEventListener ("mousemove", drag))] []

let root model dispatch =
    div [ ClassName "main" ]
        [ header
          div [ ClassName "area" ]
            [ inputBlock model dispatch
              dragBar
              outputBlock model
              settings model dispatch ] ]
