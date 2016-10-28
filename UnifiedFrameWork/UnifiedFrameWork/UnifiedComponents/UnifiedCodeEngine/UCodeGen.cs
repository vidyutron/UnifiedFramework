using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.UCodeGenerator
{
    public class UCodeGen
    {
        public static Dictionary<string,string> FilesToInclude { get; set; }
        public static void Initiate()
        {
            FilesToInclude = new Dictionary<string, string>();
        }

        public static void Finalise()
        {
            //AutomateAddFile.IncludeInProject(FilesToInclude);
            if (FilesToInclude != null && FilesToInclude.Count > 0)
            {
                var projFilePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                var p1 = new Microsoft.Build.Evaluation.Project(Path.Combine(projFilePath,
                    Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().Location).Replace(".dll", "") + ".csproj"));
                foreach (var filePath in FilesToInclude)
                {
                    if (filePath.Value.ToLower().Contains("compile"))
                        p1.AddItem(filePath.Value, filePath.Key + ".cs");
                    else
                        p1.AddItem(filePath.Value, filePath.Key, new Dictionary<string, string> { { "CopyToOutputDirectory", "PreserveNewest" } });
                }
                p1.Save();
            }
            else
            { Console.WriteLine("Either Unified Code Generation Intiate functionality was not used or no files were generated to include in Project"); }

        }
    }
}
