using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Service
{
    public class RoomTypeService
    {
        public List<RoomType> GetAllRoomTypes()
        {
            return Hotel.GetInstance().RoomTypes;
        }

        public void SaveRoomType(RoomType roomType)
        {
            if (roomType.Id == 0)
            {
                roomType.Id = GetNextIdValue();
                Hotel.GetInstance().RoomTypes.Add(roomType);
            }
            else
            {
                var index = Hotel.GetInstance().RoomTypes.FindIndex(r => r.Id == roomType.Id);
                Hotel.GetInstance().RoomTypes[index] = roomType;
            }
        }

        public bool RoomTypeExists(string roomTypeName, int roomIdToExclude = 0)
        {
            var roomTypes = GetAllRoomTypes();

            var existingRoomType = roomTypes.FirstOrDefault(rt =>
                rt.Name.Equals(roomTypeName, StringComparison.OrdinalIgnoreCase) &&
                rt.Id != roomIdToExclude);

            return existingRoomType != null;
        }

        public int GetNextIdValue()
        {
            var hotelInstance = Hotel.GetInstance();

            if (hotelInstance.Guests == null || hotelInstance.RoomTypes.Count == 0)
            {
                return 1;
            }

            return hotelInstance.RoomTypes.Max(r => r.Id) + 1;
        }
    }
}
