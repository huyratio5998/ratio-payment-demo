using AutoMapper;
using FluentValidation;
using PaymentDemo.Manage.Entities;
using PaymentDemo.Manage.Models;
using PaymentDemo.Manage.Repositories.Abstracts;
using PaymentDemo.Manage.Services.Abstractions;

namespace PaymentDemo.Manage.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UserViewModel> _validator;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UserViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<UserViewModel?> GetUserAsync(int id)
        {
            if (id <= 0) return null;

            var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);
            if (user == null) return null;

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            var users = _unitOfWork.GetRepository<User>().GetAll().ToList();
            return _mapper.Map<List<UserViewModel>>(users);
        }

        public async Task<int> CreateUserAsync(UserViewModel user)
        {
            var userValidate = _validator.Validate(user);
            if (!userValidate.IsValid) return 0;

            var createdUser = await _unitOfWork.GetRepository<User>()
                .CreateAsync(_mapper.Map<User>(user));

            if (createdUser == null || createdUser.Id == 0) return 0;

            await _unitOfWork.SaveAsync();

            return createdUser.Id;
        }

        public async Task<bool> UpdateUserAsync(UserViewModel user)
        {
            var userValidate = _validator.Validate(user);
            if (!userValidate.IsValid) return false;
            if (user == null || user.Id == 0) return false;

            var userRepo = _unitOfWork.GetRepository<User>();
            var userInfoCreatedDate = (await userRepo.GetByIdAsync(user.Id))?.CreatedDate;
            var newUser = _mapper.Map<User>(user);
            if (userInfoCreatedDate != null) newUser.CreatedDate = (DateTime)userInfoCreatedDate;

            var result = _unitOfWork.GetRepository<User>()
                .Update(newUser);

            if (!result) return false;

            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
