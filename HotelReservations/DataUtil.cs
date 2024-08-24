using HotelReservations.Exceptions;
using HotelReservations.Model;
using HotelReservations.Repository;
using HotelReservations.Repostiory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations
{
    public class DataUtil
    {
        public static void LoadData()
        {
            Hotel hotel = Hotel.GetInstance();
            hotel.Id = 1;
            hotel.Name = "Hotel Park";
            hotel.Address = "Kod Futoskog parka...";

            try
            {
                IRoomTypeRepository roomTypeRepository = new RoomTypeRepository();
                var loadedRoomTypes = roomTypeRepository.GetAll();

                if (loadedRoomTypes != null)
                {
                    Hotel.GetInstance().RoomTypes = loadedRoomTypes;
                }

                IRoomRepository roomRepository = new RoomRepository();
                var loadedRooms = roomRepository.GetAll();

                if (loadedRooms != null)
                {
                    Hotel.GetInstance().Rooms = loadedRooms;
                }

                IUserRepository userRepository = new UserRepository();
                var loadedUsers = userRepository.GetAll();

                if (loadedUsers != null)
                {
                    Hotel.GetInstance().Users = loadedUsers;
                }

                IGuestRepository guestRepository = new GuestRepository();
                var loadedGuests = guestRepository.GetAll();

                if (loadedGuests != null)
                {
                    Hotel.GetInstance().Guests = loadedGuests;
                }

                IPriceRepository priceRepository = new PriceRepository();
                var loadedPrices = priceRepository.GetAll();

                if (loadedRoomTypes != null)
                {
                    Hotel.GetInstance().PriceList = loadedPrices;
                }

                IReservationRepository reservationRepository = new ReservationRepository();
                var loadedReservations = reservationRepository.GetAll();

                if (loadedReservations != null)
                {
                    Hotel.GetInstance().Reservations = loadedReservations;
                }



            }
            catch (CouldntLoadResourceException)
            {
                Console.WriteLine("Call an administrator, something weird is happening with the files");
            }
            catch (Exception ex)
            {
                Console.Write("An unexpected error occured", ex.Message);
            }
        }

        public static void PersistData()
        {
            try
            {

                IRoomTypeRepository roomTypeRepository = new RoomTypeRepository();
                roomTypeRepository.Save(Hotel.GetInstance().RoomTypes);

                IRoomRepository roomRepository = new RoomRepository();
                roomRepository.Save(Hotel.GetInstance().Rooms);

                IUserRepository userRepository = new UserRepository();
                userRepository.Save(Hotel.GetInstance().Users);         

                IGuestRepository guestRepository = new GuestRepository();
                guestRepository.Save(Hotel.GetInstance().Guests);

                IPriceRepository priceRepository = new PriceRepository();
                priceRepository.Save(Hotel.GetInstance().PriceList);

                IReservationRepository reservationRepository = new ReservationRepository();
                reservationRepository.Save(Hotel.GetInstance().Reservations);

            }
            catch (CouldntPersistDataException)
            {
                Console.WriteLine("Call an administrator, something weird is happening with the files");
            }
        }
    }
}