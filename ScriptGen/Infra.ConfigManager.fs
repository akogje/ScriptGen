namespace ScriptGen.Infra

open ScriptGen.Domain.Models
open System.Configuration

module ConfigManager =
    
    type Cfg = 
         | ServerVersion 
         | ServerName 
         | DbName
         | WorkingDirPath
         | ScmToolPath
    with
    member x.Value = 
        ConfigurationManager.AppSettings.Get (x.ToString())
    end
    
    let initConfig () = {
        ServerVersion = int Cfg.ServerVersion.Value
        ServerName = Cfg.ServerName.Value
        DbName = Cfg.DbName.Value
        WorkingDirPath = Cfg.WorkingDirPath.Value
        ScmToolPath = Cfg.ScmToolPath.Value
    }

