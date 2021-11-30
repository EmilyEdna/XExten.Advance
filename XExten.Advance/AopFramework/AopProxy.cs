using XExten.Advance.AopFramework.AopAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.IO;

namespace XExten.Advance.AopFramework
{
    /// <summary>
    /// Aop
    /// </summary>
    public class AopProxy
    {
        private static readonly string[] IgnoreMethodName = new[] { "GetType", "ToString", "GetHashCode", "Equals" };
        /// <summary>
        /// 以接口的性质创建代理
        /// </summary>
        /// <typeparam name="TProxyInterface"></typeparam>
        /// <typeparam name="TImplement"></typeparam>
        /// <returns></returns>
        public static TProxyInterface CreateProxyOfRealize<TProxyInterface, TImplement>()
        {
            return CreateProxy<TProxyInterface, TImplement>();
        }
        /// <summary>
        /// 以自身的性质创建代理
        /// </summary>
        /// <typeparam name="TProxyClass"></typeparam>
        /// <returns></returns>
        public static TProxyClass CreateProxyOfInherit<TProxyClass>()
        {
            return CreateProxy<TProxyClass, TProxyClass>();
        }
        /// <summary>
        /// 以接口的性质创建代理
        /// </summary>
        /// <param name="ProxyInterface"></param>
        /// <param name="Implement"></param>
        /// <returns></returns>
        public static object CreateProxyOfRealize(Type ProxyInterface, Type Implement)
        {
            return CreateProxy(ProxyInterface, Implement);
        }
        /// <summary>
        /// 以自身的性质创建代理
        /// </summary>
        /// <param name="ProxyClass"></param>
        /// <returns></returns>
        public static object CreateProxyOfInherit(Type ProxyClass)
        {
            return CreateProxy(ProxyClass, ProxyClass);
        }
        private static TProxy CreateProxy<TProxy, TProxyClass>()
        {
            Type TInterface = typeof(TProxy);
            Type TImplement = typeof(TProxyClass);

            string nameOfAssembly = TImplement.Name + "ProxyAssembly";
            string nameOfModule = TImplement.Name + "ProxyModule";
            string nameOfType = TImplement.Name + "Proxy";

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);
            TypeBuilder typeBuilder = null;
            MethodAttributes methodAttributes;
            if (typeof(TProxy).Equals(typeof(TProxyClass)))
            {
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, TImplement);
                methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
            }
            else
            {
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, null, new[] { TInterface });
                methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
            }

            return (TProxy)Invoke(TImplement, typeBuilder, methodAttributes);
        }
        private static object CreateProxy(Type Proxy, Type Implement)
        {
            Type TInterface = Proxy;
            Type TImplement = Implement;

            string nameOfAssembly = TImplement.Name + "ProxyAssembly";
            string nameOfModule = TImplement.Name + "ProxyModule";
            string nameOfType = TImplement.Name + "Proxy";

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);
            TypeBuilder typeBuilder = null;
            MethodAttributes methodAttributes;
            if (Proxy.Equals(TImplement))
            {
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, TImplement);
                methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
            }
            else
            {
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, null, new[] { TInterface });
                methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
            }

            return Invoke(TImplement, typeBuilder, methodAttributes);
        }
        private static object Invoke(Type TImplement, TypeBuilder typeBuilder, MethodAttributes methodAttributes)
        {
            Type interceptorAttributeType = TImplement.GetCustomAttribute(typeof(InterceptorBaseAttribute))?.GetType();

            if (interceptorAttributeType == null)
                throw new NullReferenceException($"{nameof(InterceptorBaseAttribute)} Not Used,Please Check");
            //声明拦截器构造
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            //获取构造函数IL
            var ilOfCtor = constructorBuilder.GetILGenerator();
            //声明拦截器字段
            FieldBuilder fieldInterceptor = typeBuilder.DefineField("_interceptor", interceptorAttributeType, FieldAttributes.Private);
            //第一个参数推到构造函数中的堆栈上
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            //实例化
            ilOfCtor.Emit(OpCodes.Newobj, interceptorAttributeType.GetConstructor(new Type[0]));
            //给字段赋值
            ilOfCtor.Emit(OpCodes.Stfld, fieldInterceptor);
            //实现类声明字段
            FieldBuilder _serviceImpObj = typeBuilder.DefineField("_serviceImpObj", TImplement, FieldAttributes.Private);
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Newobj, TImplement.GetConstructor(new Type[0]));
            ilOfCtor.Emit(OpCodes.Stfld, _serviceImpObj);

            ilOfCtor.Emit(OpCodes.Ret);

            MethodInfo[] methodsOfTypes = TImplement.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methodsOfTypes)
            {
                if (IgnoreMethodName.Contains(method.Name)) continue;

                Type[] methodParameterTypes = method.GetParameters().Select(t => t.ParameterType).ToArray();

                var methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, methodParameterTypes);
                //获取方法IL
                var ilMethod = methodBuilder.GetILGenerator();
                //设置字段
                var methodName = ilMethod.DeclareLocal(typeof(string));     //instance of method name
                var className = ilMethod.DeclareLocal(typeof(string));     //instance of class name
                var parameters = ilMethod.DeclareLocal(typeof(object[]));   //instance of parameters
                var result = ilMethod.DeclareLocal(typeof(object));         //instance of result
                Dictionary<Type, LocalBuilder> actionTypeBuilders = new Dictionary<Type, LocalBuilder>();

                string CodeLine = string.Empty;
                //attribute init
                if (method.GetCustomAttributes<AopBaseActionAttribute>().Any() || TImplement.GetCustomAttributes<AopBaseActionAttribute>().Any())
                {
                    CodeLine = method.GetCustomAttributes<AopBaseActionAttribute>().FirstOrDefault().Code;
                    //method can override class attrubute
                    if (method.GetCustomAttributes<AopBaseActionAttribute>().Any())
                    {
                        foreach (var item in method.GetCustomAttributes<AopBaseActionAttribute>().ToDictionary(k => k.GetType(), v => default(LocalBuilder)))
                        {
                            if (actionTypeBuilders.ContainsKey(item.Key))
                                actionTypeBuilders[item.Key] = item.Value;
                            else
                                actionTypeBuilders.Add(item.Key, item.Value);
                        }
                    }
                    else if (TImplement.GetCustomAttributes<AopBaseActionAttribute>().Any())
                    {
                        foreach (var item in TImplement.GetCustomAttributes<AopBaseActionAttribute>().ToDictionary(k => k.GetType(), v => default(LocalBuilder)))
                        {
                            if (actionTypeBuilders.ContainsKey(item.Key))
                                actionTypeBuilders[item.Key] = item.Value;
                            else
                                actionTypeBuilders.Add(item.Key, item.Value);
                        }
                    }

                    foreach (var item in actionTypeBuilders.Select(t => t.Key).ToArray())
                    {
                        var actionAttributeObj = ilMethod.DeclareLocal(item);
                        ilMethod.Emit(OpCodes.Newobj, item.GetConstructor(new Type[0]));
                        ilMethod.Emit(OpCodes.Stloc, actionAttributeObj);
                        actionTypeBuilders[item] = actionAttributeObj;
                    }
                }

                //if no attribute
                if (fieldInterceptor != null || actionTypeBuilders.Any())
                {
                    ilMethod.Emit(OpCodes.Ldstr, method.Name);
                    ilMethod.Emit(OpCodes.Stloc_0, methodName);

                    ilMethod.Emit(OpCodes.Ldstr, method.DeclaringType.Name);
                    ilMethod.Emit(OpCodes.Stloc_1, className);

                    ilMethod.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    ilMethod.Emit(OpCodes.Newarr, typeof(object));
                    ilMethod.Emit(OpCodes.Stloc, parameters);

                    // build the method parameters
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldloc, parameters);
                        ilMethod.Emit(OpCodes.Ldc_I4, j);
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                        //box
                        ilMethod.Emit(OpCodes.Box, methodParameterTypes[j]);
                        ilMethod.Emit(OpCodes.Stelem_Ref);
                    }
                }

                //dynamic proxy action before
                if (actionTypeBuilders.Any())
                {
                    //load arguments
                    foreach (var item in actionTypeBuilders)
                    {
                        ilMethod.Emit(OpCodes.Ldloc, item.Value);
                        ilMethod.Emit(OpCodes.Ldloc, methodName);
                        ilMethod.Emit(OpCodes.Ldloc, className);
                        ilMethod.Emit(OpCodes.Ldloc, parameters);
                        ilMethod.Emit(OpCodes.Call, item.Key.GetMethod("Before"));
                    }
                }

                if (interceptorAttributeType != null)
                {
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, fieldInterceptor);
                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, _serviceImpObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, parameters);
                    // call Invoke() method of Interceptor
                    ilMethod.Emit(OpCodes.Callvirt, interceptorAttributeType.GetMethod("Invoke"));
                }
                else
                {
                    //direct call method
                    if (method.ReturnType == typeof(void) && !actionTypeBuilders.Any())
                    {
                        ilMethod.Emit(OpCodes.Ldnull);
                    }

                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, _serviceImpObj);
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                    }
                    ilMethod.Emit(OpCodes.Callvirt, TImplement.GetMethod(method.Name));
                    //box
                    if (actionTypeBuilders.Any())
                    {
                        if (method.ReturnType != typeof(void))
                            ilMethod.Emit(OpCodes.Box, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Ldnull);
                    }
                }

                //dynamic proxy action after
                if (actionTypeBuilders.Any())
                {
                    ilMethod.Emit(OpCodes.Stloc, result);

                    //1->2 before and 2->1 after
                    foreach (var item in actionTypeBuilders.Reverse())
                    {
                        ilMethod.Emit(OpCodes.Ldloc, item.Value);
                        ilMethod.Emit(OpCodes.Ldloc, methodName);
                        ilMethod.Emit(OpCodes.Ldloc, className);
                        ilMethod.Emit(OpCodes.Ldloc, result);
                        ilMethod.Emit(OpCodes.Callvirt, item.Key.GetMethod("After"));

                        //if no void return,set result
                        if (method.ReturnType == typeof(void))
                            ilMethod.Emit(OpCodes.Pop);
                        else
                            ilMethod.Emit(OpCodes.Stloc, result);
                    }
                }
                else
                {
                    //if no void return,set result
                    if (method.ReturnType == typeof(void))
                        ilMethod.Emit(OpCodes.Nop);
                    else
                        ilMethod.Emit(OpCodes.Stloc, result);
                }

                // pop the stack if return void
                if (method.ReturnType == typeof(void))
                {
                    //if no action attribute，void method need pop(action attribute method has done before)
                    if (!actionTypeBuilders.Any())
                        ilMethod.Emit(OpCodes.Pop);
                }
                else
                {
                    //unbox,if direct invoke,no box
                    if (fieldInterceptor != null || actionTypeBuilders.Any())
                    {
                        ilMethod.Emit(OpCodes.Ldloc, result);

                        if (method.ReturnType.IsValueType)
                            ilMethod.Emit(OpCodes.Unbox_Any, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Castclass, method.ReturnType);
                    }
                }
                // complete
                ilMethod.Emit(OpCodes.Ret);
            }

            var typeInfo = typeBuilder.CreateTypeInfo();
            return Activator.CreateInstance(typeInfo);
        }
    }
}
