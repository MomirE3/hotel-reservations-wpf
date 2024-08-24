using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using HotelReservations.Repostiory;

namespace HotelReservations.Service
{
    public class PriceService
    {


        public List<Price> GetAllPrices()
        {
            var prices = new List<Price>();
            foreach (var price in Hotel.GetInstance().PriceList)
            {
                if (price.IsActive)
                {
                    prices.Add(price);
                }
            }
            return prices;
        }
        public void SavePrice(Price price)
        {
            if (price.Id == 0)
            {
                price.Id = GetNextIdValue();
                Hotel.GetInstance().PriceList.Add(price);
            }
            else
            {
                var index = Hotel.GetInstance().PriceList.FindIndex(p => p.Id == price.Id);
                Hotel.GetInstance().PriceList[index] = price;
            }
        }

        public List<RoomType> GetAllRoomTypes()
        {
            return Hotel.GetInstance().RoomTypes.Where(rt => rt.IsActive).ToList();
        }

        public Price GetPrice(int Id)
        {
            foreach (var price in GetAllPrices())
            {
                if (price.Id == Id)
                {
                    return price;
                }
            }
            return null!;
        }

        public void DeletePrice(int Id)
        {
            var priceToDelete = GetPrice(Id);
            if (priceToDelete != null)
            {
                priceToDelete.IsActive = false;
            }
        }

        public bool CanAddPriceForRoomTypeAndReservationType(int roomTypeId, ReservationType reservationType)
        {
            var existingPrice = Hotel.GetInstance().PriceList.FirstOrDefault(p =>
                p.RoomType.Id == roomTypeId &&
                p.ReservationType == reservationType &&
                p.IsActive);

            return existingPrice == null;
        }

        public List<ReservationType> GetAvailableReservationTypesForRoomType(int roomTypeId)
        {
            var existingReservationTypesForRoomType = Hotel.GetInstance().PriceList
                .Where(p => p.RoomType.Id == roomTypeId && p.IsActive)
                .Select(p => p.ReservationType)
                .ToList();

            var allReservationTypes = Enum.GetValues(typeof(ReservationType))
                .Cast<ReservationType>()
                .ToList();

            return allReservationTypes.Except(existingReservationTypesForRoomType).ToList();
        }

        public int GetNextIdValue()
        {
            var hotelInstance = Hotel.GetInstance();

            if (hotelInstance.PriceList == null || hotelInstance.PriceList.Count == 0)
            {
                return 1;
            }

            return hotelInstance.PriceList.Max(r => r.Id) + 1;
        }


    }
}
