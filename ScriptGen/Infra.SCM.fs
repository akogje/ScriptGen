namespace ScriptGen.Infra

open ScriptGen.Domain.Models
open ScriptGen.Common.Utils
open ScriptGen.Infra.Logger

module SCM =

    let merge out err =
        List.concat [out; err]
        |> List.filter notBlank
        |> concat
        |> (fun s -> if notBlank s then Some s else None)

    let private run exePath workingDir args =
        let stdout, stderr = ExternalProcess.run exePath workingDir args
        let messages = merge stdout stderr
        match messages with 
        | Some s -> sprintf "\n(git %s):\n%s" args s 
                    |> logMsg;
                    if s.Contains("error:")
                    then Result.Error (exn s)
                    else Result.Ok ()
        | None -> Result.Ok ()

    let private gitCommit exePath workingDir =
        try 
            let add () = run exePath workingDir "add -A"
            let commit () = run exePath workingDir "commit -a -m \"commiting new change.\""
            let addAndCommit =  add => commit
            addAndCommit()
        with e -> 
            Result.Error e
    
    let initSCM (config:Config) = {
        WorkingDirPath = config.WorkingDirPath
        ScmToolPath = config.ScmToolPath
        Pull = fun _ -> Result.Ok () //"fetch origin"
        CommitAndPush = fun _ -> gitCommit config.ScmToolPath config.WorkingDirPath
    }