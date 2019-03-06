module Program

open Elmish
open Elmish.Browser.Navigation
open Fable.Core.JsInterop
open Elmish.React
open State
open View

importAll "../sass/main.sass"
    
open Elmish.Debug
open Elmish.HMR

Program.mkProgram init update root
#if DEBUG
|> Program.withDebugger
#endif
|> Program.withReact "elmish-app"
|> Program.run
