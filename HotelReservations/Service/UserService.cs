
using HotelReservations.Model;
using HotelReservations.Repostiory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Service
{
    public class UserService
    {

        private IUserRepository userRepository;

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            foreach (var user in Hotel.GetInstance().Users) 
            {
                if(user.isActive) 
                {
                    users.Add(user);
                }
            }
            return users;
        }
        public void SaveUser(User user)
        {
            if (user.Id == 0)
            {
                user.Id = GetNextIdValue();
                Hotel.GetInstance().Users.Add(user);
            }
            else
            {
                var index = Hotel.GetInstance().Users.FindIndex(u => u.Id == user.Id);
                Hotel.GetInstance().Users[index] = user;
            }
        }

        public bool UserJMBGExists(string JMBG, int userIDToExclude)
        {
            var users = Hotel.GetInstance().Users.Where(u => u.Id != userIDToExclude);

            return users.Any(u => u.JMBG == JMBG);
        }

        public bool UserUsernameExists(string Username, int userIDToExclude)
        {
            var users = Hotel.GetInstance().Users.Where(u => u.Id != userIDToExclude);

            return users.Any(u => u.Username == Username);
        }
        public int GetNextIdValue()
        {
            var hotelInstance = Hotel.GetInstance();

            if (hotelInstance.Users == null || hotelInstance.Users.Count == 0)
            {
                return 1;
            }

            return hotelInstance.Users.Max(r => r.Id) + 1;
        }
    }
}
