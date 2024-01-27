using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSGenerator {
    public class Person {
        public class Address {

        }

        public int Age;
        public string FullName;
        public Address Address;


        public static List<Person> GetAll() {
            throw new NotImplementedException();
        }
        public static Person Load(int id) {
            throw new NotImplementedException();
        }
        public static void Delete(int id) {
            throw new NotImplementedException();
        }
        public static int Insert(Person data) {
            throw new NotImplementedException();
        }
        public Address AddAddress(string street, int houseNo, int country, DateTime startDate, DateTime? endDate) {
            throw new NotImplementedException();
        }

    }
}