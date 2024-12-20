using System.Reflection;

namespace  CLI{
    public class CommandInfo     (MethodInfo method__ ,  Type  baseType__ , string alias__,  string? help__) {

    
        public string Alias {get => alias; }
        private readonly string  alias =  alias__ ;        

        public string? CommandHelp {get =>  help; } 
        private readonly string? help =  help__; 


        public MethodInfo Method  {get =>  method; }
        public Type Type {get =>  type ; }  
        private readonly  MethodInfo method =  method__  ; 
        private readonly Type  type =  baseType__; 


    }

}

