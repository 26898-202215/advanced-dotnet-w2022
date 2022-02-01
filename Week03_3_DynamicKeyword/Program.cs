using System;
using System.Dynamic;

namespace Week03_3_DynamicKeyword
{
    class Program
    {
        static void Main(string[] args)
        {
            // dynamic typing is the process of resolving member information at runtime, regardless of type
            // we can do this using the "dynamic" keyword

            // declare and initialize a dynamic object
            // using the ExpandoObject class, we can add members to a type a runtime
            ExpandoObject expandoObject = new ExpandoObject();

            dynamic myObject = new ExpandoObject();

            // by declaring and initializing the value of our property
            // we have effectively added the property Color, and Size to our object
            myObject.Color = "Red";
            myObject.Size = "Large";

            Console.WriteLine(myObject.Color);
            Console.WriteLine(myObject.Size);


            //EX2:
            dynamic d = new ExpandoObject();

            d.MyProperty = "testing value";

            // by declaring and initializing an action called MyMethod
            // we have effectively added the method MyMethod to our object
            d.MyMethod = (Action)(() =>
            {
                Console.WriteLine("this is a dynamic action");
            });

            // let's print out the contents of the property
            Console.WriteLine(d.MyProperty);

            // let's invoke our method we just created
            d.MyMethod();



            //EX3 
            //****** Exception throwing cases ******
            dynamic d2 = new object();// this will compile successfully, however will encounter a runtime error

            // Reason: because object does not contain a property called MyProperty
            // un-comment the below line of code and run the project to see the error
            // d2.MyProperty = "test";

            dynamic d3 = null; // this will compile successfully, however an exception of type RuntimeBinderException will be thrown

            // Reason: because we cannot perform runtime binding on a null reference, i.e. an object that is null or has not been initialized
            // un-comment the below line of code and run the project to see the error
            //d3.MyMethod = (Action)(() =>
            //{
            //	Console.WriteLine("this is a dynamic action");
            //});

        }
    }
}
