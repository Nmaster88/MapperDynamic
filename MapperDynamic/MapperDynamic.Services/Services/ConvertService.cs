using CsvReaderApp.Models;
using System.Collections;
using System.Reflection;

//TODO: check this class again, to understand it's intent
namespace CsvReaderApp.Services
{
    /// <summary>
    /// ConvertService uses ObjectAssignmentService properties to do the mapping between an input object to an output object that contains a list
    /// </summary>
    /// <typeparam name="TI"></typeparam>
    /// <typeparam name="TO"></typeparam>
    public class ConvertService<TI, TO>
    {
        private readonly ObjectAssignementService<TI, TO> _objectAssignementService;
        public ConvertService(ObjectAssignementService<TI, TO> objectAssignementService)
        {
            _objectAssignementService = objectAssignementService ?? throw new ArgumentNullException(nameof(ObjectAssignementService<TI, TO>));
        }

        public void Convert(TI input, TO output)
        {
            Type typeElementO = null;
            if (typeof(TO).IsGenericType && typeof(TO).GetGenericTypeDefinition() == typeof(List<>))
            {
                Type typeListO = typeof(TO);
                typeElementO = typeListO.GetGenericArguments()[0];
            }

            //TO if its a list we need to get what is the element of the list so that we can use it to create objects to populate the list
            //if it is already the class its easier
            if (typeof(TI).IsGenericType && typeof(TI).GetGenericTypeDefinition() == typeof(List<>))
            {
                var inputList = input as IEnumerable;
                if (inputList != null)
                {
                    foreach (object element in inputList)
                    {
                        dynamic instance = Activator.CreateInstance(typeElementO); //creating a new instance of the output class

                        Type objectElementType = element.GetType();

                        // Get all properties of the object type
                        PropertyInfo[] properties = objectElementType.GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            var objPropertyMatch = _objectAssignementService.ObjectPropertiesIOMatch.Find(x => x.PropertyInput.Name == property.Name);
                            if (objPropertyMatch != null)
                            {
                                Console.WriteLine("property found.");


                                // Get all properties of the object type Output
                                PropertyInfo[] propertiesO = typeElementO.GetProperties();

                                PropertyInfo propertyOMatched = propertiesO.FirstOrDefault(x => x.Name == objPropertyMatch.PropertyOutput.Name);

                                if (propertyOMatched != null)
                                {
                                    var value = property.GetValue(element);
                                    if (objPropertyMatch.ObjectConversion == ObjectConversion.NoConversion)
                                    {
                                        instance.GetType().GetProperty(propertyOMatched.Name)?.SetValue(instance, value);
                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }
                                    //TODO: need to work when the type is different for Input and Output
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"{typeof(TI)} is not a valid type. It should be a list");
            }
        }

        private void AssignToElement()
        {
            throw new NotImplementedException();
        }

        //TODO: code method that reads the value of a property and assigns it to the right method to convert.
        // using enum is find but doesn't follow best practices of OOP. we should use a design patter for this.

        private string IntConvertToString(int value)
        {
            return value.ToString();
        }

        private string DecimalConvertToString(decimal value)
        {
            return value.ToString();
        }

        private decimal StringConvertToDecimal(string value)
        {
            decimal result;

            if (decimal.TryParse(value, out result))
            {
                return result;
            }

            throw new ArgumentException("Invalid decimal value.");
        }

        private decimal StringConvertToInt(string value)
        {
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                return (decimal)intValue; // Successful conversion to int, cast to decimal
            }

            // Handle conversion failure or invalid input
            // You can throw an exception, return a default value, or handle it based on your requirements
            throw new ArgumentException("Invalid integer value.");
        }
    }
}
