using BearBuildTool.Tools;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BearBuildTool.Projects
{
    class Compiler
    {
        private static Dictionary<string, Assembly> Assemblys = null;
        public static Assembly CompilerAndLoad(string[] source, string out_name)
        {
            if(Assemblys == null)
                Assemblys = new Dictionary<string, Assembly>();
            if (Assemblys.ContainsKey(out_name)) return Assemblys[out_name];
            if (source == null || source.Length == 0)
            {
                throw new Exception("База проектов пуста!!!");
            }
            ListFiles listFiles = new ListFiles(out_name + ".txt");
            if (!FileSystem.ExistsFile(out_name)|| listFiles.TestUpdate(source)|| listFiles.GetFilesMaxDate()> FileSystem.GetLastWriteTime(out_name))
            {
                listFiles.ClearFiles();
                listFiles.Files.AddRange(source);
                listFiles.Write();
                return Compile(source, out_name);
            }
            else
            {
                var result =  Assembly.LoadFrom(out_name);
                Assemblys.Add(out_name, result);
                return result;
            }
        }
        private static Assembly Compile(string[] source, string out_name)
        {
            if(FileSystem.ExistsFile(out_name)) File.Delete(out_name);
            Console.WriteLine(String.Format("Сборка {0}", out_name));
            if (source == null || source.Length == 0)
            {
                throw new Exception("База проектов пуста!!!");
            }
            File.WriteAllLines(out_name + ".txt", source);
            CompilerParameters op = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = out_name,
                IncludeDebugInformation = false,
                GenerateInMemory = false,
                WarningLevel = 3,
                TreatWarningsAsErrors = false
            };
            op.ReferencedAssemblies.Add("System.dll");
            op.CompilerOptions = "/optimize";
            op.TempFiles = new TempFileCollection();

            var asm = Assembly.GetExecutingAssembly();
            op.ReferencedAssemblies.Add(asm.Location);

            CompilerResults results;
            try
            {
                var ProviderOptions = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };
                var Compiler = new CSharpCodeProvider(ProviderOptions);
                results = Compiler.CompileAssemblyFromFile(op, source.ToArray());
            }
            catch (Exception Ex)
            {
                throw new Exception(String.Format("Неудалось запустить кампилятор файла {0}", out_name), Ex);
            }


            if (results.Errors.Count != 0)
            {
                Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");
                foreach (var error in results.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
                Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка компиляции {0}", out_name));
            }
            Assembly asmOut = results.CompiledAssembly;
            if (asmOut == null)
            {
                throw new Exception(String.Format("Неудалось открыть Assembly в {0}", out_name));
            }
            op.TempFiles.Delete();
            return asmOut;
        }
    }
}
