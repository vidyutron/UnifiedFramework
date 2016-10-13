using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedFrameWork.UnifiedComponents;

namespace UnifiedFrameWork.Controller
{
   public class UComponentController
    {
        //private CSVReadWrite csvReadWrite;
        private DBHandler dbHandler;
        private DirectoryHandler directoryHandler;
        private ExcelReadWrite excelReadWrite;
        private SendMail sendMail;
        private TextReadWrite textReadWrite;
        private XmlReadWrite xmlReadWrite;

        public DataTable SqlSelectQuery(string connectionString,string selectQuery)
        {
            this.dbHandler = new DBHandler();
            return dbHandler.SqlSelectQuery(connectionString, selectQuery);
        }
        public DataTable SqlFilterByIdQuery(string connectionString, string filterQuery,int id)
        {
            this.dbHandler = new DBHandler();
            return dbHandler.SqlFilterByIdQuery(connectionString, filterQuery,id);
        }
        public DataTable SqlSelectFilterQuery(string connectionString,Dictionary<string,string>filterCondition,string sqlSelectFilterQuery)
        {
            this.dbHandler = new DBHandler();
            return dbHandler.SqlSelectFilterQuery(connectionString,filterCondition,sqlSelectFilterQuery);
        }
        public bool SqlInsertQuery(string connectionString,Dictionary<string, string> insertList, string sqlInsertQuery)
        {
            this.dbHandler = new DBHandler();
            return dbHandler.SqlInsertQuery(connectionString, insertList,sqlInsertQuery);
        }
        public bool SqlUpdateQuery(string connectionString,Dictionary<string, string> updateList, string sqlUpdateQuery)
        {
            this.dbHandler = new DBHandler();
            return dbHandler.SqlUpdateQuery(connectionString,sqlUpdateQuery,updateList);
        }

        public void ExcelWriter(string filepath, string filename, List<string> values, string delimiter="-!-")
        {
            excelReadWrite = new ExcelReadWrite();
            excelReadWrite.ExcelWriter(filepath, filename, values, delimiter);
        }
        public List<string> ExcelReader(string filepath, string filename,
            string sheetName, UnifiedCleanser cleanser = UnifiedCleanser.Default, string delimitor = "-!-")
        {
            excelReadWrite = new ExcelReadWrite();
            return excelReadWrite.ExcelReader(filepath, filename, sheetName, cleanser, delimitor);
        }

        public void SendMail(string mailFrom, List<string> mailToList,
            string smtpClient, string subjectLine, string mailBody, int portNumber,
            List<string> attachmentList, string credentialUser, string credentialPass)
        {
            sendMail = new SendMail();
            sendMail.CreateEmail(mailFrom, mailToList, smtpClient, subjectLine,
                mailBody, portNumber, attachmentList, credentialUser, credentialPass);
        }

        public void TextWriter(List<string> inputList, string filepath, string fileName)
        {
            textReadWrite = new TextReadWrite();
            textReadWrite.TextFileWriter(inputList, filepath, fileName);

        }
        public List<string> TextReader(string filepath, string fileName)
        {
            textReadWrite = new TextReadWrite();
            return textReadWrite.TextFileReader(filepath,fileName);

        }

        public void CSVWriter(string filePath,string fileName,List<List<string>> dataList)
        {
            // Write sample data to CSV file
            using (CsvFileWriter writer = new CsvFileWriter(Path.Combine(filePath,fileName+".csv")))
            {
                for (int i = 0; i < dataList.Count; i++)
                {
                    CsvRow row = new CsvRow();
                    for (int j = 0; j <dataList[i].Count; j++)
                        row.Add(dataList[i][j].Trim());
                    writer.WriteRow(row);
                }
            }
        }

        public List<string> ReadTest(string filePath,string fileName)
        {
            // Read sample data from CSV file
            List<string> readList = new List<string>();
            using (CsvFileReader reader = new CsvFileReader(Path.Combine(filePath, fileName + ".csv")))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    foreach (string s in row)
                    {
                        readList.Add(s);
                    }
                }
            }
            return readList;
        }

        public void XmlWriter(string filePath, string fileName, string subRootNode)
        {
            xmlReadWrite = new XmlReadWrite();
            xmlReadWrite.XmlWriter(filePath, fileName, subRootNode);
        }
        //Incomplete
        public void XmlReader(string filePath, string fileName, string subRootNode)
        {
            xmlReadWrite = new XmlReadWrite();
            xmlReadWrite.XmlWriter(filePath, fileName, subRootNode);
        }
    }
}
