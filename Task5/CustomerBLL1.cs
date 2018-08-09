using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyIoc.Attributes;
using MyIoc;

namespace Task5
{
    [ImportConstructor]
    public class CustomerBLL1
    {

        private ICustomerDAL dal;
        private Logger logger;

        public CustomerBLL1(ICustomerDAL dal, Logger logger)
        {
            this.dal = dal;
            this.logger = logger;
        }

    }
}
