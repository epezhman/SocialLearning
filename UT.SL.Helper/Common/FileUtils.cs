using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.SL.Helper
{
    public class FileUtils
    {
        public static string[] DangerousExtensionArray = new string[]{".386",".ADE",".ADP",".ADT",".APP",".ASP",".BAS",
                    ".BAT",".BIN",".BTM",".CBT",".CHM",".CLA",".CLASS",".CMD",".COM",".CPL",".CRT",".CSC",
                    ".CSS",".DLL",".DOT",".EML",".EMAIL",".EXE",".FON",".HLP",".HTA",".HTM",".HTML",
                    ".INF",".INI",".INS",".ISP",".JS",".JSE",".LIB",".LNK",".MDB",".MDE",".MHT",".MHTM",
                    ".MHTML",".MSO",".MSC",".MSI",".MSP",".MST",".OBJ",".OBJ",".OV?",".PCD",".PGM",".PIF",
                    ".PRC",".REG",".SCR",".SCT",".SHB",".SHS",".SMM",".ASM",".C",".CPP",".PAS",".BAS",".FOR",
                    ".ASP",".ASPX",".CS",".SYS",".URL",".VB",".VBE",".VBS",".VXD",".WSC",".WSF",".WSH",
                    ".XL?",".IATA",".IMDG",".h"};

        /// <summary>
        /// Reads a file and return the whole text .
        /// </summary>
        /// <param name="strFilePath">Whole path to the file and file name and extension</param>
        /// <returns></returns>
        public static string ReadTextFile(string strFilePath)
        {
            StreamReader rd;
            string strOutput = "";

            rd = File.OpenText(strFilePath);

            while (rd.Peek() != -1)
                strOutput += rd.ReadLine();

            rd.Close();

            return strOutput;
        }
    }
}
