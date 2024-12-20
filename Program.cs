
namespace CLI {


    public class Exec { 

        public static int Main(params string[] args) {
            
            CommandLoader.OnReady +=  RunCommandLine ;
            try{
                CommandLoader loader =  new ();
                return (int) ExitCode.Error ;
            }
            catch(Exception) {
            return (int) ExitCode.Success; 
            }
        }   


        static void RunCommandLine(Dictionary<string, CommandInfo> commands){

            CommandLine commandLine  =  new(commands); 
            commandLine.Enter();

        }
        enum ExitCode {
            Success  = 0 ,  
            Error = 1,
            GoofedUp =  -1
        }
    }
}