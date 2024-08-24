using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
namespace HotelReservations
{
    public class Config
    {
        public static string CONNECTION_STRING { get; } = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog = POPSR8/2022; Integrated Security = True; Connect Timeout = 30; Encrypt=False";
    }
}
