using System.Collections.Generic;
using FluentAssertions;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
using Xunit;

namespace Csvialize.UnitTests
{
    public class BrowserTests
    {
        private readonly User[] _users = { new User("Jim", 45), new User("Joe", 68) };
        private readonly string _usersString =   "Name,Age\r\nJim,45\r\nJoe,68\r\n";

        [Fact]
        public async void Can_serialize_csv()
        {
            var testModule = new ConfigurableNancyModule(config =>
            {
                config.Get("/", (o, module) => _users);
            });

            var browser = new Browser(with =>
            {
                with.Module(testModule);
            });

            var response = await browser.Get("/", context =>
            {
                context.Accept(new MediaRange("text/csv"));
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response.ContentType.Should().Be("text/csv");

            response.Body.AsString().ShouldBeEquivalentTo(_usersString);
        }

        [Fact]
        public async void Can_deserialize_csv()
        {
            var testModule = new ConfigurableNancyModule(config =>
            {
                config.Post("/", (o, module) =>
                {
                    var u = module.Bind<List<User>>();
                    u.ShouldBeEquivalentTo(_users);
                    return u;
                });
            });

            var browser = new Browser(with =>
            {
                with.Module(testModule);
            });

            var response = await browser.Post("/", context =>
            {
                context.Body(_usersString, "text/csv");
                context.Accept("text/csv");
            });
            
            response.Body.ContentType.ShouldBeEquivalentTo("text/csv");
            response.Body.AsString().ShouldBeEquivalentTo(_usersString);
        }
    }
}
