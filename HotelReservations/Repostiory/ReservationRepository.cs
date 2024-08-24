using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HotelReservations.Model;
using HotelReservations.Windows;

namespace HotelReservations.Repostiory
{
    public class ReservationRepository : IReservationRepository
    {
        public List<Reservation> GetAll()
        {
            var reservations = new List<Reservation>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                try
                {
                    conn.Open();

                    var commandText = "SELECT r.*, room.*, rt.* FROM dbo.reservation r INNER JOIN dbo.room ON r.room_Id = room.room_id INNER JOIN dbo.room_type rt ON room.room_type_id = rt.room_type_id\r\n";
                    var commandText2 = "SELECT rg.*, g.* FROM dbo.reservation r INNER JOIN dbo.ReservationGuest rg ON r.reservation_id = rg.reservation_id INNER JOIN dbo.guest g ON rg.guest_id = g.guest_id\r\n";
                    SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(commandText2, conn);

                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "reservation");
                    adapter2.Fill(dataSet, "ReservationGuest");

                    foreach (DataRow reservationRow in dataSet.Tables["reservation"].Rows)
                    {
                        var reservation = new Reservation()
                        {
                            Id = (int)reservationRow["reservation_id"],
                            RoomNumber = new Room()
                            {
                                Id = (int)reservationRow["room_id"],
                                RoomNumber = (string)reservationRow["room_number"],
                                HasTV = (bool)reservationRow["has_TV"],
                                HasMiniBar = (bool)reservationRow["has_mini_bar"],
                                IsActive = (bool)reservationRow["room_is_active"],
                                RoomType = new RoomType()
                                {
                                    Id = (int)reservationRow["room_type_id"],
                                    Name = (string)reservationRow["room_type_name"],
                                    IsActive = (bool)reservationRow["room_type_is_active"]
                                }
                            },
                            ReservationType = reservationRow["reservation_type"] == DBNull.Value ? null : (ReservationType)Enum.Parse(typeof(ReservationType), (string)reservationRow["reservation_type"]),
                            StartDateTime = (DateTime)reservationRow["startDateTime"],
                            EndDateTime = reservationRow["endDateTime"] == DBNull.Value ? null : (DateTime?)reservationRow["endDateTime"],
                            TotalPrice = reservationRow["totalPrice"] == DBNull.Value ? null : (double?)(reservationRow["totalPrice"] as double?),
                            IsActive = (bool)reservationRow["reservation_is_active"],
                            Guests = new List<Guest>()
                        };


                        foreach (DataRow guestRow in dataSet.Tables["ReservationGuest"].Rows)
                        {
                            if ((int)guestRow["reservation_id"] == reservation.Id)
                            {
                                Guest guest = new Guest()
                                {
                                    Id = (int)guestRow["guest_id"],
                                    Name = (string)guestRow["guest_name"],
                                    Surname = (string)guestRow["guest_surname"],
                                    IDNumber = (string)guestRow["guest_idnumber"],
                                    IsActive = (bool)guestRow["guest_is_active"],
                                };
                                reservation.Guests.Add(guest);

                            }
                        }

                        reservations.Add(reservation);
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
            }

            return reservations;
        }

        public int InsertReservation(Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO dbo.reservation (room_id, reservation_type, startDateTime, endDateTime, totalPrice, reservation_is_active)
                    OUTPUT inserted.reservation_id
                    VALUES (@room_id, @reservation_type, @startDateTime, @endDateTime, @totalPrice, @reservation_is_active)
                ";

                command.Parameters.Add(new SqlParameter("room_id", reservation.RoomNumber.Id));
                command.Parameters.Add(new SqlParameter("reservation_type", (object)reservation.ReservationType ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("startDateTime", reservation.StartDateTime));
                command.Parameters.Add(new SqlParameter("endDateTime", (object)reservation.EndDateTime ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("totalPrice", (object)reservation.TotalPrice ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("reservation_is_active", reservation.IsActive));

                return (int)command.ExecuteScalar();
            }
        }

        public int InsertReservationGuest(Reservation reservation, Guest guest)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();

                command.CommandText = @"
                    INSERT INTO dbo.ReservationGuest (reservation_id, guest_id)
                    OUTPUT inserted.reservation_id, inserted.guest_id
                    VALUES (@reservation_id, @guest_id)
                ";


                command.Parameters.Add(new SqlParameter("reservation_id", reservation.Id));
                command.Parameters.Add(new SqlParameter("guest_id", guest.Id));


                return (int)command.ExecuteScalar();
            }
        }

        public bool ReservationIdExists(int reservationId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM dbo.reservation WHERE reservation_id = @reservation_id";
                command.Parameters.Add(new SqlParameter("reservation_id", reservationId));

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public void Save(List<Reservation> reservationList)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (Reservation reservation in reservationList)
                        {
                            if (ReservationIdExists(reservation.Id))
                            {
                                Update(reservation);
                            }
                            else
                            {
                                reservation.Id = InsertReservation(reservation);

                                foreach (var guest in reservation.Guests)
                                {
                                    InsertReservationGuest(reservation, guest);
                                }
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


        public void Update(Reservation reservationList)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE dbo.reservation 
                    SET room_id=@room_id, reservation_type=@reservation_type, startDateTime=@startDateTime, endDateTime=@endDateTime, totalPrice=@totalPrice, reservation_is_active=@reservation_is_active
                    WHERE reservation_id=@reservation_id
                "
                ;

                command.Parameters.Add(new SqlParameter("reservation_id", reservationList.Id));
                command.Parameters.Add(new SqlParameter("room_id", reservationList.RoomNumber.Id));
                command.Parameters.Add(new SqlParameter("reservation_type", (object)reservationList.ReservationType ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("startDateTime", reservationList.StartDateTime));
                command.Parameters.Add(new SqlParameter("endDateTime", (object)reservationList.EndDateTime ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("totalPrice", (object)reservationList.TotalPrice ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("reservation_is_active", reservationList.IsActive));

                command.ExecuteNonQuery();
            }
        }
    }
}
