using System.Data.SqlTypes;
using System.Text; 
using System.Text.Json ;
using System.Text.Json.Serialization.Metadata;

namespace CLI {



    /// <summary>
    /// Nothing here must belong to the project itself, only ecapsulate test commands here.
    /// </summary>




    public  class Test {

        public Test(){

            Console.WriteLine($"{typeof(Test).Name} instantiated. Constructer was respected."); 
            //InstanceTracker.TryAddInstance(typeof(Test) , this) ;
        }
        
        [Command("info")]
        public static void GetFileInfo(string filePath)
        {
            if (File.Exists(filePath) || Directory.Exists(filePath))
            {
                FileInfo info = new(filePath);
                StringBuilder builder = new();
                builder.Append('\n');
                builder.AppendLine($"File:\t\t{Path.GetFileName(filePath)}");
                builder.AppendLine($"Full path:\t{Path.GetFullPath(filePath)}");
                builder.AppendLine($"Last accessed:\t{info.LastAccessTime}");
                builder.AppendLine($"Last modified:\t{info.LastWriteTime}");
                builder.AppendLine($"Size:\t\t{info.Length / 1024} KiB");
                Console.WriteLine(builder.ToString());   
            }
            else
            {   
                Console.WriteLine($"{filePath} is not a valid file path.");
            }
        }

    } 

}
