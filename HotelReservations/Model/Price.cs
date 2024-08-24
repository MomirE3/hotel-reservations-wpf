using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HotelReservations.Model
{
    public class Price
    {
        public int Id { get; set; }
        public RoomType RoomType { get; set; }
        public ReservationType ReservationType { get; set; }
        public float PriceValue { get; set; }
        public bool IsActive { get; set; } = true;

        public Price Clone()
        {
            var clone = new Price();
            clone.Id = Id;
            clone.RoomType = RoomType;
            clone.ReservationType = ReservationType;
            clone.PriceValue = PriceValue;
            clone.IsActive = IsActive;

            return clone;
        }
    }


    
}
