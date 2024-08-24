using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repostiory
{
    public interface IReservationRepository
    {
        List<Reservation> GetAll();
        int InsertReservation(Reservation reservationList);
        int InsertReservationGuest(Reservation reservationList, Guest guest);
        void Update(Reservation reservationList);
        void Save(List<Reservation> reservationList);
    }
}
