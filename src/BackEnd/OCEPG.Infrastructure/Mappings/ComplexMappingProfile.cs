using AutoMapper;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.Dto;
using Sqids;

namespace OCEPG.Infrastructure.Mappings
{
    public class ComplexMappingProfile : Profile
    {
        private readonly SqidsEncoder<int> _idEnconder;
        private readonly SqidsEncoder<long> _idlEnconder;

        public ComplexMappingProfile(SqidsEncoder<int> idEnconder, SqidsEncoder<long> idlEnconder)
        {
            _idEnconder = idEnconder;
            _idlEnconder = idlEnconder;
            CreateMapper();
        }

        private void CreateMapper()
        {
            CreateMap<Usuario, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Telefone))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => src.Administrador))
                .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Funcao))
                .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken))
                .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.MapFrom(src => src.RefreshTokenExpiryTime))
                .ForMember(dest => dest.UserIdentifier, opt => opt.MapFrom(src => src.UserIdentifier))
                .ReverseMap();

            CreateMap<Usuario, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => _idEnconder.Encode(src.Id)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Telefone))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => src.Administrador))
                .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Funcao))
                .ReverseMap();


            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => _idEnconder.Encode(src.Id)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => src.Admin))
                .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function));

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => _idEnconder.Decode(src.Id).Count > 0
                ? _idEnconder.Decode(src.Id)[0] : 0))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => src.Admin))
                .ForMember(dest => dest.Function, opt => opt.MapFrom(src => src.Function));


            CreateMap<User, UserLoginDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ReverseMap();

            CreateMap<UserDto, UserRegister>().ReverseMap();

            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => _idlEnconder.Encode(src.Id)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => src.EnrollmentDate))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.MonthlyPayment, opt => opt.MapFrom(src => src.MonthlyPayment))
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId));

            CreateMap<CustomerDto, Customer>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => _idlEnconder.Decode(src.CustomerId).Count > 0
                ? _idlEnconder.Decode(src.CustomerId)[0] : 0))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telephone))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => src.EnrollmentDate))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.MonthlyPayment, opt => opt.MapFrom(src => src.MonthlyPayment))
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId));

            CreateMap<Cliente, CustomerDto>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => _idlEnconder.Encode(src.Id)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src.Telefone))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Idade))
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => src.DataInscricao))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Texto))
                .ForMember(dest => dest.MonthlyPayment, opt => opt.MapFrom(src => src.Mensalidade))
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TipoId))
                .ReverseMap();

            CreateMap<Call, CallDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => _idlEnconder.Encode(src.CallNumber)))
                .ForMember(dest => dest.CallNumber, opt => opt.MapFrom(src => src.CallNumber))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<CallDto, Call>()
                .ForMember(dest => dest.CallNumber, opt => opt.MapFrom(src => src.CallNumber))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.FinishDate, opt => opt.MapFrom(src => src.FinishDate))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        }
    }
}
