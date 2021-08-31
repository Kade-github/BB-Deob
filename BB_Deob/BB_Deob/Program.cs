using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib;
using dnlib.DotNet;
using System.Text.RegularExpressions;

namespace BB_Deob
{
    class Program
    {
        static void Main(string[] args)
        {
            ModuleDef module;
            try
            {
                module = ModuleDefMD.Load(args[0]);
            }
            catch
            {
                Console.WriteLine("No arguments added!");
                return;
            }
            Console.WriteLine("Loaded module " + module.Name + ", Runtime: " + module.RuntimeVersion);
            string bb_version = "unknown/bb obfuscator not found";
            foreach (TypeDef type in module.Types)
            {
                if (type.Name.Contains("__BB_OBFUSCATOR_VERSION_"))
                {
                    bb_version = type.Name.String.Split('_')[5] + "." + type.Name.String.Split('_')[6] + "." + type.Name.String.Split('_')[7];
                }
            }
            Console.WriteLine("BB Obfuscator Version: " + bb_version);
            Console.WriteLine("[1] Renaming...");
            int count = 0;
            int classNumber = 0;
            Regex regex = new Regex(@"[a-zA-Z0-9_]+");
            foreach (TypeDef type in module.Types)
            {
                if (!type.Name.Contains("__BB_OBFUSCATOR_VERSION_") || !type.Name.Contains("ArrayCopy"))
                {
                    if (type.Name != "<Module>")
                    {
                        if (!regex.IsMatch(type.Name))
                        {
                            type.Name = "Class" + classNumber;
                            ++classNumber;
                        }
                        int fieldNumber = 0;
                        foreach (FieldDef field in type.Fields)
                        {
                            if (!regex.IsMatch(field.Name))
                            {
                                field.Name = "Field" + fieldNumber;
                                ++fieldNumber;
                            }
                        }
                        int paramNumber = 0;
                        int methodNumber = 0;
                        foreach (MethodDef method in type.Methods)
                        {
                            foreach (ParamDef param in method.ParamDefs)
                            {
                                if (!regex.IsMatch(param.Name))
                                {
                                    param.Name = "Param" + paramNumber;
                                    ++paramNumber;
                                }
                            }
                            if (!regex.IsMatch(method.Name))
                            {
                                method.Name = "Method" + methodNumber;
                                ++methodNumber;
                            }
                        }
                    }
                }
                count++;
                Console.WriteLine("[1] Renaming [" + count + "/" + module.Types.Count + "]");
            }
            Console.WriteLine("[2] Watermarking...");
            module.Name += "_renamed";
            Console.WriteLine("[3] Saving...");
            module.Write(module.FullName.Split('.')[0] + "-Renamed.dll");
            Console.WriteLine("Saved to " + Directory.GetCurrentDirectory() + @"\" + module.FullName.Split('.')[0] + "-Renamed.dll");
        }
    }
}
