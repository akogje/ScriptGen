namespace ScriptGen.Infra
open System

module Logger = begin
    
    let private timestamp() = 
        DateTime.Now.ToString "yyyy-MM-dd HH:mm:ss"

    let logMsg msg = 
        let ts = timestamp()
        printfn "[%s]: %s" ts msg

    let logError (e:Exception) =  
        e |> sprintf "%A" |> logMsg

    let logResult = function
    | Result.Ok _ -> logMsg "done!"
    | Result.Error e -> e |> sprintf "%A" |> logMsg

    let logStart _ = logMsg "Started."
    
    let logEnd _ = logMsg "Ended."

end

