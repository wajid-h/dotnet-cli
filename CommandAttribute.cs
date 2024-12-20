namespace CLI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute(string command__, string help__ = "No description.") : Attribute
    {

        public string Command { get => command; }
        public string Help { get => help; }
        private readonly string command = command__;
        private readonly string help = help__;

    }
}