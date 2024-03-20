using AutoMapper;
using InoxThanhNamServer.Datas.Authentication;
using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Services.JWT;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using InoxThanhNamServer.Models;
using System.Data;
using InoxThanhNamServer.Datas.UserAddress;
using InoxThanhNamServer.Services.UserAddress;

namespace InoxThanhNamServer.Services.UserSer
{
    public class UserService : IUserService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;
        private readonly IUserAddressService _userAddressService;

        public UserService(InoxEcommerceContext context, IJWTService jwtService, IMapper mapper, IUserAddressService userAddressService)
        {
            _context = context;
            _jwtService = jwtService;
            _mapper = mapper;
            _userAddressService = userAddressService;
        }

        public async Task<ApiResponse<LoginResult>> Login(LoginRequest request)
        {
            try
            {
                await Task.CompletedTask;
                var user = _context.Users.FirstOrDefault(x => x.Username == request.Username);

                if (user == null)
                {
                    return new ApiResponse<LoginResult>
                    {
                        Success = false,
                        Message = "Người dùng không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                else if(!user.IsActive)
                {
                    return new ApiResponse<LoginResult>
                    {
                        Success = false,
                        Message = "Tài khoản bị khóa, vui lòng liên hệ admin.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var userAddress = await _context.UserAddresses.Where(x => x.UserID == user.UserID).ToListAsync();

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return new ApiResponse<LoginResult>
                    {
                        Success = false,
                        Message = "Sai mật khẩu, nhập lại mật khẩu.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var roles = (from u in _context.Users
                             join ur in _context.UserRoles
                             on u.UserID equals ur.UserID
                             join r in _context.Roles
                             on ur.RoleID equals r.RoleID
                             where u.UserID == user.UserID
                             select r.Name).ToArray();

                var claims = new List<Claim>
                {
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim("Fullname", user.Firstname + " " + user.Lastname),
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var result = _jwtService.GenerateToken(user, claims, DateTime.Now);
                result.UserResult = new UserResult
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    Roles = roles,
                    Fullname = user.Lastname + " " + user.Firstname,
                    Phone = "0" + user.Phone.ToString(),
                    Address = userAddress.Select(x => x.Address).ToList()
                };

                return new ApiResponse<LoginResult>
                {
                    Success = true,
                    Message = "Đăng nhập thành công.",
                    Data = result,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResult>
                {
                    Success = false,
                    Message = "User service - Login: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<UserCreateRequest>> Register(UserCreateRequest newUser)
        {
            try
            {
                if (newUser.Username == null || newUser.Password == null)
                {
                    return new ApiResponse<UserCreateRequest>
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không hợp lệ.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                if (await IsExist(newUser.Username))
                {
                    return new ApiResponse<UserCreateRequest>
                    {
                        Success = false,
                        Message = "Username hoặc email này đã tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var userEntity = _mapper.Map<User>(newUser);

                await _context.Users.AddAsync(userEntity);

                var normalRole = await _context.Roles.FirstOrDefaultAsync(x => x.Code == 300);
                if (normalRole == null)
                {
                    return new ApiResponse<UserCreateRequest>
                    {
                        Success = false,
                        Message = "Không tồn tại quyền để tạo tài khoản mới.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                await _context.SaveChangesAsync();

                await RelateRole(normalRole.RoleID, userEntity.UserID);

                return new ApiResponse<UserCreateRequest>
                {
                    Success = true,
                    Message = "Tạo người dùng mới thành công.",
                    Data = newUser,
                    Status = (int)HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserCreateRequest>
                {
                    Success = false,
                    Message = "User service - Register: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        private async Task RelateRole(int roleID, Guid userID)
        {
            var userRole = new UserRole
            {
                RoleID = roleID,
                UserID = userID,
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsExist(string userName)
        {
            try
            {
                await Task.CompletedTask;
                bool result = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName) is not null;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ApiResponse<string>> ChangePassword(Guid UserID, ChangePassswordRequest request)
        {
            try
            {
                await Task.CompletedTask;
                var user = await _context.Users.FindAsync(UserID);
                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Người dùng không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                }
                if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Mật khẩu cũ không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _context.Users.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Đổi mật khẩu thành công, vui lòng đăng nhập lại.",
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "User service - ChangePassword: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<Object>> Logout(string token)
        {
            try
            {
                await Task.CompletedTask;
                bool result = await _jwtService.RevokeToken(token);
                if (!result)
                {
                    return new ApiResponse<Object>
                    {
                        Success = false,
                        Message = "Token không hợp lệ.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                else
                    return new ApiResponse<Object>
                    {
                        Success = true,
                        Message = "Đăng xuất thành công.",
                        Status = (int)HttpStatusCode.OK
                    };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Object>
                {
                    Success = false,
                    Message = "User service - Logout: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<UserProfile>> GetProfile(Guid UserID)
        {
            try
            {
                await Task.CompletedTask;
                var user = await _context.Users.FindAsync(UserID);
                if (user == null)
                {
                    return new ApiResponse<UserProfile>
                    {
                        Success = false,
                        Message = "Người dùng không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                return new ApiResponse<UserProfile>
                {
                    Success = true,
                    Message = "Lấy người dùng thành công.",
                    Data = _mapper.Map<UserProfile>(user),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserProfile>
                {
                    Success = false,
                    Message = "UserService - GetProfile: " + ex,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<UserProfile>>> GetUsers()
        {
            try
            {
                await Task.CompletedTask;
                var users = await _context.Users.ToListAsync();

                if (users == null || users.Count() <= 0)
                {
                    return new ApiResponse<List<UserProfile>>
                    {
                        Success = false,
                        Message = "Danh sách trống.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var usersMapper = users.Select(x => _mapper.Map<UserProfile>(x)).ToList();
                foreach(var user in usersMapper)
                {
                    user.Roles = (from u in _context.Users
                                  join ur in _context.UserRoles
                                  on u.UserID equals ur.UserID
                                  join r in _context.Roles
                                  on ur.RoleID equals r.RoleID
                                  where u.UserID == user.UserID
                                  select r.Name).ToList();
                    var userAddressesEntity = await _context.UserAddresses.Where(x => x.UserID == user.UserID).ToListAsync();
                    var addressResponse = await _userAddressService.GetAddressByUser(user.UserID);
                    if(addressResponse != null && addressResponse.Success)
                    {
                        user.UserAddress = addressResponse.Data;
                    }
                };

                return new ApiResponse<List<UserProfile>>
                {
                    Success = true,
                    Message = "Lấy danh sách người dùng thành công.",
                    Data = usersMapper,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UserProfile>>
                {
                    Success = false,
                    Message = "UserService - GetUsers: " + ex,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<UserUpdateRequest>> UpdateUser(Guid UserID, UserUpdateRequest user)
        {
            try
            {
                await Task.CompletedTask;

                if (UserID != user.UserID)
                {
                    return new ApiResponse<UserUpdateRequest>
                    {
                        Success = false,
                        Message = "Người dùng không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var userInData = await GetByID(UserID);
                if (userInData is null)
                {
                    return new ApiResponse<UserUpdateRequest>
                    {
                        Success = false,
                        Message = "Người dùng không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                _mapper.Map(user, userInData);


                _context.Users.Update(userInData);
                await _context.SaveChangesAsync();

                return new ApiResponse<UserUpdateRequest>
                {
                    Success = true,
                    Message = "Cập nhật người dùng thành công.",
                    Data = user,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserUpdateRequest>
                {
                    Success = false,
                    Message = "UserService - UpdateUser: " + ex,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        private async Task<User?> GetByID(Guid UserID)
        {
            try
            {
                await Task.CompletedTask;
                return await _context.Users.FindAsync(UserID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse<object>> DeleteUser(Guid UserID)
        {
            try
            {
                await Task.CompletedTask;
                await _context.Database.ExecuteSqlInterpolatedAsync($"sp_delete_user {UserID}");
                await _context.SaveChangesAsync();
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa người dùng thành công.",
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Object>
                {
                    Success = false,
                    Message = "UserService - DeleteUser: " + ex,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public ApiResponse<Object> SendBackupEmail(string recipientEmail, string token)
        {
            string senderEmail = "minh.quang1720@gmail.com";
            string senderPassword = "acanhcamlay2";

            SmtpClient smtpClient = new SmtpClient("smtp.test.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail);
            mailMessage.To.Add(recipientEmail);
            mailMessage.Subject = "Xác thực yêu cầu sao lưu tài khoản";
            mailMessage.Body = $"Vui lòng nhấp vào liên kết sau để xác thực yêu cầu sao lưu tài khoản: {token}";
            mailMessage.IsBodyHtml = false;

            try
            {
                smtpClient.Send(mailMessage);
                return new ApiResponse<Object>
                {
                    Success = true,
                    Message = "Gửi mail thành công",
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Object>
                {
                    Success = false,
                    Message = "UserService - SendBackupRequest: " + ex,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
