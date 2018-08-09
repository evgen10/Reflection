using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyIoc.Attributes;
using System.Reflection;

namespace MyIoc
{
    public class Container
    {
        private Dictionary<Type, Type> importDictionary;
        private Dictionary<Type, Type> exportDictionary;


        public Container()
        {
            importDictionary = new Dictionary<Type, Type>();
            exportDictionary = new Dictionary<Type, Type>();
        }

        public void AddAssembly(Assembly assembly)
        {
            var types = assembly.ExportedTypes;

            foreach (var type in types)
            {

                int val = DefineAttribute(type);

                if (val == 1)
                {
                    importDictionary.Add(type, type);
                }
                if (val == 0)
                {
                    var attributes = type.GetCustomAttribute<ExportAttribute>();

                    exportDictionary.Add(attributes.Contract ?? type, type);

                }


            }

        }

        public void AddType(Type type)
        {
            int val = DefineAttribute(type);

            if (val == 1)
            {
                importDictionary.Add(type, type);
            }
            else if (val == 0)
            {
                exportDictionary.Add(type, type);
            }
            else
            {
                throw new Exception("Attribute missing or invalid");
            }
        }

        public void AddType(Type type, Type baseType)
        {

            if (DefineAttribute(type) == 0)
            {
                exportDictionary.Add(baseType, type);
            }
            else
            {
                throw new Exception("Attribute missing or invalid");
            }

        }



        public object CreateInstance(Type type)
        {
            return Instance(type);
        }

        public T CreateInstance<T>()
        {

            return (T)Instance(typeof(T));
        }


        private object Instance(Type type)
        {
            if (HasAttributeImport(type))
            {
                if (type.GetProperties().Where(t => t.GetCustomAttribute<ImportAttribute>() != null).Count() != 0)
                {
                    return InstanceByProperty(type);
                }


                return InstanceByConstructor(type);


            }
            else
            {
                throw new Exception("Attribute missing or invalid");
            }

        }

        private object InstanceByProperty(Type type)
        {
            var properties = type.GetProperties().Where(p => p.GetCustomAttribute<ImportAttribute>() != null);
            object result = Activator.CreateInstance(type);

            foreach (var prop in properties)
            {
                Type value;

                if (exportDictionary.TryGetValue(prop.PropertyType, out value))
                {
                    prop.SetValue(result, Activator.CreateInstance(value));
                }
                else
                {
                    throw new Exception($"Does not have dependence for {prop.Name}");
                }

            }

            return result;

        }

        private object InstanceByConstructor(Type type)
        {
            var constructor = GetConstructor(type);
            var constractorParameters = constructor.GetParameters();

            List<object> parametrs = new List<object>();

            foreach (var parametr in constractorParameters)
            {
                Type value;

                if (exportDictionary.TryGetValue(parametr.ParameterType, out value))
                {
                    parametrs.Add(Activator.CreateInstance(value));
                }
                else
                {
                    throw new Exception($"Does not have dependence for {parametr.Name}");
                }

            }

            return Activator.CreateInstance(type, parametrs.ToArray());
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            var constractors = type.GetConstructors();

            if (constractors.Length != 0)
            {
                return constractors.First();
            }
            else
            {
                throw new Exception("Does not have public constructors");
            }
        }

        private bool HasAttributeImport(Type type)
        {
            if (importDictionary.ContainsKey(type))
            {
                return true;
            }

            return false;
        }

        //возвращает 1 если класс помечен атрибуитом Import 0 если export -1 если не помечен данными атрибутами
        private int DefineAttribute(Type type)
        {
            IEnumerable<Attribute> attribyteInfo = type.GetCustomAttributes<ImportAttribute>();
            IEnumerable<PropertyInfo> typeProp = type.GetProperties().Where(p => p.GetCustomAttribute<ImportAttribute>() != null);

            if (typeProp.Count() != 0)
            {
                return 1;
            }

            attribyteInfo = type.GetCustomAttributes<ImportConstructorAttribute>();

            if (attribyteInfo.Count() != 0)
            {
                return 1;
            }

            attribyteInfo = type.GetCustomAttributes<ExportAttribute>();

            if (attribyteInfo.Count() != 0)
            {
                return 0;
            }

            return -1;

        }




    }
}
