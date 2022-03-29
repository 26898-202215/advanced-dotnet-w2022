using System;
using System.Collections.Generic;
using System.Text;

namespace Week10_1_Singleton
{
    class DatabaseConnection
    {
        private static readonly DatabaseConnection _instance = new DatabaseConnection();

        private DatabaseConnection()
        {
        }

        public static DatabaseConnection GetInstance()
        {
            return _instance;
        }
    }
}
