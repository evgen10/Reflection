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

            var container = new Container();

            //container.AddAssembly(Assembly.GetExecutingAssembly());

            container.AddType(typeof(CustomerBLL2));
            //container.AddType(typeof(CustomerBLL1));
            container.AddType(typeof(Logger));
            container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));

            var ob = (CustomerBLL2)container.CreateInstance(typeof(CustomerBLL2));
            var ob2 = container.CreateInstance<CustomerBLL2>();

            //var ob = (CustomerBLL1)container.CreateInstance(typeof(CustomerBLL1));
            //var ob2 = container.CreateInstance<CustomerBLL1>();

            Console.WriteLine();
        }
    }
}
