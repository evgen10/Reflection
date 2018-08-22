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

        //для хранения типов помеченных как Import
        private Dictionary<Type, Type> importDictionary;
        //для хранения типов помеченных как Export
        private Dictionary<Type, Type> exportDictionary;

        public Container()
        {
            importDictionary = new Dictionary<Type, Type>();
            exportDictionary = new Dictionary<Type, Type>();
        }

        //добавляет в контейнер типы в сборке помеченные атрибутами(Import, Export)
        public void AddAssembly(Assembly assembly)
        {
            var types = assembly.ExportedTypes;

            foreach (var type in types)
            {
                int value = DefineAttribute(type);

                if (value == 1)
                {
                    importDictionary.Add(type, type);
                }
                if (value == 0)
                {
                    //если класс помечен  атрибутом export
                    var attributes = type.GetCustomAttribute<ExportAttribute>();
                    //добавляем как ключ в словарь параметр атрибута если он есть, иначе сам тип
                    exportDictionary.Add(attributes.Contract ?? type, type);
                }

            }

        }
        
        //добавляет в контейнер конкретный тип
        public void AddType(Type type)
        {
            int value = DefineAttribute(type);

            if (value == 1)
            {
                importDictionary.Add(type, type);
            }
            else if (value == 0)
            {
                exportDictionary.Add(type, type);
            }
            else
            {
                throw new ContainerException("Attribute missing or invalid");
            }



        }

        //добавляет в контейнер тип и его базовый класс
        //предназначен для внедряемого класса
        public void AddType(Type type, Type baseType)
        {
            if (DefineAttribute(type) == 0)
            {
                exportDictionary.Add(baseType, type);
            }
            else
            {
                throw new ContainerException("Attribute missing or invalid");
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

        //инициализирует тип в зависимости от способа инициализации
        private object Instance(Type type)
        {
            //если передаваемый тип помечен атрибутом Import
            if (HasAttributeImport(type))
            {
                // если свойства в классе помечены атрибутом Import
                if (type.GetProperties().Where(t => t.GetCustomAttribute<ImportAttribute>() != null).Count() != 0)
                {
                    //инициализируем по свойствам 
                    return InstanceByProperty(type);
                }

                //иначе по конструктору 
                return InstanceByConstructor(type);

            }
            else
            {
                throw new ContainerException("Attribute missing or invalid");
            }

        }

        //инициализирует зависимый тип по свойствам
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
                    throw new ContainerException($"Does not have dependence for {prop.Name}");
                }
            }

            return result;

        }

        //инициализирует зависимый тип по конструктору
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
                    throw new ContainerException($"Does not have dependence for {parametr.ParameterType} {parametr.Name} ");
                }
            }

            return Activator.CreateInstance(type, parametrs.ToArray());
        }

        //получает конструктор в указаном типе
        private ConstructorInfo GetConstructor(Type type)
        {
            var constractors = type.GetConstructors();

            if (constractors.Length != 0)
            {
                return constractors.First();
            }
            else
            {
                throw new ContainerException("Does not have public constructors");
            }
        }

        //проверяет находится ли в контейнере данный класс с атрибутом Import
        private bool HasAttributeImport(Type type)
        {
            if (importDictionary.ContainsKey(type))
            {
                return true;
            }

            return false;
        }

        //возвращает 1 если класс помечен атрибуитом Import
        //0 если export 
        //-1 если не помечен данными атрибутами
        private int DefineAttribute(Type type)
        {
            IEnumerable<Attribute> attribyteInfo;
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
