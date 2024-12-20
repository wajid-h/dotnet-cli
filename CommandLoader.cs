using System.Reflection;
namespace CLI
{
    public class CommandLoader
    {

        public static event Action<Dictionary<string, CommandInfo>>? OnReady;

        public Dictionary<string, CommandInfo>? Commands { get => commands; }
        private readonly Dictionary<string, CommandInfo> commands = [];


        // Enforce loading commands before anything is done on the loader
        public CommandLoader()
        {
            Console.WriteLine("Initializing command registery");
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            var types = executingAssembly.GetTypes();

            foreach (var type in types)
            {
                var attribMethods = type.GetMethods(

                    BindingFlags.NonPublic |
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.Instance

                ).Where(
                    command => command.GetCustomAttribute<CommandAttribute>() != null
                );

                foreach (var command in attribMethods)
                {
                    try
                    {

#pragma warning disable CS8600, CS8602

                        CommandAttribute attributeInfo = command.GetCustomAttribute<CommandAttribute>();
                        commands.Add(
                            attributeInfo.Command,
                            new CommandInfo( command, type  , attributeInfo.Command,  attributeInfo.Help) 
                        );
#pragma warning restore CS8600, CS8602

                    }
                    catch (ArgumentException EX)
                    {
                        Console.WriteLine($"ERROR: Failed to load command '{command.Name}'\n[Stack Trace]\n{EX.StackTrace}");
                    }
                }
            }
            OnReady?.Invoke(commands);
        }


    }
}





