using HotelReservations.Model;
using HotelReservations.Repository;
using HotelReservations.Repostiory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Service
{
    public class RoomService
    {
        IRoomRepository roomRepository;
        public RoomService()
        {
            roomRepository = new RoomRepository();
        }

        public List<Room> GetAllRooms()
        {
            return Hotel.GetInstance().Rooms;
        }

        public List<RoomType> GetAllRoomTypes()
        {
            return Hotel.GetInstance().RoomTypes.Where(rt => rt.IsActive).ToList();
        }

        public void SaveRoom(Room room)
        {
            if (room.Id == 0)
            {
                room.Id = GetNextIdValue();
                Hotel.GetInstance().Rooms.Add(room);
            }
            else
            {
                var index = Hotel.GetInstance().Rooms.FindIndex(r => r.Id == room.Id);
                Hotel.GetInstance().Rooms[index] = room;
            }
        }

        public bool RoomNumberExists(string roomNumber, int roomIdToExclude)
        {
            var rooms = Hotel.GetInstance().Rooms.Where(r => r.Id != roomIdToExclude);

            return rooms.Any(r => r.RoomNumber == roomNumber);
        }
        public List<RoomType> GetIncludedRoomTypes(int roomTypeId)
        {
            var roomsWithRoomType = Hotel.GetInstance().Rooms
                .Where(r => r.RoomType.Id == roomTypeId)
                .Select(r => r.RoomType)
                .ToList();

            return roomsWithRoomType;
        }


        public int GetNextIdValue()
        {
            var hotelInstance = Hotel.GetInstance();

            if (hotelInstance.Rooms == null || hotelInstance.Rooms.Count == 0)
            {
                return 1; 
            }

            return hotelInstance.Rooms.Max(r => r.Id) + 1;
        }

    }
}
