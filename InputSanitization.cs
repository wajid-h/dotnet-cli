using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace CLI
{
    public class InputSantizer
    {
        public static InputInfo? SantizeInput(string inputString , bool exludeCommand  =  true)
        {

            List<string> tokens = [.. inputString.Split()];

            if(exludeCommand)
            {
                static bool filter(string s) => string.IsNullOrWhiteSpace(s);
                tokens.RemoveAll(filter);
            }
            if(tokens.Count == 0) return  null; 
            string baseCommand = tokens[0];
            tokens.RemoveAt(0); 
            return new(baseCommand, tokens.Count > 0 ? [..tokens] : null);
        }

    }

    public  class InputInfo(string command__, params string[]? args__)
    {
        public string Command { get => command; }
        public string[]? Arguments { get => arguments; }

        private readonly string command = command__;
        private readonly string[]? arguments = args__;


        public ParameterInfo[]  Params {get =>  parameters; } 
        private ParameterInfo[] parameters = [] ;

        public void LoadParams(ParameterInfo[] parameters){
            this.parameters = parameters ;
        }

        public object[] FilterOptionalArgs(MethodInfo  targetMethod) {
            List<object?>  finalParams = [] ; 
            MethodInfo subroutine = targetMethod; 
            List<ParameterInfo> parameters = [..subroutine.GetParameters()] ;
            List<ParameterInfo> optionalParameters =  parameters.FindAll(query =>  query.IsOptional) ;            

            for (int i = 0 ; i <  parameters.Count ; i++ )
            {
                if(parameters[i].IsOptional && Arguments?[i] == null)
                {
                    Type paramType =  parameters[i].ParameterType;
                    finalParams.Add(default) ;
                }else {
                    finalParams.Add(Arguments?[i]); 
                }
            }
            return [finalParams]; 

        }
        public string JoinArguments(){
            if(arguments  ==  null) return string.Empty; 
            StringBuilder builder =  new();
            foreach(string arg in arguments) 
               builder.Append(arg);
            return builder.ToString();

        }
    }
}

