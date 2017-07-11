using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Nancy.ModelBinding;
using Xunit;

namespace Csvialize.UnitTests
{
    public class CsvDeserializerTests
    {
        private readonly User[] _users = { new User("Jim", 45), new User("Joe", 68) };
        private readonly string _usersString = "Name,Age\r\nJim,45\r\nJoe,68\r\n";

        public static IEnumerable<object> TestData()
        {
            return CollectionTypes().Select(type => new object[] {type});
        }

        public static IEnumerable<Type> CollectionTypes()
        {
            yield return typeof(List<User>);
            yield return typeof(IList<User>);
            yield return typeof(ICollection<User>);
            yield return typeof(HashSet<User>);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void Can_deserialize_collections(Type type)
        {
            var deserializer = new CsvDeserializer();
            var bindingContext = new BindingContext()
            {
                DestinationType = type,
                GenericType = type.GetGenericArguments()[0],
                Configuration = new BindingConfig()
            };

            var result = deserializer.Deserialize("text/csv", new MemoryStream(Encoding.UTF8.GetBytes(_usersString)),
                bindingContext);

            type.IsInstanceOfType(result).Should().BeTrue();

            ((IEnumerable)result).ShouldBeEquivalentTo(_users);
        }

        [Fact]
        public void Can_deserialize_single()
        {
            var deserializer = new CsvDeserializer();
            var bindingContext = new BindingContext()
            {
                DestinationType = typeof(User),
                GenericType = null,
                Configuration = new BindingConfig()
            };

            var result = deserializer.Deserialize("text/csv", new MemoryStream(Encoding.UTF8.GetBytes("Name,Age\r\nJim,45\r\n")),
                bindingContext);

            ((User)result).ShouldBeEquivalentTo(_users[0]);
        }
        
    }
}
