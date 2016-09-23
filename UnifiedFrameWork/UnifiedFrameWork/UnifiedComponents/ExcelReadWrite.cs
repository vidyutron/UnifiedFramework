using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace UnifiedFrameWork.Controller
{
    class ExcelReadWrite
    {
        static Excel.Application xlApp = new Excel.Application();
        static Excel.Workbook xlWorkBook;
        static Excel.Worksheet xlWorkSheet;
        static Excel.Range xlRange;
        static object misValue = System.Reflection.Missing.Value;

        internal void ExcelWriter(string filepath, string filename, List<string> values
            , string delimiter="-!-")
        {
            string completeFilePath = Path.Combine(filepath,filename);
            for (int localRCount = 1; localRCount <= values.Count; localRCount++)
            {
                int delimterCount = Regex.Matches(values[localRCount - 1], delimiter).Count;
                for (int localCCount = 1; localCCount <= delimterCount; localCCount++)
                {
                    xlWorkSheet.Cells[localRCount, localCCount] = values[localCCount];
                }
            }
            xlWorkBook.SaveAs(completeFilePath + ".xlsx", Excel.XlFileFormat.xlWorkbookNormal,
                misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlExclusive,
                misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            ReleaseObject(xlWorkSheet);
            ReleaseObject(xlWorkBook);
            ReleaseObject(xlApp);
        }

        internal List<string> ExcelReader(string filepath, string filename,string sheetName,string delimitor="-!-")
        {
            string completeFilePath = Path.Combine(filepath, filename);
            List<string> readList = new List<string>();
            try
            {
                xlWorkBook = xlApp.Workbooks.Open(completeFilePath);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Sheets.get_Item(sheetName);
                xlRange = xlWorkSheet.UsedRange;
                int iRowCount = xlRange.Rows.Count;//number of rows present with data
                int iColCount = xlRange.Columns.Count;//numberr of colums present with data

                // The logic to iterate depends on the scenario,
                // it can be either [iRowCount]-Row dependent, or [iColCount]-Column Dependednt
                for (int i = 1; i <= iRowCount; i++)
                {
                    string tempString = string.Empty;
                    for (int k = 1; k <= iColCount; k++)
                    {
                        tempString = tempString + Convert.ToString(xlWorkSheet.Cells[i, k].Value2)+ delimitor;
                    }
                    readList.Add(tempString);
                }
                ExcelCleanUp();
                return readList;

            }
            catch (Exception ex)
            {
                //logging mechanism
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        internal static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                //logging mechanism
                Console.WriteLine(ex.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        internal void ExcelCleanUp()
        {
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //  Thumb Rule for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorkSheet);

            //close and release
            xlWorkBook.Close();
            Marshal.ReleaseComObject(xlWorkBook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
