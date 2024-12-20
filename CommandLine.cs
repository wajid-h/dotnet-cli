#pragma warning disable  IDE

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace CLI
{
    public class CommandLine
    {
        public Dictionary<string, CommandInfo> Commands;

        public CommandLine(Dictionary<string, CommandInfo> commandLineContext)
        {
            InstanceTracker.AddInstance(typeof(CommandLine), this);
            Commands = commandLineContext;
        }


        public void Enter()
        {
            // for the lifetime of program. 
            while (true)
            {
                Console.Write("$");
                string? inp = Console.ReadLine();
                if (!string.IsNullOrEmpty(inp))
                {
                    InputInfo? input = InputSantizer.SantizeInput(inp);
                    RunCommand(input);
                }
            }
        }

        private void RunCommand(InputInfo? userInput)
        {
            if (userInput == null) return;
            try
            {
                CommandInfo? targetCommand = Commands.First(query => query.Key == userInput.Command).Value;
                if (targetCommand != null)
                {
                    RunAppCommand(userInput, targetCommand); 
                }

            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"exec_error: '{userInput.Command}' is not a valid command name.");
            }

        }



        public void RunAppCommand(InputInfo userInput, CommandInfo targetCommand)
        {
            MethodInfo subroutine = targetCommand.Method;
            List<ParameterInfo> parameters = [..subroutine.GetParameters()] ;
            List<ParameterInfo> optionalParameters =  parameters.FindAll(query =>  query.IsOptional) ;
            if(userInput.Arguments?.Length < parameters.Count  )
            parameters.RemoveAll(query  =>  query.IsOptional) ;

            try
            {
                bool useInstance = !subroutine.IsStatic;
                object? targetInstance = useInstance ? InstanceTracker.GetInstance(targetCommand.Type) : null;
                if (userInput.Arguments != null && parameters != null)
                {
                    object[]? filteredArgs = ConvertArgTypes(userInput.Arguments, subroutine);
                                        
                    subroutine.Invoke(targetInstance ?? null, filteredArgs);
                    return;
                }
                else subroutine.Invoke(targetInstance ?? null, null);

            }
            catch (TargetParameterCountException)
            {
                LogParamMismatchErr(userInput, targetCommand);
                PrintCommandHelp(userInput.Command);
            }
        }

        private static void RunShellCommand(string args , string shellName){

            bool bash =  shellName.Equals("bash", StringComparison.OrdinalIgnoreCase)  ; 
            string shellExitCommand =  bash? "" : "/C";

            ProcessStartInfo processInfo = new()
            {
                FileName = $"{shellName}",
                Arguments = $@"{shellExitCommand} {args}",
                WindowStyle =  ProcessWindowStyle.Hidden ,
                RedirectStandardOutput =  true,
                RedirectStandardError =  true , 
                RedirectStandardInput =  false
            };
            
            Process? process = Process.Start(processInfo) ;
            string? stdout =  process?.StandardOutput.ReadToEnd();
            string? stderr  =  process?.StandardError.ReadToEnd();
            
            if(!string.IsNullOrEmpty(stdout))
                Console.WriteLine(stdout);
            if(!string.IsNullOrEmpty(stderr)){
                Console.WriteLine(stderr);
            }
            
            process?.WaitForExit();
            
        }       

        
        [Command("shell", "<shell-name>  starts a headless integrated target shell process.")]
        private static void ShellCommandPassthrough(string targetShell){       
             
            while (true)
            {   
                Console.Write($"[shell@{targetShell}]$");
                string? inp = Console.ReadLine();
                if (!string.IsNullOrEmpty(inp))
                {
                    RunShellCommand(inp,  targetShell);
                    if(inp.Equals("exit" , StringComparison.OrdinalIgnoreCase)) {
                        break; 
                    }
                }
            }
        }
        
        static void LogParamMismatchErr(InputInfo userInput, CommandInfo targetCommand)
        {   


            var subroutine = targetCommand.Method;
            string[]? args = userInput.Arguments;
            int passedArgCount = args != null ? args.Length : 0;
            int requiredCount = subroutine.GetParameters().Length;
            if (passedArgCount == requiredCount) return; // My exceptions hallucinate smh
            string? callName = targetCommand.Method.GetCustomAttribute<CommandAttribute>()?.Command;
            Console.WriteLine($"exec_error: {callName} takes {requiredCount} required parameter(s), {passedArgCount} arguments were passed.");

        }

        [Command("num", "num<int> prints the same shit back to you.")]
        static void T(int cli , int shi =  default)
        {
            Console.WriteLine(cli + shi);
        }

        static object[] ConvertArgTypes(string[] cliArgs, MethodInfo targetMethod)
        {

            List<object> args = [];

            var methodParams = targetMethod.GetParameters();
            var passedArgs = cliArgs;

            if (methodParams.Length != passedArgs.Length)
                return [.. args];

            for (int i = 0; i < passedArgs.Length; i++)
            {
                ParameterInfo parameterInfo = methodParams[i];

                try
                {
                    object? obj = TypeDescriptor.GetConverter(parameterInfo.ParameterType).ConvertFromString(passedArgs[i]);
                    args.Add(obj ?? passedArgs[i]);
                }

                catch (NotSupportedException)
                {
                    Console.WriteLine($"exec_error: Argument could not be converted into smart object.");
                    break;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"interp_error:{passedArgs[i]}  is not a valid value for type {parameterInfo}");
                }
            }

            return [.. args];
        
        }


        [Command("clear", "Clears the console")]
        private static void Clear() => Console.Clear();



        private bool PrintCommandHelp(string commandName)
        {
            try
            {
                CommandInfo command = Commands.First(query => query.Key == commandName).Value;
                Console.WriteLine($"\n[{command.Alias}]\t{command.CommandHelp}");
                return true;
            }
            catch (Exception)
            {
                if(commandName  != "-all")
                Console.WriteLine($"inter_error: opereation failed, help target is not recoginzed.");
                return false;
            }
        }

        //  please dont mind ;)


        [Command("help", "<function-name> to get info about function.\n<-all> to describe entire command registery.")]
        private void HelpTargeted(string target)
        {
            bool func_exists = PrintCommandHelp(target);
            if (!func_exists && target.Equals("-all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var command in Commands)
                    PrintCommandHelp(command.Key);
            }
        }


    }

}

#pragma warning restore IDE