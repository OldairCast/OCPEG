using Bogus;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;

namespace OCPEG.Common.TestUtilities.Requests
{
    public class RequestUserTest
    {
        public static User Build(int passwordLength = 10)
        {
            //objeto do Bogus
            return new Faker<User>()
                .RuleFor(user => user.Name, (f) => f.Person.FirstName)
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
                .RuleFor(user => user.PhoneNumber, (f) => f.Person.Phone)
                .RuleFor(user => user.PasswordHash, (f) => f.Internet.Password(passwordLength))
                .RuleFor(user => user.UserIdentifier, (f) => Guid.NewGuid());
        }

        public static UserDto BuildDto(int passwordLength = 10)
        {
            //objeto do Bogus
            return new Faker<UserDto>()
                .RuleFor(user => user.Name, (f) => f.Person.FirstName)
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
                .RuleFor(user => user.PhoneNumber, (f) => f.Person.Phone);
                //.RuleFor(user => user.Password, (f) => f.Internet.Password(passwordLength));
        }

        public static (UserDto user, string password) BuildFakeDto()
        {
            var password = new Faker().Internet.Password();

            var user = new Faker<UserDto>()
                .RuleFor(user => user.Name, (f) => f.Person.FirstName)
                .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
                .RuleFor(user => user.PhoneNumber, (f) => f.Person.Phone);
                //.RuleFor(user => user.Password, (f) => password);

            return (user, password);

        }
    }
}
