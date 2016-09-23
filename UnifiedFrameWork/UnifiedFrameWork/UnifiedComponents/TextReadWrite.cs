using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.Controller
{
    public class TextReadWrite
    {
        internal void TextFileWriter(List<string> inputList, string filepath, string fileName)
        {
            try
            {
                using (StreamWriter file =
                new StreamWriter(Path.Combine(filepath, fileName)))
                {
                    if (inputList.Count > 0)
                    {
                        foreach (string line in inputList)
                        {
                            file.WriteLine(line);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal List<string> TextFileReader(string filepath, string fileName)
        {
            try
            {
                List<string> returnList = new List<string>();
                var fileStream = new FileStream(Path.Combine(filepath, fileName), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        returnList.Add(line);
                    }
                }
                return returnList;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}