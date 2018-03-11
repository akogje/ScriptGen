namespace ScriptGen.Domain
open ScriptGen.Domain.Models
open System.Text.RegularExpressions
open System
module Logic =
   
    type ScriptGen.Domain.Models.SqlType
    with
    
    member x.extension = 
        match x with
        | Constraint -> "check" 
        | UDTT -> "udtt"
        | _ -> x.ToString().ToLower()   
    
    member x.subDir =
        match x with
        | Schema     -> "Schemas"
        | Table      -> "Tables" 
        | View       -> "Views"
        | Sproc      -> "Stored Procedures" 
        | Func       -> "Functions"
        | PKey       -> "Keys"
        | FKey       -> "Keys"
        | Constraint -> "Constraints"
        | Index      -> "Indexes"
        | User       -> "Users"
        | Role       -> "Roles" 
        | Synonym    -> "Synonyms"
        | UDTT       -> "User Defined Table Types"
    end
    
    type ScriptGen.Domain.Models.DBObject
    with   
    member x.fileName = 
        let err = sprintf "The schema is incorrect for %A %s." x.Type x.Name
        let schema =  
            match x.Type, x.Schema with
            | Table     , Some s 
            | View      , Some s 
            | Sproc     , Some s
            | Func      , Some s 
            | PKey      , Some s
            | FKey      , Some s  
            | Constraint, Some s
            | Index     , Some s
            | Synonym   , Some s
            | UDTT      , Some s -> s
            | Schema    , None
            | User      , None 
            | Role      , None -> ""
            | _ -> failwith err

        let name = sprintf "%s.%s.%s.sql" schema x.Name x.Type.extension
        name.TrimStart '.'
    end

    let toScript (o:DBObject) = {
        Name = o.fileName
        Dirs = [o.Type.subDir]
        Body = o.ScriptTxt
    }
    
    let private ``remove[]`` (str:string) = 
        str.Replace("[", "").Replace("]", "")

    let private rowVersion (str:string) = 
        str.Replace("timestamp", "rowversion")

    let private ``WITH(PAD_...)`` = 
        Regex (@"WITH \(PAD_INDEX.+?\)", RegexOptions.Compiled)

    let private removeWITH (str:string) =
        ``WITH(PAD_...)``.Replace(str, "")

    let formatSql s =  {
        s with Body = 
                s.Body 
                |> ``remove[]`` 
                |> rowVersion
                |> removeWITH
    }

    let scriptAll batches =
        try
            batches
            |> Seq.concat 
            |> Seq.map toScript
            |> Seq.map formatSql
            |> Result.Ok
        with e -> Result.Error e
