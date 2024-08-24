using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repostiory
{
    public interface IGuestRepository
    {
        List<Guest> GetAll();
        int Insert(Guest guest);
        void Update(Guest guest);
        void Save(List<Guest> guestList);
    }
}
