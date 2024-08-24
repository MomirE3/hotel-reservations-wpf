using System;
using HotelReservations.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Repository;

namespace HotelReservations.Repostiory
{
    public class RoomRepository : IRoomRepository
    {
        public List<Room> GetAll()
        {
            var rooms = new List<Room>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT r.*, rt.* FROM dbo.room r\r\nINNER JOIN dbo.room_type rt ON r.room_type_id = rt.room_type_id";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "room");

                foreach (DataRow row in dataSet.Tables["room"]!.Rows)
                {
                    var room = new Room()
                    {
                        Id = (int)row["room_id"],
                        RoomNumber = (string)row["room_number"],
                        HasTV = (bool)row["has_TV"],
                        HasMiniBar = (bool)row["has_mini_bar"],
                        IsActive = (bool)row["room_is_active"],
                        RoomType = new RoomType()
                        {
                            Id = (int)row["room_type_id"],
                            Name = (string)row["room_type_name"],
                            IsActive = (bool)row["room_type_is_active"]
                        }
                    };

                    rooms.Add(room);
                }
            }

            return rooms;
        }

        public int Insert(Room room)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO dbo.room (room_number, has_TV, has_mini_bar, room_is_active, room_type_id)
                    OUTPUT inserted.room_id
                    VALUES (@room_number, @has_TV, @has_mini_bar, @room_is_active, @room_type_id)
                ";

                command.Parameters.Add(new SqlParameter("room_number", room.RoomNumber));
                command.Parameters.Add(new SqlParameter("has_TV", room.HasTV));
                command.Parameters.Add(new SqlParameter("has_mini_bar", room.HasMiniBar));
                command.Parameters.Add(new SqlParameter("room_is_active", room.IsActive));
                command.Parameters.Add(new SqlParameter("room_type_id", room.RoomType.Id));


                return (int)command.ExecuteScalar();
            }
        }

        public void Update(Room room)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE dbo.room 
                    SET room_number=@room_number, has_TV=@has_TV, has_mini_bar=@has_mini_bar, room_is_active=@room_is_active, room_type_id=@room_type_id
                    WHERE room_id=@room_id
                ";

                command.Parameters.Add(new SqlParameter("room_id", room.Id));
                command.Parameters.Add(new SqlParameter("room_number", room.RoomNumber));
                command.Parameters.Add(new SqlParameter("has_TV", room.HasTV));
                command.Parameters.Add(new SqlParameter("has_mini_bar", room.HasMiniBar));
                command.Parameters.Add(new SqlParameter("room_is_active", room.IsActive));
                command.Parameters.Add(new SqlParameter("room_type_id", room.RoomType.Id));

                command.ExecuteNonQuery();
            }
        }

        public bool RoomIdExists(int roomId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM dbo.room WHERE room_id = @room_id";
                command.Parameters.Add(new SqlParameter("room_id", roomId));

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public void Save(List<Room> roomList)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (Room room in roomList)
                        {
                            if (RoomIdExists(room.Id))
                            {
                                Update(room);
                            }
                            else
                            {
                                room.Id = Insert(room);
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
