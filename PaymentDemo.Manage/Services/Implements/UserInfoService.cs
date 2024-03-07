using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;
using System.Text;

namespace PaymentDemo.Manage.Services.Implements
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;

        public UserInfoService(IUnitOfWork unitOfWork, IMapper mapper, ICommonService commonService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commonService = commonService;
        }

        public async Task<UserInfo?> CreateUserInfo(UserInfo userInfo)
        {
            userInfo.Id = 0;
            if (string.IsNullOrWhiteSpace(userInfo.UserName) || string.IsNullOrWhiteSpace(userInfo.Password)) return null;

            // hash pass
            userInfo.Password = _commonService.HashPasword(userInfo.Password, out var salt);
            userInfo.Salt = salt.ToString();

            var user = await _unitOfWork.GetRepository<UserInfo>().CreateAsync(userInfo);
            await _unitOfWork.SaveAsync();

            return user;
        }

        public async Task<UserInfo?> GetUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return null;

            var user = await _unitOfWork.GetRepository<UserInfo>()
                .GetAll().AsQueryable()
                .SingleOrDefaultAsync(x => x.Email.Equals(email));

            if (user == null || string.IsNullOrWhiteSpace(user.Salt) || string.IsNullOrWhiteSpace(user.Password)) return null;

            var passHashed = user.Password;
            var salt = Encoding.ASCII.GetBytes(user.Salt);

            if (!_commonService.VerifyPassword(password, passHashed, salt)) return null;

            return user;
        }
    }
}
