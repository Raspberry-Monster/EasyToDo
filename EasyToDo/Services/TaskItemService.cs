using EasyToDo.Models;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services
{
    public sealed class TaskItemService(EasyToDoDbContext repository)
    {
    }
}
