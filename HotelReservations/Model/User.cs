using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HotelReservations.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string JMBG { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType
        {
            get;
            set;
        }
        public bool isActive { get; set; } = true;

        public User() { }
        public User(int id, string name, string surname, string jMBG, string username, string password, bool IsActive)
        {
            Id = id;
            Name = name;
            Surname = surname;
            JMBG = jMBG;
            Username = username;
            Password = password;
            isActive = IsActive;
        }

        public User Clone()
        {
            var clone = new User();
            clone.Id = Id;
            clone.Name = Name;
            clone.Surname = Surname;
            clone.JMBG = JMBG;
            clone.Username = Username;
            clone.Password = Password;
            clone.UserType = UserType;
            clone.isActive = isActive;
            return clone;
        }

        public override string ToString()
        {
            return $"User ID: {Id}, Name: {Name}, Surname: {Surname}, JMBG: {JMBG}, Username: {Username}, Password: {Password}, User Type: {UserType}";
        }
    }

    

}
