module State

open Elmish
open Types
open Fable.PowerPack
open Fable.PowerPack.Fetch
open System
open Fable.Import
open Fable.SimpleJson

let productionUrl = "http://139.59.139.80/api/"
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
    #if DEBUG
    devUrl
    #endif
    productionUrl

let init args =
    { CollGeneration = CollectionGeneration.List
      OutputFeature = JustTypes
      Input = ""
      RootObjectName = "Root"
      Output = "" }, Cmd.none   

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

let update msg model =
    match msg with
    | BuildTypes value ->
        let newModel = { model with Input = value }
        newModel, generateStructureCmd newModel
    | RootNameChanged rootName ->
        let newModel = { model with RootObjectName = rootName }
        newModel, generateStructureCmd newModel
    | GenerateStructureLoaded (Result.Ok result) ->
        { model with Output = result }, Cmd.none
    | GenerateStructureLoaded (Result.Error e) ->
        { model with Output = e.Message }, Cmd.none
    | CollectionGenerationSelected collectionGeneration ->
        let newModel = { model with CollGeneration = collectionGeneration }
        newModel, generateStructureCmd newModel
    | OutputFeatureSelected outputFeature ->
        let newModel = { model with OutputFeature = outputFeature }
        newModel, generateStructureCmd newModel
