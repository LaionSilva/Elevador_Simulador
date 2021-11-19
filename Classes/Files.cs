using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elevador_Simulador.Classes
{
    class Files
    {
        public static void ReadFile(string path, ref string[] lines)
        {
            try
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            catch (FileNotFoundException) { } 
            catch (Exception) { }
        }

        public static void WriteFile(string path, string[] lines)
        {
            WriteFile(path, lines.Length > 0 ? String.Join("\n", lines) : "");
        }

        public static void WriteFile(string path, string text = "")
        {
            try
            {
                System.IO.File.WriteAllText(path, text);
            }
            catch (FileNotFoundException) { }
            catch (Exception) { }
        }
    }
}
