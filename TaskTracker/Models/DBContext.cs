using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TaskTracker.Models
{
    /// <summary>
    /// Класс создан для общения с базой. Здесь вызываются все SQL запросы
    /// </summary>
    public class TasksDBContext
    {
        private string connectionString = "";

        public CheckList CheckList { get { return GetCheckList(); } }

        public List<Task> Tasks
        {
            get { return GetTasks(); }
        }

        public TasksDBContext()
        {
            FillConnectionString();
        }

        /// <summary>
        /// Получаем строку соединения из web.config
        /// </summary>
        private void FillConnectionString()
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/TaskTracker");
            var cs=config.ConnectionStrings.ConnectionStrings["TasksConnectionString"];
            connectionString = cs.ConnectionString;
        }

        public void EditTask(Task task)
        {
            UpdateTask(task);
        }

        public void DeleteTask(Task task)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(string.Format("DELETE FROM Tasks WHERE id={0}", task.Id), con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        private List<Task> GetTasks()
        {
            List<Task> tasks = new List<Task>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM TASKS", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new Task
                    {
                        Id = (int)reader["Id"],
                        TaskText = reader["TaskText"].ToString(),
                        State = (TaskState)(int)reader["Status"],
                        TillDate = (DateTime)reader["TillDate"]
                    });
                }

                con.Close();
            }
            return tasks;
        }

        public Task GetTaskById(int id)
        {
            Task task = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(string.Format("SELECT * FROM TASKS WHERE Id={0}", id), con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    task = new Task
                    {
                        Id = (int)reader["Id"],
                        TaskText = reader["TaskText"].ToString(),
                        State = (TaskState)(int)reader["Status"],
                        TillDate = (DateTime)reader["TillDate"]
                    };
                }

                con.Close();
            }

            return task;
        }

        private void UpdateTask(Task task)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Tasks set TaskText=@text, TillDate=@date, Status=@status WHERE Id=@id", con);
                cmd.Parameters.Add("id", SqlDbType.Int).Value = task.Id;
                cmd.Parameters.Add("text", SqlDbType.VarChar).Value = task.TaskText;
                cmd.Parameters.Add("date", SqlDbType.DateTime).Value = task.TillDate;
                cmd.Parameters.Add("status", SqlDbType.Int).Value = task.State;
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void AddNewTask(Task task)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into tasks values(@text, @date, @status)", con);
                cmd.Parameters.Add("text", SqlDbType.VarChar).Value = task.TaskText;
                cmd.Parameters.Add("date", SqlDbType.DateTime).Value = task.TillDate;
                cmd.Parameters.Add("status", SqlDbType.Int).Value = task.State;
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void UpdateState(int id, int state)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Tasks SET Status=@state WHERE Id=@id", con);
                cmd.Parameters.Add("state", SqlDbType.Int).Value = state;
                cmd.Parameters.Add("id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        private CheckList GetCheckList()
        {
            CheckList cl = new CheckList();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT cl.id as clId, t.* FROM CheckList cl join Tasks t on cl.taskId=t.id", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = (int)reader["clId"];
                    cl.Tasks.Add( new Task
                    {
                        Id = (int)reader["Id"],
                        TaskText = reader["TaskText"].ToString(),
                        State = (TaskState)(int)reader["Status"],
                        TillDate = (DateTime)reader["TillDate"]
                    });
                }
                con.Close();
            }

            return cl;
        }

        public void AddTaskToCheckList(int taskId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO CheckList values(@id)", con);
                cmd.Parameters.Add("id", SqlDbType.Int).Value = taskId;
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void DeleteTaskFromCheckList(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM CheckList WHERE taskId=@id", con);
                cmd.Parameters.Add("id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}