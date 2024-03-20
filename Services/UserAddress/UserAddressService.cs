﻿using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Datas.UserAddress;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InoxThanhNamServer.Services.UserAddress
{
    public class UserAddressService : IUserAddressService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;

        public UserAddressService(InoxEcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ApiResponse<AddressProfile>> GetAddressByUser(Guid UserID)
        {
            try
            {
                await Task.CompletedTask;
                var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserID == UserID);
                if (address == null)
                {
                    return new ApiResponse<AddressProfile>
                    {
                        Success = false,
                        Message = "Khách hàng chưa có địa chỉ",
                        Data = null,
                        Status = (int)HttpStatusCode.OK
                    };
                }
                return new ApiResponse<AddressProfile>
                {
                    Success = true,
                    Message = "Địa chỉ khách hàng",
                    Data = _mapper.Map<AddressProfile>(address),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new ApiResponse<AddressProfile>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<AddressUpdateRequest>> UpdateAddress(int AddressID, AddressUpdateRequest address)
        {
            try
            {
                await Task.CompletedTask;
                if (AddressID != address.AddressID)
                {
                    return new ApiResponse<AddressUpdateRequest>
                    {
                        Success = false,
                        Message = "Địa chỉ không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var addressInData = await _context.UserAddresses.FindAsync(AddressID);
                if (addressInData is null)
                {
                    return new ApiResponse<AddressUpdateRequest>
                    {
                        Success = false,
                        Message = "Địa chỉ không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                _mapper.Map(address, addressInData);


                _context.UserAddresses.Update(addressInData);
                await _context.SaveChangesAsync();

                return new ApiResponse<AddressUpdateRequest>
                {
                    Success = true,
                    Message = "Cập nhật địa chỉ thành công.",
                    Data = address,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new ApiResponse<AddressUpdateRequest>
                {
                    Success = false,
                    Message = ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
