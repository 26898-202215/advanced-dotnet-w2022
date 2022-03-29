using System;

namespace Week11_1_Facade
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting 'Facade' Design Pattern Application!");

            Driver sampleDriver = new Driver("Abc 123 XYZ", "John Doe");

            InsuranceFacade newFacade = new InsuranceFacade();
            newFacade.SetRate(sampleDriver);


        }
    }
}
