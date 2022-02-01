using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Week03_2_SqlQueryBuilder
{
    class Program
    {

        private static readonly Dictionary<Type, string> typeMap = new Dictionary<Type, string>
        {
            { typeof(Guid), "uniqueidentifier NOT NULL"},
            { typeof(Guid?), "uniqueidentifier NULL"},
            { typeof(DateTimeOffset), "datetimeoffset(7) NOT NULL"},
            { typeof(DateTimeOffset?), "datetimeoffset(7) NULL"},
            { typeof(int), "INT NOT NULL"},
            { typeof(int?), "INT NULL"},
            { typeof(double), "decimal(20,0) NOT NULL"},
            { typeof(double?), "decimal(20,0) NULL"},
            { typeof(string), "varchar(max) NULL"}
        };
        static void Main(string[] args)
        {
            // find all the types in the current assembly
            // that are not abstract classes and are not generic types
            // and that implement the IDatabaseTable interface
            var types =
                typeof(Program).Assembly.ExportedTypes.Where(t => t.IsClass
                && !t.IsAbstract
                && !t.IsGenericType
                && typeof(IDatabaseTable).IsAssignableFrom(t));

            var results = types.Select(GenerateTable);

            foreach (var item in results)
            {
                Console.WriteLine(item);
            }
        }

        public static string GenerateTable(Type type)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"CREATE TABLE {type.Name}");
            builder.AppendLine("(");

            // find all the properties on the given type
            // that are public instance properties
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                // generate the SQL query portion of each column
                // based on the property info
                builder.AppendLine($"\t{GenerateColumn(propertyInfo)}");
            }
            builder.AppendLine(")");
            return builder.ToString();
        }

        public static string GenerateColumn(PropertyInfo propertyInfo)
        {
            return $"{propertyInfo.Name} {typeMap[propertyInfo.PropertyType]},";
        }


    }

    public interface IDatabaseTable
    {

    }

    public class Owner : IDatabaseTable
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedTime { get; set; }

    }

    public interface IShape : IDatabaseTable
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }

    public abstract class Shape : IShape
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public Guid OwnerId { get; set; }

    }

    public class Circle : Shape
    {
        public double Radius { get; set; }
    }

    public class Rectangle : Shape
    {
        public double Length { get; set; }
        public double Width { get; set; }

    }

    public class Triangle : Shape
    {
        public double Base { get; set; }
        public double Height { get; set; }

    }
    public class Square : Shape
    {
        public double Side { get; set; }
    }
}
