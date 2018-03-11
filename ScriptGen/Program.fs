namespace ScriptGen
open System
open ScriptGen.Application
module ConsoleHost = 
    
    [<EntryPoint>]
    let main args = 
        try
            let app = AppHost.setup ()
            app.Run ()
        finally
            Console.ReadKey true |> ignore
        0

