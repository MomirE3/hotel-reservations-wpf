using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace HotelReservations.Model
{
    [Serializable]
    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IDNumber { get; set; }
        public bool IsActive { get; set; } = true;

        public Guest Clone()
        {
            var clone = new Guest();
            clone.Id = Id;
            clone.Name = Name;
            clone.Surname = Surname;
            clone.IDNumber = IDNumber;
            clone.IsActive = IsActive;

            return clone;
        }
    }
}
