using System;
using HotelReservations.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Repository;
using HotelReservations.Windows;

namespace HotelReservations.Repostiory
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        public List<RoomType> GetAll()
        {
            var roomTypes = new List<RoomType>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM dbo.room_type";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "room_type");               

                foreach (DataRow row in dataSet.Tables["room_type"]!.Rows)
                {
                    var roomType = new RoomType()
                    {
                        Id = (int)row["room_type_id"],
                        Name = (string)row["room_type_name"],
                        IsActive = (bool)row["room_type_is_active"]
                    };

                    roomTypes.Add(roomType);
                }
            }

            return roomTypes;
        }

        public int Insert(RoomType roomType)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO dbo.room_type (room_type_name, room_type_is_active)
                    OUTPUT inserted.room_type_id
                    VALUES (@room_type_name,@room_type_is_active)
                ";

                command.Parameters.Add(new SqlParameter("room_type_name", roomType.Name));
                command.Parameters.Add(new SqlParameter("room_type_is_active", roomType.IsActive));
                return (int)command.ExecuteScalar();
            }
        }

        public void Update(RoomType roomType)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE dbo.room_type
                    SET room_type_name=@room_type_name, room_type_is_active=@room_type_is_active
                    WHERE room_type_id=@room_type_id
                ";

                command.Parameters.Add(new SqlParameter("room_type_id", roomType.Id));
                command.Parameters.Add(new SqlParameter("room_type_name", roomType.Name));
                command.Parameters.Add(new SqlParameter("room_type_is_active", roomType.IsActive));
                command.ExecuteNonQuery();
            }
        }

        public bool RoomTypeIdExists(int roomTypeId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM dbo.room_type WHERE room_type_id = @room_type_id";
                command.Parameters.Add(new SqlParameter("room_type_id", roomTypeId));

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public void Save(List<RoomType> roomTypeList)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (RoomType roomType in roomTypeList)
                        {
                            if (RoomTypeIdExists(roomType.Id))
                            {
                                Update(roomType);
                            }
                            else
                            {
                                roomType.Id = Insert(roomType);
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
