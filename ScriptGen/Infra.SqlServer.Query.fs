namespace ScriptGen.Infra.SqlServer

open Microsoft.SqlServer.Management.Smo
open ScriptGen.Infra.SqlServer.SqlAdapters  

module Query =
    
    let private getSchemas (db:Database) (sc:Scriptor) = 
        db.Schemas 
        |> Seq.cast<Schema> 
        |> Seq.filter (fun t -> not t.IsSystemObject)  
        |> Seq.map sc.toSchema 
    
    let private userTables (db:Database) =
        db.Tables 
        |> Seq.cast<Table> 
        |> Seq.filter (fun t -> not t.IsSystemObject) 
    
    let private userViews (db:Database) =
        db.Views 
        |> Seq.cast<View> 
        |> Seq.filter (fun t -> not t.IsSystemObject) 
    
    let private getTables (db:Database) (sc:Scriptor) = 
        db |> userTables |> Seq.map sc.toTable
    
    let private getViews (db:Database) (sc:Scriptor) = 
        db |> userViews |> Seq.map sc.toView
    
    let private getSprocs (db:Database) (sc:Scriptor) = 
        db.StoredProcedures 
        |> Seq.cast<StoredProcedure> 
        |> Seq.filter (fun t -> not t.IsSystemObject) 
        |> Seq.map sc.toSproc
    
    let private getFuncs (db:Database) (sc:Scriptor) = 
        db.UserDefinedFunctions 
        |> Seq.cast<UserDefinedFunction> 
        |> Seq.filter (fun t -> not t.IsSystemObject) 
        |> Seq.map sc.toFunc    
    
    let private getChecks' (sc:Scriptor) (tbl:Table) = 
        tbl.Checks
        |> Seq.cast<Check>
        |> Seq.map (sc.toConstraint2 tbl.Schema)
    
    let private getChecks (db:Database) (sc:Scriptor) = 
        db |> userTables  
           |> Seq.map (getChecks' sc)
           |> Seq.concat
    
    let private getDefaults' (sc:Scriptor) (tbl:Table)  =
         tbl.Columns 
         |> Seq.cast<Column> 
         |> Seq.map (fun c -> c.DefaultConstraint)
         |> Seq.filter ((<>) null)
         |> Seq.map (sc.toConstraint tbl.Schema)
    
    let private getDefaults (db:Database) (sc:Scriptor) = 
        db |> userTables
           |> Seq.map (getDefaults' sc)
           |> Seq.concat
    
    let private getFKeys' (sc:Scriptor) (tbl:Table) = 
        tbl.ForeignKeys
        |> Seq.cast<ForeignKey>
        |> Seq.map (sc.toFKey tbl.Schema)
    
    let private getFKeys (db:Database) (sc:Scriptor) = 
        db |> userTables 
           |> Seq.map (getFKeys' sc)
           |> Seq.concat
    
    let private getUsers (db:Database) (sc:Scriptor) = 
        db.Users 
        |> Seq.cast<User> 
        |> Seq.filter (fun t -> not t.IsSystemObject) 
        |> Seq.map sc.toUser
    
    let private getRoles (db:Database) (sc:Scriptor) = 
        db.Roles 
        |> Seq.cast<DatabaseRole>  
        |> Seq.filter (fun r -> not (r.Name = "public" || r.IsFixedRole))
        |> Seq.map sc.toRole

    let private getIndexes' (sc:Scriptor) (tbl:Table) = 
        tbl.Indexes
        |> Seq.cast<Index> 
        |> Seq.map (sc.toIndex tbl.Schema)
    
    let private getIndexes (db:Database) (sc:Scriptor) = 
        db |> userTables 
           |> Seq.map (getIndexes' sc)
           |> Seq.concat
    
    let private getSynonyms (db:Database) (sc:Scriptor) = 
        db.Synonyms 
        |> Seq.cast<Synonym>          
        |> Seq.map sc.toSynonym

    let private getUDTTs (db:Database) (sc:Scriptor) = 
        db.UserDefinedTableTypes
        |> Seq.cast<UserDefinedTableType>
        |> Seq.map sc.toUDTT


    let private getTablesWithAll (db:Database) (sc:Scriptor) = seq {
        for tbl in db |> userTables do
            yield sc.toTable tbl
            yield! getChecks' sc tbl
            yield! getDefaults' sc tbl
            yield! getFKeys' sc tbl
            yield! getIndexes' sc tbl
    }
    
    (*   
    let private getGrants' (sc:Scriptor) (view:View) =
        view.EnumObjectPermissions()
        |> Array.map sc.toGrant
        |> Array.toSeq

    let private getGrants (db:Database) (sc:Scriptor) = 
        db |> userViews 
           |> Seq.map (getGrants' sc)
           |> Seq.concat
        This is incomplete. Grants can be on any object. 
        To search permissions by iterating all objects is impractical
        Rather use ScriptingOptions.Permissions = true to automatically script grants along with parent objects.

    *)
    type Context = {
        Db: Database
        Sc: Scriptor
    }
    with
    member x.getSchemas  () = getSchemas  x.Db x.Sc
    member x.getTables   () = getTables   x.Db x.Sc
    member x.getViews    () = getViews    x.Db x.Sc
    member x.getSprocs   () = getSprocs   x.Db x.Sc
    member x.getFuncs    () = getFuncs    x.Db x.Sc
    member x.getChecks   () = getChecks   x.Db x.Sc
    member x.getDefaults () = getDefaults x.Db x.Sc
    member x.getFKeys    () = getFKeys    x.Db x.Sc
    member x.getUsers    () = getUsers    x.Db x.Sc
    member x.getRoles    () = getRoles    x.Db x.Sc
    //member x.getGrants ()   = getGrants   x.Db x.Sc
    member x.getIndexes  () = getIndexes  x.Db x.Sc
    member x.getSynonyms () = getSynonyms x.Db x.Sc
    member x.getUDTTs    () = getUDTTs    x.Db x.Sc
    member x.getTablesWithAll () = getTablesWithAll x.Db x.Sc