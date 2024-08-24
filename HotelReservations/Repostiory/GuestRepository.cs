using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Exceptions;
using HotelReservations.Model;

namespace HotelReservations.Repostiory
{
    public class GuestRepository : IGuestRepository
    {
        public List<Guest> GetAll()
        {
            var guests = new List<Guest>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM dbo.guest";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "guest");


                foreach (DataRow row in dataSet.Tables["guest"]!.Rows)
                {
                    var guest = new Guest()
                    {
                        Id = (int)row["guest_id"],
                        Name = (string)row["guest_name"],
                        Surname = (string)row["guest_surname"],
                        IDNumber = (string)row["guest_idnumber"],
                        IsActive = (bool)row["guest_is_active"],
                    };

                    guests.Add(guest);
                }
            }

            return guests;
        }

        public int Insert(Guest guest)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO dbo.guest (guest_name, guest_surname, guest_idnumber, guest_is_active)
                    OUTPUT inserted.guest_id
                    VALUES (@guest_name, @guest_surname, @guest_idnumber, @guest_is_active)
                ";

                command.Parameters.Add(new SqlParameter("guest_name", guest.Name));
                command.Parameters.Add(new SqlParameter("guest_surname", guest.Surname));
                command.Parameters.Add(new SqlParameter("guest_idnumber", guest.IDNumber));
                command.Parameters.Add(new SqlParameter("guest_is_active", guest.IsActive));

                return (int)command.ExecuteScalar();
            }
        }

        public void Update(Guest guest)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE dbo.guest 
                    SET guest_name=@guest_name, guest_surname=@guest_surname, guest_idnumber=@guest_idnumber, guest_is_active=@guest_is_active
                    WHERE guest_id=@guest_id
                ";

                command.Parameters.Add(new SqlParameter("guest_id", guest.Id));
                command.Parameters.Add(new SqlParameter("guest_name", guest.Name));
                command.Parameters.Add(new SqlParameter("guest_surname", guest.Surname));
                command.Parameters.Add(new SqlParameter("guest_idnumber", guest.IDNumber));
                command.Parameters.Add(new SqlParameter("guest_is_active", guest.IsActive));

                command.ExecuteNonQuery();
            }
        }

        public bool GuestIdExists(int guestId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM dbo.guest WHERE guest_id = @guest_id";
                command.Parameters.Add(new SqlParameter("guest_id", guestId));

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public void Save(List<Guest> guestList)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (Guest guest in guestList)
                        {
                            if (GuestIdExists(guest.Id))
                            {
                                Update(guest);
                            }
                            else
                            {
                                guest.Id = Insert(guest);
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
