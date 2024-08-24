using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repostiory
{
    public interface IUserRepository
    {
        List<User> GetAll();
        int Insert(User user);
        void Update(User user);
        void Save(List<User> userList);
    }
}
