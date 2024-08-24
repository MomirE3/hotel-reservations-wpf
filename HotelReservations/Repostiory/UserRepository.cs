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
    public class UserRepository : IUserRepository
    {
        public List<User> GetAll()
        {
            var users = new List<User>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM [dbo].[user]";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "user");

                foreach (DataRow row in dataSet.Tables["user"]!.Rows)


                {
                    var user = new User()
                    {
                        Id = (int)row["user_id"],
                        Name = (string)row["first_name"],
                        Surname = (string)row["last_name"],
                        JMBG = (string)row["JMBG"],
                        Username = (string)row["username"],
                        Password = (string)row["password"],
                        UserType = (string)row["user_type"],
                        isActive = (bool)row["user_is_active"],
                    };

                    users.Add(user);
                }
            }

            return users;
        }

        public int Insert(User user)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [dbo].[user] (first_name, last_name, JMBG, username, password, user_type, user_is_active)
                    OUTPUT inserted.user_id
                    VALUES (@first_name, @last_name, @JMBG, @username, @password, @user_type, @user_is_active)
                ";

                command.Parameters.Add(new SqlParameter("first_name", user.Name));
                command.Parameters.Add(new SqlParameter("last_name", user.Surname));
                command.Parameters.Add(new SqlParameter("JMBG", user.JMBG));
                command.Parameters.Add(new SqlParameter("username", user.Username));
                command.Parameters.Add(new SqlParameter("password", user.Password));
                command.Parameters.Add(new SqlParameter("user_type", user.UserType));
                command.Parameters.Add(new SqlParameter("user_is_active", user.isActive));

                return (int)command.ExecuteScalar();
            }
        }

        public void Update(User user)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE [dbo].[user] 
                    SET first_name=@first_name, last_name=@last_name, JMBG=@JMBG, username=@username, password=@password, user_type=@user_type, user_is_active=@user_is_active
                    WHERE user_id=@user_id
                ";

                command.Parameters.Add(new SqlParameter("user_id", user.Id));
                command.Parameters.Add(new SqlParameter("first_name", user.Name));
                command.Parameters.Add(new SqlParameter("last_name", user.Surname));
                command.Parameters.Add(new SqlParameter("JMBG", user.JMBG));
                command.Parameters.Add(new SqlParameter("username", user.Username));
                command.Parameters.Add(new SqlParameter("password", user.Password));
                command.Parameters.Add(new SqlParameter("user_type", user.UserType));
                command.Parameters.Add(new SqlParameter("user_is_active", user.isActive));

                command.ExecuteNonQuery();
            }
        }

        public bool UserIdExists(int userId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM [dbo].[user] WHERE user_id = @user_id";
                command.Parameters.Add(new SqlParameter("user_id", userId));

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public void Save(List<User> userList)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (User user in userList)
                        {
                            if (UserIdExists(user.Id))
                            {
                                Update(user);
                            }
                            else
                            {
                                user.Id = Insert(user);
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
