using HotelReservations.Model;
using HotelReservations.Repository;
using HotelReservations.Repostiory;
using HotelReservations.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelReservations.Service
{
    public class GuestService
    {
        private IGuestRepository guestRepository;

        public GuestService()
        {
            guestRepository = new GuestRepository();
        }

        public List<Guest> GetAllGuest()
        {
            var guestList = new List<Guest>();

            foreach (var guest in Hotel.GetInstance().Guests)
            {
                {
                    if (guest.IsActive)
                    {
                        guestList.Add(guest);
                    }
                }

            }
            return guestList;
        }

        public Guest GetAllGuestById(int id)
        {
            return Hotel.GetInstance().Guests.First(guest => guest.Id == id);
        }

        public Guest GetGuest(string IdNUmber)
        {
            foreach (var guest in GetAllGuest())
            {
                if (guest.IDNumber == IdNUmber)
                {
                    return guest;
                }
            }
            return null!;
        }

        public Guest GetGuestId(int Id)
        {
            foreach (var guest in GetAllGuest())
            {
                if (guest.Id == Id)
                {
                    return guest;
                }
            }
            return null!;
        }

        public bool GuestIDNumberExists(string IDNumber, int guestIDNumberToExclude)
        {
            var guests = Hotel.GetInstance().Guests.Where(g => g.Id != guestIDNumberToExclude);

            return guests.Any(g => g.IDNumber == IDNumber);
        }

        public void DeleteGuest(string IdNumber) 
        {
            var guestToDelete = GetGuest(IdNumber);
            if(guestToDelete != null)
            {
                guestToDelete.IsActive = false;
            }
        }

        public void SaveGuest(Guest guest)
        {

            if (guest.Id == 0)
            {
                guest.Id = GetNextIdValue();
                Hotel.GetInstance().Guests.Add(guest); 
            }
            else
            {
                var index = Hotel.GetInstance().Guests.FindIndex(u => u.Id == guest.Id);
                Hotel.GetInstance().Guests[index] = guest;
            }
        }

        public int GetNextIdValue()
        {
            var hotelInstance = Hotel.GetInstance();

            if (hotelInstance.Guests == null || hotelInstance.Guests.Count == 0)
            {
                return 1;
            }

            return hotelInstance.Guests.Max(r => r.Id) + 1;
        }
    }
}
