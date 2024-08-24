using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repostiory
{
    public interface IPriceRepository
    {
        List<Price> GetAll();
        int Insert(Price price);
        void Update(Price price);
        void Save(List<Price> priceList);
    }
}
