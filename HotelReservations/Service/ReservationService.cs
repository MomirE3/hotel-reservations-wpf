using HotelReservations.Model;
using HotelReservations.Repository;
using HotelReservations.Repostiory;
using HotelReservations.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HotelReservations.Service
{
    public class ReservationService
    {
        private readonly PriceService priceService;
        IReservationRepository reservationRepository;
        public ReservationService()
        {
            reservationRepository = new ReservationRepository();
            priceService = new PriceService();
        }
        public List<Reservation> GetAllReservations()
        {
            var reservations = new List<Reservation>();
            foreach (var reservation in Hotel.GetInstance().Reservations)
            {
                if (reservation.IsActive)
                {
                    reservations.Add(reservation);
                }
            }
            return reservations;
        }

        static List<Room> GetUniqueElements(List<Room> list1, List<Room> list2)
        {
            List<Room> uniqueElements = list1.Except(list2).ToList();

            return uniqueElements;
        }
        public List<Room> GetAllRoomNumbers()
        {
            var rooms = Hotel.GetInstance().Rooms.Where(r => r.IsActive).ToList();
            var takenRoomNumbers = new List<Room>();
            var reservationService = new ReservationService();
            var allReservations = reservationService.GetAllReservations().Where(r => r.EndDateTime == null).ToList();
            foreach (var room in rooms)
            {
                foreach(var reservation in allReservations)
                {
                    if (reservation.RoomNumber.Id == room.Id) 
                    {
                        takenRoomNumbers.Add(room);
                    }
                }           
            }
            List<Room> uniqueElements = GetUniqueElements(rooms, takenRoomNumbers);
            return uniqueElements;
        }

        public void SaveReservation(Reservation reservation)
        {
            if (reservation.Id == 0)
            {
                reservation.Id = GetNextIdValue();
                Hotel.GetInstance().Reservations.Add(reservation);
            }
            else
            {
                var index = Hotel.GetInstance().Reservations.FindIndex(r => r.Id == reservation.Id);
                Hotel.GetInstance().Reservations[index] = reservation;
            }
        }

        public Reservation GetReservationById(int reservationId)
        {
            return Hotel.GetInstance().Reservations.FirstOrDefault(r => r.Id == reservationId && r.IsActive);
        }
        public int GetNextIdValue()
        {
            var hotelInstance = Hotel.GetInstance();

            if (hotelInstance.Reservations == null || hotelInstance.Reservations.Count == 0)
            {
                return 1;
            }

            return hotelInstance.Reservations.Max(r => r.Id) + 1;
        }
    }
}