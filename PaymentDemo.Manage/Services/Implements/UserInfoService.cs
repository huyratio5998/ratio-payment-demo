using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserInfoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserInfo?> GetUserAsync(string email, string password)
        {
            if(string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(email)) return null;

            var user = await _unitOfWork.GetRepository<UserInfo>()
                .GetAll().AsQueryable()
                .SingleOrDefaultAsync(x => x.Email.Equals(email) && x.Password.Equals(password));

            if (user == null) return null;

            return user;
        }
    }
}
