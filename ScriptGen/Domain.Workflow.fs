namespace ScriptGen.Domain

open ScriptGen.Domain
open ScriptGen.Domain.Models
open ScriptGen.Common.Utils
open ScriptGen.Infra.Logger

module Workflow = begin

     let run (ct:AppContainer) =
        try
            let flow =
                logStart 
                >> Result.Ok
                => ct.Disk.CheckRootDirExists
                => ct.Git.Pull
                => ct.Disk.DeleteAll
                => ct.DB.QueryAll
                => Logic.scriptAll
                => ct.Disk.SaveAll
                => ct.Git.CommitAndPush
                >> logResult 
                >> logEnd               
                
            flow()

        with e -> logError e
end  
































(*
    let run4 (container:AppContainer) =
        try
            let db = container.SMO
            let disk = container.FileRW
            let git = container.SCMRepo


            let flow =
                Logic.logStart
                >> onOK disk.CheckRootDirExists
                >> onOK git.Pull
                >> onOK disk.DeleteAll
                >> onOK db.QueryAll
                >> onOK Logic.scriptAll
                >> onOK disk.SaveAll
                >> onOK git.CommitAndPush
                >> Logic.log
                >> Logic.logEnd
            do flow() |> ignore
        with e ->
                printfn "%A" e
                raise e

    let run3 (container:AppContainer) =
        try
            let db = container.SMO
            let disk = container.FileRW
            let git = container.SCMRepo
            
            let ``Check root directory exists.`` = Result.bind disk.CheckRootDirExists
            let ``Pull from Git server.`` = Result.bind git.Pull
            let ``Delete all existing files from root directory.`` = Result.bind disk.DeleteAll
            let ``Query sql for db objects.`` = Result.bind db.QueryAll
            let ``Script all objects.`` = Result.bind Logic.scriptAll
            let ``Save all scripts.`` = Result.bind disk.SaveAll
            let ``Commit to Git and push.`` = Result.bind git.CommitAndPush
            let ``Log results.`` = Logic.log
            let ``Start.`` = Logic.logStart
            let ``End.`` = Logic.logEnd
            
            let flow = 
                   ``Start.``
                >> ``Check root directory exists.``
                >> ``Pull from Git server.``
                >> ``Delete all existing files from root directory.``
                >> ``Query sql for db objects.``
                >> ``Script all objects.``
                >> ``Save all scripts.``
                >> ``Commit to Git and push.``
                >> ``Log results.``
                >> ``End.``
                
            flow() |> ignore

        with e ->
                printfn "%A" e
                raise e
  
    let run2 (container:AppContainer) =
        try
            let db = container.SMO
            let disk = container.FileRW
            let git = container.SCMRepo
            
            let result =
                Logic.logStart ()
                >>= disk.CheckRootDirExists
                >>= git.Pull
                >>= disk.DeleteAll
                >>= db.QueryAll
                >>= Logic.scriptAll
                >>= disk.SaveAll
                >>= git.CommitAndPush
                >>= Logic.logEnd
                
            Logic.log result

        with e ->
                printfn "%A" e
                raise e
*) 

  
