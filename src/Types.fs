module Types

type CollectionGeneration =
    | List
    | Array
    | CharpList

type OutputFeature =
    | JustTypes
    | NewtosoftAttributes

type Model =
    { CollGeneration: CollectionGeneration
      OutputFeature: OutputFeature
      Input: string
      RootObjectName: string
      Output: string }

type Msg =
    | BuildTypes of string
    | CollectionGenerationSelected of CollectionGeneration
    | OutputFeatureSelected of OutputFeature
    | Loaded of Result<string, exn>
    | RootNameChanged of string

type JsonResult<'a> =
    | Ok of 'a
    | Error of string

type Request =
    { Data: string
      RootObjectName : string
      ListGeneratorType: CollectionGeneration
      TypeGeneration: OutputFeature }
