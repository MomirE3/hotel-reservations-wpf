using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repostiory
{
    public interface IRoomTypeRepository
    {
        List<RoomType> GetAll();
        int Insert(RoomType roomType);
        void Update(RoomType roomType);
        void Save(List<RoomType> roomTypeList);
    }
}
