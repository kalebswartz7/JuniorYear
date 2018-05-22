using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

// Author: Kaleb Swartz 
// This program reads in two files, one containing the blueprint for which the record file should be created and the other containing
//the data for which the record file should be populated. It then creates a new file with the data organized in the specified manner.

namespace hw4
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            MainClass m = new MainClass();
            Dictionary<string, string> data = new Dictionary<string, string>();
            String tsvName = args[0];
            String tmpName = args[1];
            m.parseFirst(data, tsvName, tmpName);
        }

        public void parseFirst(Dictionary<string, string> d, String inFileName, String outFileName)
        {
            String[] lines = System.IO.File.ReadAllLines(inFileName);
            String line = lines[0];
            String data = "";
            int position = 0;
            for (int j = 0; j < line.Length; j++) {
                if (!line[j].Equals('\t')) {
                    data += line[j];
                }
                else
                {
                d.Add(data, position.ToString());
                    data = "";
                    position++;
                }
                if (j == line.Length - 1) {
                    d.Add(data, position.ToString());
                    data = "";
                    position++;
                }

            }
            for (int i = 1; i < lines.Length; i++) {
                var copied = new Dictionary<string, string>(d);
                fillDict(copied, inFileName, i, outFileName);
            }
        }

        public Dictionary <string, string> fillDict(Dictionary<string, string> d2, String fileName, int lineNum, String outFileName) {
            String[] lines = System.IO.File.ReadAllLines(fileName);
            int position = 0;
            String data = "";
            String line = lines[lineNum];
            for (int j = 0; j < line.Length; j++) {
                if ((!line[j].Equals('\t')) && j != line.Length - 1) {
                    data += line[j];
                }
                else {
                    if (j == line.Length - 1) {
                        data += line[j];
                    }
                    List<string> keys = new List<string>(d2.Keys);
                    foreach (String key in keys)
                    {
                        String value = d2[key];
                        if (value.Equals(position.ToString()))
                        {
                            d2[key] = data;
                        }
                    } 
                    position++;
                    data = "";
                }
            }
            replaceTSV(d2, outFileName);

            return d2;
        }

        public void replaceTSV(Dictionary<string, string> d, String outFileName)
        {
            String filetoCreate = "";
            List<string> keys2 = new List<string>(d.Keys);
            foreach (String key in keys2)
            {
                if (key.Equals("ID"))
                {
                    filetoCreate = d[key] + ".txt";
                }
            }

            String[] lines = System.IO.File.ReadAllLines(outFileName);
            String finalText = "";
            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];
                StringBuilder builder = new StringBuilder(line);
                String replace = "<<";
                for (int j = 0; j < builder.Length - 1; j++)
                {
                    if (builder[j] == '<' && builder[j + 1] == '<')
                    {
                        while (builder[j + 2] != '>')
                        {
                            replace += builder[j + 2];
                            j++;
                        }
                        replace += ">>";

                        List<string> keys = new List<string>(d.Keys);
                        foreach (String key in keys)
                        {
                            if (("<<" + key + ">>").Equals(replace))
                            {
                                builder.Replace(replace, d[key]);
                                replace = "<<";
                            }

                        }
                    }

                }
                finalText += builder.ToString() + "\n";
                //File.AppendAllText(filetoCreate, builder.ToString() + "\n");

            }
            File.WriteAllText(filetoCreate, finalText);
        }
            

        public void printDict(Dictionary<string, string> d) {
            foreach (KeyValuePair<string, string> kvp in d)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                    kvp.Key, kvp.Value);
            }
        }
    }
}
