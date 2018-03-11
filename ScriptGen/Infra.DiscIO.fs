namespace ScriptGen.Infra
open System.IO
open ScriptGen.Domain.Models
open ScriptGen.Common.Utils
open ScriptGen.Infra.Logger

module DiskIO = 


    let formPath = List.toArray >> Path.Combine

    let private ensureFolders dirPath =
        try
            if Directory.Exists dirPath
            then Ok ()
            else 
                Directory.CreateDirectory dirPath
                |> ignore
                Ok ()
        with
            e -> Result.Error e
      
    let private save root script =
        let dirPath = (root::script.Dirs) |> formPath
        let filePath = [dirPath; script.Name] |> formPath
        let checkDir _ = ensureFolders dirPath
        let saveFile _ = 
            try
                filePath |> sprintf "%s" |> logMsg
                File.WriteAllText(filePath, script.Body)
                Result.Ok ()
            with
                e -> Result.Error e
        let checkAndSave = checkDir => saveFile
        checkAndSave()

    let private delete dirPath =
        try 
            Directory.Delete(dirPath, true)
            Result.Ok dirPath
        with 
            e -> Result.Error e

    let private deleteAllUnder = delete => ensureFolders

    let private listErrors es result =  
        match result with
        | Result.Ok _ -> es
        | Result.Error e -> e::es
    
    let private saveAll root (scripts:Script seq)  = // TODO: do not collect all errors. Fail on first error.
        scripts
        |> Seq.map (save root)
        |> Seq.fold listErrors []
        |> function [] -> Result.Ok () | es -> Result.Error es.Head

    let initFileRW (config:Config) = 
        let root = config.WorkingDirPath in {
        RootDirPath = root
        CheckRootDirExists = fun _ -> ensureFolders root
        DeleteAll = fun _ -> deleteAllUnder root
        SaveAll = (saveAll root)
    }
    
   