using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MyIoc.Attributes;
using MyIoc;

namespace Task5
{
    class Program
    {
        static void Main(string[] args)
        {

            //Assembly assembly = Assembly.LoadFrom(@"D:\Tasks\Code\Task5.Reflection\MyIoc\bin\Debug\MyIoc.dll");

            ////var types = assembly.GetTypes().Where(t => t.IsClass).ToList();

            //var types = assembly.ExportedTypes;

            //foreach (var type in types)
            //{             

            //    var t = type.GetCustomAttribute<ExportAttribute>();
            //    Console.WriteLine(t);
            //}


            

            var container = new Container();

            //container.AddAssembly(Assembly.GetExecutingAssembly());

            container.AddType(typeof(CustomerBLL2));
            container.AddType(typeof(Logger));
            container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var ob = (CustomerBLL2)container.CreateInstance(typeof(CustomerBLL2));

           // var ob2 = container.CreateInstance<CustomerBLL1>();

            Console.WriteLine();
        }
    }
}
