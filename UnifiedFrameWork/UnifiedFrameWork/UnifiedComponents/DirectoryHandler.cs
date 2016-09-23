using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.Controller
{
    public class DirectoryHandler
    {
        //mode- 'Default'- create directory in Solution's path
        //mode- 'Custom'- create directory in the location passed in argument
        //filePath- for default mode user may supply parent folder name to be created, in custom mode complete path needs to be provided
        public static string DirectoryCreation(string filePath, string mode = "default")
        {
            try
            {
                string defaultFilepath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                string completeFilePath = Path.Combine(defaultFilepath, filePath);
                if (mode.ToLower().Equals("default") && !string.IsNullOrEmpty(filePath))
                {
                    if (!Directory.Exists(completeFilePath))
                        Directory.CreateDirectory(completeFilePath);
                    return completeFilePath;
                }
                else
                {
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    return filePath;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public static void DirectoryCleanUp(string filePath, string filePattern = "")
        {
            try
            {
                if (string.IsNullOrEmpty(filePattern))
                {
                    string[] filePresence = Directory.GetFiles(filePath);
                    if (filePresence.Length >= 1)
                    {
                        foreach (string singleFile in filePresence)
                            File.Delete(singleFile);
                    }
                }
                else
                {
                    string[] filePresence = Directory.GetFiles(filePath, filePattern);
                    if (filePresence.Length >= 1)
                    {
                        foreach (string singleFile in filePresence)
                            File.Delete(singleFile);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
