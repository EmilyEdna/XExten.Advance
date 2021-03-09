using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Synctool.EmitFramework
{
    public class TypeCreator
    {
        /// <summary>
        ///  Instance
        /// </summary>
        public static TypeCreator Instance = new Lazy<TypeCreator>().Value;

        public dynamic CreateAssembly(string AssemblyName) 
        {

            AssemblyName assembly = new AssemblyName(AssemblyName);

            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.RunAndCollect);

            builder.DefineDynamicModule(AssemblyName);

        }

    }
}
