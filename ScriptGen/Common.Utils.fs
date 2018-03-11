namespace ScriptGen.Common
module Utils =
    open System

    let concat strs = 
        strs 
        |> Seq.cast<string> 
        |> String.concat Environment.NewLine

    let notBlank = String.IsNullOrWhiteSpace >> not

    let (>>=) result func = 
        Result.bind func result 

    ///<summary>'on success continue with'</summary>
    let onOK = Result.bind

    ///<summary>'on success continue with'</summary>
    let (=>) f1 f2 = f1 >> Result.bind f2