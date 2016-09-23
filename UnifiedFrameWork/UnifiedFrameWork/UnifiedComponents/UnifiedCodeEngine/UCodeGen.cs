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
        public static List<string> FilesToInclude;
        public static void Initiate()
        {
            FilesToInclude = new List<string>();
        }

        public static void Finalise()
        {
            //AutomateAddFile.IncludeInProject(FilesToInclude);
            if (FilesToInclude != null && FilesToInclude.Count > 0)
            {
                var projFilePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                var p1 = new Microsoft.Build.Evaluation.Project(Path.Combine(projFilePath,
                    Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().Location).Replace(".dll", "") + ".csproj"));
                foreach (string filePath in FilesToInclude)
                {
                    p1.AddItem("Compile", filePath + ".cs");
                }
                p1.Save();
            }
            else
            { Console.WriteLine("Either Unified Code Generation Intiate functionality was not used or no files were generated to include in Project"); }

        }
    }
}
