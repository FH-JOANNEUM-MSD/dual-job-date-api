﻿using DualJobDate.BusinessLogic.Services.Interface;
using DualJobDate.BusinessObjects.Dto;
using System.Text.Json;

namespace DualJobDate.BusinessLogic.Services;

public class UserService : IUserService
{
    public Task<string> GetUserById(int id)
    {
        var result = JsonSerializer.Serialize(new UserDto(id, "Foo", "Bar"));
        return Task.FromResult(result);
    }
}
