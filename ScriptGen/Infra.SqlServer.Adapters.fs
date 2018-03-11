namespace ScriptGen.Infra.SqlServer

module SqlAdapters = 
    open Microsoft.SqlServer.Management.Smo
    open ScriptGen.Domain.Models
    open ScriptGen.Common.Utils
        
    let private toSchema so (obj:Schema) = {
        Type = SqlType.Schema
        Schema = None
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }    
    
    let private toTable so (obj:Table) = {
        Type = SqlType.Table
        Schema = Some obj.Schema
        Name  = obj.Name; 
        ScriptTxt = so |> obj.Script |> concat 
    }
    
    let private toView so (obj:View) = {
        Type = SqlType.View
        Schema = Some obj.Schema
        Name  = obj.Name;
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toSproc so (obj:StoredProcedure) = {
        Type = SqlType.Sproc
        Schema = Some obj.Schema
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toFunc so (obj:UserDefinedFunction) = {
        Type = SqlType.Func
        Schema = Some obj.Schema
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }    
    
    let private toConstraint so schema (obj:DefaultConstraint) = {
        Type = SqlType.Constraint
        Schema = Some schema
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toConstraint2 so schema (obj:Check) = {
        Type = SqlType.Constraint
        Schema = Some schema
        Name  = obj.Name;
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toFKey so schema (obj:ForeignKey) = {
        Type = SqlType.FKey
        Schema = Some schema
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toUser so (obj:User) = {
        Type = SqlType.User
        Schema = None
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toRole so (obj:DatabaseRole) = {
        Type = SqlType.Role
        Schema = None
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }

    let private toIndex so schema (obj:Index) = {
        Type = if obj.IsClustered then SqlType.PKey else SqlType.Index
        Schema = Some schema
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toSynonym so (obj:Synonym) = {
        Type = SqlType.Synonym
        Schema = Some obj.Schema
        Name  = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }
    
    let private toUDTT so (obj:UserDefinedTableType) = {
        Type = SqlType.UDTT
        Schema = Some obj.Schema
        Name = obj.Name
        ScriptTxt = so |> obj.Script |> concat
    }
    
    type Scriptor = {SO: ScriptingOptions}
    with
    member x.toSchema      = toSchema      x.SO
    member x.toTable       = toTable       x.SO
    member x.toView        = toView        x.SO
    member x.toSproc       = toSproc       x.SO
    member x.toFunc        = toFunc        x.SO
    member x.toConstraint  = toConstraint  x.SO
    member x.toConstraint2 = toConstraint2 x.SO
    member x.toFKey        = toFKey        x.SO
    member x.toUser        = toUser        x.SO
    member x.toRole        = toRole        x.SO
    member x.toIndex       = toIndex       x.SO
    member x.toSynonym     = toSynonym     x.SO
    member x.toUDTT        = toUDTT        x.SO