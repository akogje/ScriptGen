namespace ScriptGen.Application   

open ScriptGen.Domain.Models
open ScriptGen.Infra
open ScriptGen.Domain

module AppHost = begin

    let private resolveAll () = 
        let config = ConfigManager.initConfig() 
        {   Config = config
            DB = SqlServer.Setup.initSMO config  
            Disk = DiskIO.initFileRW config       
            Git = SCM.initSCM config  
        }
    
    let setup () = {
        Run = resolveAll >> Workflow.run
    }
end