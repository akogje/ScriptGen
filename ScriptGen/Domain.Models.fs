namespace ScriptGen.Domain

module Models = begin

    type Config = {
        ServerVersion:int
        ServerName:string
        DbName:string        
        WorkingDirPath:string
        ScmToolPath:string
    }
    type SqlType =
        | Schema
        | Table | View 
        | Sproc | Func 
        | PKey | FKey 
        | Constraint //constraint covers ukey, tbl contraints, column defaults.
        | User | Role // | Grant grants need not be/ cannot be independently scripted.
        | Index  
        | Synonym
        | UDTT // is User Defined Table Type

    type DBObject = {
        Type: SqlType
        Schema: string option
        Name: string
        ScriptTxt:string
    }
     
    type Script = {
        Name: string
        Dirs: string list
        Body: string
    } 
    
    type FileRW = {
        RootDirPath: string
        CheckRootDirExists: unit -> Result<unit, exn>
        DeleteAll: unit -> Result<unit, exn>
        SaveAll: Script seq -> Result<unit, exn>
    }

    type ScmRepo = {
        WorkingDirPath: string
        ScmToolPath: string
        Pull: unit -> Result<unit, exn>
        CommitAndPush: unit -> Result<unit, exn>
    }

    type Batches = DBObject seq seq

    type SMO = {
        QueryAll: unit -> Result<Batches, exn>
    }
    type AppContainer = {
        Config:Config
        DB:SMO
        Disk:FileRW
        Git:ScmRepo
    }
    type App = {
        Run:unit -> unit
    }
end

