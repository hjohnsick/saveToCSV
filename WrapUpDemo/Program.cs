using System;
using System.Collections.Generic;
using System.IO;

namespace WrapUpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PersonModel> people = new List<PersonModel>
            {
                new PersonModel{ FirstName = "Regina", LastName = "Philangee", Email = "regina_philangee@comcast.net"},
                new PersonModel{ FirstName = "Phil", LastName = "McCracken", Email = "philly@comcast.net"},
                new PersonModel{ FirstName = "Anita", LastName = "Break", Email = "a_break@yahoo.com"},
            };

            List<CarModel> cars = new List<CarModel>
            {
                new CarModel{ Manufacturer = "Ford", Model = "darnBronco"},
                new CarModel{ Manufacturer = "Ford", Model = "Mustang"},
                new CarModel{ Manufacturer = "Buick", Model = "Encore"}
            };

            DataAccess<PersonModel> person = new DataAccess<PersonModel>();
            person.BadEntryFound += Person_BadEntryFound;
            person.SaveToCSV(people, @"C:\Temp\SavedFiles\people.csv");
            
            DataAccess<CarModel> car = new DataAccess<CarModel>();
            car.BadEntryFound += Car_BadEntryFound;
            car.SaveToCSV(cars, @"C:\Temp\SavedFiles\cars.csv");

            Console.ReadLine();
        }

        private static void Car_BadEntryFound(object sender, CarModel e)
        {
            Console.WriteLine($"Bad Entry found for {e.Manufacturer} {e.Model}");
        }

        private static void Person_BadEntryFound(object sender, PersonModel e)
        {
            Console.WriteLine($"Bad Entry found for {e.FirstName} {e.LastName}");
        }
    }

    public class DataAccess<T> where T: new()
    {
        public event EventHandler<T> BadEntryFound;
        public void SaveToCSV(List<T> items, string filePath) 
        {
            List<string> rows = new List<string>();
            T entry = new T();
            var cols = entry.GetType().GetProperties();
            string row = "";
            foreach (var col in cols)
            {
                row += $",{col.Name}";
            }

            row = row.Substring(1);
            rows.Add(row);

            foreach (var item in items)
            {
                row = "";
                bool badWordDetected = false;

                foreach (var col in cols)
                {
                    string val = col.GetValue(item, null).ToString();

                    badWordDetected = BadWordDetector(val);
                    if (badWordDetected == true)
                    {
                        BadEntryFound?.Invoke(this, item);
                        break;
                    }

                    row += $",{val},";
                }

                if (badWordDetected == false)
                {
                    row = row.Substring(1);
                    rows.Add(row);
                }

            }

            File.WriteAllLines(filePath, rows);
        }

        private bool BadWordDetector(string stringToTest)
        {
            bool output = false;
            string lowerCaseTest = stringToTest.ToLower();

            if (lowerCaseTest.Contains("darn") || lowerCaseTest.Contains("heck"))
            {
                output = true;
            }

            return output;
        }
    }
}
