using System;
using System.Collections.Generic;
using System.Text;

namespace Week10_2_Factory
{
    class EmptyClasses
    {
        // Product abstract Class
        public abstract class IProduct
        {

        }

        // A 'ConcreteProduct' class
        public class ConcreteProductA : IProduct
        {

        }

        // A 'ConcreteProduct' Class
        public class ConcreteProductB : IProduct
        {

        }

        //The Creator class
        public class Creator
        {
            public IProduct GetProduct(string type)
            {
                switch (type)
                {
                    case "ProductA":
                        return new ConcreteProductA();
                    case "ProductB":
                        return new ConcreteProductB();
                    default:
                        throw new NotSupportedException();
                }
            }
        }

    }
}
