namespace ScriptGen.Infra.SqlServer
module SqlScriptOptions = 
    open Microsoft.SqlServer.Management.Smo
    open Microsoft.SqlServer.Management.Common
    open System.Text
    let defaults serverVersion = 
        ScriptingOptions (
            // General
            AllowSystemObjects = false,
            DdlHeaderOnly = false,
            DdlBodyOnly = false,

            // Here you could disable scripting absolute file paths
            NoFileGroup = true,
            IncludeFullTextCatalogRootPath = true,

            // Output
            ScriptBatchTerminator = true,
            BatchSize = 100,
            NoCommandTerminator = false,
            AnsiFile = false,
            Encoding = Encoding.UTF8,
            ToFileOnly = false,

            // "General" from the SSMS UI
            AnsiPadding = false,
            AppendToFile = true,
            IncludeIfNotExists = false,
            ContinueScriptingOnError = false,
            ConvertUserDefinedDataTypesToBaseType = false,
            WithDependencies = false, //scripts main table including inline dependent tables etc.
            IncludeHeaders = false, //top level comment,
            DriIncludeSystemNames = false,
            SchemaQualify = true,         
            Bindings = false,
            NoCollation = true,
            
            ScriptDrops = false,
            ExtendedProperties = false,
            TargetServerVersion = serverVersion,
            TargetDatabaseEngineEdition = DatabaseEngineEdition.Standard,
            TargetDatabaseEngineType = DatabaseEngineType.Standalone,
            LoginSid = false,
            Permissions = true, //Grants on views etc.

            ScriptOwner = false,
            Statistics = false,
            IncludeDatabaseContext = false, // use database statement(s)
            ScriptSchema = true,
            ScriptData = false, //true,

            // "Table view options" from the SSMS UI
            ChangeTracking = false,
            
            ScriptDataCompression = false,

            PrimaryObject = true,
            Triggers = true,
            

            NoIdentities                   = false, // prints identity declaration on pk column
            IncludeDatabaseRoleMemberships = true,
            NoMailProfilePrincipals        = true,
            NoMailProfileAccounts          = true,
            NoExecuteAs                    = true,
            TimestampToBinary              = false,
            NoFileStreamColumn             = false,
            NoFileStream                   = false,
            NoViewColumns                  = false,
            NoVardecimal                   = false,
            EnforceScriptingOptions        = true,
            OptimizerData                  = false,
            NoXmlNamespaces                = false,
            NoTablePartitioningSchemes     = false,
            NoIndexPartitioningSchemes     = false,

            SchemaQualifyForeignKeysReferences = true,
            FullTextStopLists   = false,
            FullTextCatalogs    = false,
            NoAssemblies        = true,
            AgentAlertJob       = false,
            AgentNotify         = false,
            AgentJobId          = false,
            Default             = true,
            
            DriDefaults         = false,
            DriChecks           = false,
            NonClusteredIndexes = false,
            ClusteredIndexes    = false,
            XmlIndexes          = false,
            DriForeignKeys      = false,
            FullTextIndexes     = false,
            Indexes             = false,
            DriUniqueKeys       = false,
            DriAll              = false,
            DriAllConstraints   = false,
            DriAllKeys          = false,
            DriIndexes          = false,
            DriPrimaryKey       = false,
            DriClustered        = false,
            DriNonClustered     = false,
            DriWithNoCheck      = false
        )