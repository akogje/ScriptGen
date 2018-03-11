namespace ScriptGen.Infra.SqlServer

open Microsoft.SqlServer.Management.Smo
open Microsoft.SqlServer.Management.Common
open System.Data.SqlClient 
open ScriptGen.Domain.Models
open ScriptGen.Infra.SqlServer
open Query
open ScriptGen.Infra.Logger

module Setup = 

    let private realVersion = function
        //| 2017  -> SqlServerVersion.Version140
        | 2016  -> SqlServerVersion.Version130
        | 2014  -> SqlServerVersion.Version120
        | 2012  -> SqlServerVersion.Version110
        | other -> other |> sprintf "Sql server %d not supported." |> failwith
 
    let private prefetch<'a> (s:Server) (fields: string array) =
        s.SetDefaultInitFields(typeof<'a>, fields)
    
    let private prefetchFields (s:Server) = 
        s.SetDefaultInitFields false
        prefetch<Schema>               s [|"IsSystemObject"; "Name"|]
        prefetch<Table>                s [|"IsSystemObject"; "Name"; "Schema"|] 
        prefetch<View>                 s [|"IsSystemObject"; "Name"; "Schema"|] 
        prefetch<StoredProcedure>      s [|"IsSystemObject"; "Name"; "Schema"|] 
        prefetch<UserDefinedFunction>  s [|"IsSystemObject"; "Name"; "Schema"|] 
        prefetch<ForeignKey>           s [|"Name"|]
        prefetch<DefaultConstraint>    s [|"Name"|]
        prefetch<Check>                s [|"Name"|]
        prefetch<User>                 s [|"IsSystemObject"; "Name"|]
        prefetch<DatabaseRole>         s [|"Name"|]
        prefetch<Index>                s [|"Name"|]
        prefetch<Synonym>              s [|"Name"; "Schema"|]
        prefetch<UserDefinedTableType> s [|"Name"; "Schema"|] 

    let private queryAll (connStr:SqlConnectionStringBuilder) (options:ScriptingOptions) =
        try
            use sqlConn = new SqlConnection(connStr.ConnectionString)
            let srvConn = ServerConnection sqlConn
            let server = Server srvConn  
            prefetchFields server
            let db = server.Databases.[connStr.InitialCatalog]
            sprintf "%s\%s Connected!" server.Name db.Name |> logMsg
            let ctx = {Db = db; Sc = {SO = options}}
            let batches = seq {
                yield ctx.getSchemas  ()
                yield ctx.getTables   ()
                yield ctx.getViews    ()
                yield ctx.getSprocs   ()
                yield ctx.getFuncs    ()
                yield ctx.getChecks   ()
                yield ctx.getDefaults ()
                yield ctx.getFKeys    ()
                yield ctx.getUsers    ()
                yield ctx.getRoles    ()
                yield ctx.getIndexes  ()
                yield ctx.getSynonyms ()
                yield ctx.getUDTTs    ()
                yield Seq.empty
            }
            Result.Ok batches
        with
            e -> Result.Error e
 
    let private queryAll2 (connStr:SqlConnectionStringBuilder) (options:ScriptingOptions) =
        try
            use sqlConn = new SqlConnection(connStr.ConnectionString)
            let srvConn = ServerConnection sqlConn
            let server = Server srvConn  
            prefetchFields server
            let db = server.Databases.[connStr.InitialCatalog]
            sprintf "%s\%s Connected!" server.Name db.Name |> logMsg
            let ctx = {Db = db; Sc = {SO = options}}
            let batches = seq {
                yield ctx.getSchemas  ()
                yield ctx.getTablesWithAll ()
                yield ctx.getViews    ()
                yield ctx.getSprocs   ()
                yield ctx.getFuncs    ()
                yield ctx.getUsers    ()
                yield ctx.getRoles    ()
                yield ctx.getSynonyms ()
                yield ctx.getUDTTs    ()
                yield Seq.empty
            }
            Result.Ok batches
        with
            e -> Result.Error e   
    let initSMO config = 
        let serverVersion = realVersion config.ServerVersion
        let options = SqlScriptOptions.defaults serverVersion
        let connStr = 
            SqlConnectionStringBuilder 
               ( PersistSecurityInfo = true, 
                 DataSource = config.ServerName, 
                 IntegratedSecurity = true, 
                 InitialCatalog = config.DbName
               )      
        let func = (fun _ -> queryAll connStr options)
        {QueryAll = func}
