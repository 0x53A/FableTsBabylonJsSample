module Client

open Elmish
open Elmish.React

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Shared

open Fulma


// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type FileModel = { Path : string; FileName : string }
type Model = { Counter: Counter option; File: FileModel option }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| Increment
| Decrement
| InitialCountLoaded of Result<Counter, exn>
| ShowModel1
| ShowModel2



// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { Counter = None; File = None }
    let loadCountCmd =
        Cmd.ofPromise
            (fetchAs<int> "/api/init")
            []
            (Ok >> InitialCountLoaded)
            (Error >> InitialCountLoaded)
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel.Counter, msg with
    | Some x, Increment ->
        let nextModel = { currentModel with Counter = Some (x + 1) }
        nextModel, Cmd.none
    | Some x, Decrement ->
        let nextModel = { currentModel with Counter = Some (x - 1) }
        nextModel, Cmd.none
    | _, InitialCountLoaded (Ok initialCount)->
        let nextModel = { Counter = Some initialCount; File = None }
        nextModel, Cmd.none
        
    | _, ShowModel1 -> { currentModel with File = Some { Path = "https://playground.babylonjs.com/scenes/"; FileName = "dummy3.babylon" } }, Cmd.none
    | _, ShowModel2 -> { currentModel with File = Some { Path = "https://playground.babylonjs.com/scenes/"; FileName = "skull.babylon" } }, Cmd.none

    | _ -> currentModel, Cmd.none


// -------------------------------------------------------
// Import the TypeScript component

type SingleFileBabylonViewerProps =
  | StlUrl of string
  | StlFileName of string

let inline SingleFileBabylonViewer (props : SingleFileBabylonViewerProps list) : Fable.Import.React.ReactElement =
    ofImport "default" "./SingleFileBabylonViewer.tsx" (keyValueList CaseRules.LowerFirst props) []
    
// -------------------------------------------------------


let safeComponents =
    let components =
        span [ ]
           [
             a [ Href "https://saturnframework.github.io" ] [ str "Saturn" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io/elmish/" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://mangelmaxime.github.io/Fulma" ] [ str "Fulma" ]
           ]

    p [ ]
        [ strong [] [ str "SAFE Template" ]
          str " powered by: "
          components ]

let show = function
| { Counter = Some x } -> string x
| { Counter = None   } -> "Loading..."

let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ]
        [ str txt ]

let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]

          Container.container []
              [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ Heading.h3 [] [ str ("Press buttons to manipulate counter: " + show model) ] ]
                Columns.columns []
                    [ Column.column [] [ button "-" (fun _ -> dispatch Decrement) ]
                      Column.column [] [ button "+" (fun _ -> dispatch Increment) ] ]

                Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ Heading.h3 [] [ str ("Select a model to view ") ] ]
                Columns.columns []
                    [ Column.column [] [ button "Show Human" (fun _ -> dispatch ShowModel1) ]
                      Column.column [] [ button "Show Skull" (fun _ -> dispatch ShowModel2) ] ]

                // Render the Typescript Component
                (match model.File with
                 | Some f -> SingleFileBabylonViewer [ StlUrl f.Path; StlFileName f.FileName ]
                 | None -> str "No model selected")
              ]

          Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ safeComponents ] ] ]


#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
