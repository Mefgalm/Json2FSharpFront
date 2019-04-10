module Types

type CollectionGeneration =
    | List
    | Array
    | Sequence
    | CharpList

type OutputFeature =
    | JustTypes
    | NewtosoftAttributes

type Output =
    | Success of string
    | Fail of string

type Model =
    { CollGeneration: CollectionGeneration
      OutputFeature: OutputFeature
      Input: string
      RootObjectName: string
      ShowSettings: bool
      Output: Output }

type Msg =
    | BuildTypes of string
    | CollectionGenerationSelected of CollectionGeneration
    | OutputFeatureSelected of OutputFeature
    | GenerateStructureLoaded of Result<string, exn>
    | RootNameChanged of string
    | ToggleSettings

type JsonResult<'a> =
    | Ok of 'a
    | Error of string

type Request =
    { Data: string
      RootObjectName : string
      ListGeneratorType: CollectionGeneration
      TypeGeneration: OutputFeature }

type KeyValue<'key, 'value> =
    { Key: 'key
      Value: 'value }
