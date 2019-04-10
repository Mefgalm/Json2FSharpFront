module State

open Elmish
open Types
open Fable.PowerPack
open Fable.PowerPack.Fetch
open System
open Fable.Import
open Fable.SimpleJson

let productionUrl = "https://json2fsharp.com/api/"
let devUrl = "http://localhost:51014/"

type private Api = Api of string

type private ApiType =
    | GenerateStructure

let private generateUrl baseUrl = function
    | GenerateStructure -> Api (sprintf "%s%s" baseUrl "generate")

type private RequestDataAndRoute<'a>  =
    { Model : 'a
      Api: Api }

let private baseUrl =
    //#if DEBUG
    //devUrl
    //#endif
    productionUrl

let private generateDataStructureApi requestDataAndRoute =
    promise {
        let data =
            { Data = requestDataAndRoute.Model.Input
              RootObjectName = requestDataAndRoute.Model.RootObjectName
              ListGeneratorType = requestDataAndRoute.Model.CollGeneration
              TypeGeneration = requestDataAndRoute.Model.OutputFeature  }

        let getUrl (Api url) = url

        let! post = postRecord (requestDataAndRoute.Api |> getUrl) data []

        match post.Ok with
        | true ->
            return! post.text() |> Promise.map(fun x -> Json.parseAs<JsonResult<string>> x)
        | false ->
            return JsonResult.Error post.StatusText
    }

let ofResult response =
    match response with
    | JsonResult.Ok result ->
        GenerateStructureLoaded (Result.Ok result)
    | JsonResult.Error result ->
        GenerateStructureLoaded (Result.Error (Exception(result)))

let ofFail ex = GenerateStructureLoaded (Result.Error ex)

let generateStructureCmd newModel =
    let requestDataAndUrl =
        { Model = newModel
          Api = generateUrl baseUrl GenerateStructure }

    Cmd.ofPromise generateDataStructureApi requestDataAndUrl ofResult ofFail

let inputDefault =
    "{
    \"Welcome\": \"Json2FSharp\"
}"


let init _ =
    let initModel =
        { CollGeneration = CollectionGeneration.List
          OutputFeature = JustTypes
          Input = inputDefault
          RootObjectName = "Root"
          ShowSettings = true
          Output = Success "" }

    initModel, generateStructureCmd initModel

let update msg model =
    match msg with
    | BuildTypes value ->
        let newModel = { model with Input = value }
        newModel, generateStructureCmd newModel
    | RootNameChanged rootName ->
        let newModel = { model with RootObjectName = rootName }
        newModel, generateStructureCmd newModel
    | GenerateStructureLoaded (Result.Ok result) ->
        { model with Output = Success result }, Cmd.none
    | GenerateStructureLoaded (Result.Error e) ->
        { model with Output = Fail e.Message }, Cmd.none
    | CollectionGenerationSelected collectionGeneration ->
        let newModel = { model with CollGeneration = collectionGeneration }
        newModel, generateStructureCmd newModel
    | OutputFeatureSelected outputFeature ->
        let newModel = { model with OutputFeature = outputFeature }
        newModel, generateStructureCmd newModel
    | ToggleSettings ->
        let newModel = { model with ShowSettings = not (model.ShowSettings) }
        newModel, Cmd.none
