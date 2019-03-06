module State

open Elmish
open Types
open Fable.PowerPack
open Fable.PowerPack.Fetch
open System
open Fable.Import
open Fable.SimpleJson

let init args =
    { CollGeneration = CollectionGeneration.List
      OutputFeature = JustTypes
      Input = ""
      RootObjectName = "Root"
      Output = "" }, Cmd.none


let fakeLoad model =
    promise {
        let data =
            { Data = model.Input
              RootObjectName = model.RootObjectName
              ListGeneratorType = model.CollGeneration
              TypeGeneration = model.OutputFeature  }

        let! post = postRecord "http://localhost:51014/generate" data []

        match post.Ok with
        | true ->
            return! post.text() |> Promise.map(fun x -> Json.parseAs<JsonResult<string>> x)
        | false ->
            return JsonResult.Error post.StatusText
    }  

   
let ofResult response =
    match response with
    | JsonResult.Ok result ->
        Loaded (Result.Ok result)
    | JsonResult.Error result ->
        Loaded (Result.Error (Exception(result)))

let ofFail ex = Loaded (Result.Error ex)
    
let loadCmd newModel = Cmd.ofPromise fakeLoad newModel ofResult ofFail

let update msg model =
    match msg with
    | BuildTypes value ->
        let newModel = { model with Input = value }
        newModel, loadCmd newModel
    | RootNameChanged rootName ->
        let newModel = { model with RootObjectName = rootName }
        newModel, loadCmd newModel
    | Loaded (Result.Ok result) ->
        { model with Output = result }, Cmd.none
    | Loaded (Result.Error e) ->
        { model with Output = e.Message }, Cmd.none
    | CollectionGenerationSelected collectionGeneration ->
        let newModel = { model with CollGeneration = collectionGeneration }
        newModel, loadCmd newModel
    | OutputFeatureSelected outputFeature ->
        let newModel = { model with OutputFeature = outputFeature }
        newModel, loadCmd newModel
