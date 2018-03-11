namespace ScriptGen.Infra
open System.Diagnostics
open System

module ExternalProcess = begin

    [<Measure>] type millisec
    [<Measure>] type minute

    let private timeout = 1.<minute> //TODO move to config
    let private msPerMin = 60000.<millisec/minute>
    let private milliSecs (t:float<minute>) = (t * msPerMin) |> int
    let private setup pathOfExe workingDir args = 
        ProcessStartInfo
           ( CreateNoWindow = true,
             RedirectStandardError = true,
             RedirectStandardOutput = true,
             UseShellExecute = false,
             FileName = pathOfExe,
             Arguments = args,
             WorkingDirectory = workingDir
           )

    let run pathOfExe workingDir args = 
        let startInfo = setup pathOfExe workingDir args
        let ps = new Process (StartInfo = startInfo)
        let mutable stdout, stderr = [], []
        try
            ps.OutputDataReceived.Add
                (fun args -> stdout <- args.Data :: stdout) 
            ps.ErrorDataReceived.Add
                (fun args -> stderr <- args.Data :: stderr) 
            ps.Start() |> ignore
            ps.BeginErrorReadLine()
            ps.BeginOutputReadLine()
            let obeyedTimeout = ps.WaitForExit (milliSecs timeout)
            if not obeyedTimeout then
               sprintf "Process timed out!\n(%s %s.)" 
                        pathOfExe args 
               |> failwith
            stdout, stderr
        finally
            if not ps.HasExited 
                then ps.Kill()
            ps.Close()
end











(*

https://msdn.microsoft.com/en-us/library/system.diagnostics.process.beginerrorreadline(v=vs.110).aspx

Follow these steps to perform asynchronous read operations on StandardError for a Process :

1.  Set UseShellExecute to false.
2.  Set RedirectStandardError to true.
3.  Add your event handler to the ErrorDataReceived event. 
4.  Start the Process.
5.  Call BeginErrorReadLine for the Process. 
    This call starts asynchronous read operations on StandardError.    

*)