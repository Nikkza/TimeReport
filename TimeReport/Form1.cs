using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TimeReport
{
    public partial class Form1 : Form
    {
        private string _ConnectionString = Properties.Settings.Default.TimeReportConnectionString;
        public Form1()
        {
            InitializeComponent();
            ShowMembers();
            ShowProjects();
            ShowWeeks();
        }

        public void GetTimeTable()
        {
            var name = Choice.SelectedIndex + 1;
            var b = Choice.SelectedItem.ToString();
            var d = b.Split();

            List<string> tempList = new List<string>();
            foreach (var i in d)
            {
                if (i != " ")
                {
                    tempList.Add(i);
                }
            }

            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = "ShowTimeCardSP_ID";
                cmd.Parameters.Add("@personid", SqlDbType.Int).Value = name;
                cmd.Parameters.Add("@Firstname", SqlDbType.NVarChar).Value = tempList[0];
                cmd.Parameters.Add("@Lastname", SqlDbType.NVarChar).Value = tempList[1];
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(ds, "TimeTable");

                listBox1.Items.Clear();
                if (ds != null)
                {
                    foreach (DataRow row in ds.Tables["TimeTable"].Rows)
                    {
                        listBox1.Items.Add("Week:" + " " + row[0].ToString().PadRight(10) + " Hour:" + " " + row[1].ToString().PadRight(10) + " Project:" + " " + row[2]);
                    }
                }
            }
        }

        public void ShowProjects()
        {
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = "ShowProjectsSP";
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(ds, "Projects");
                if (ds != null)
                {
                    foreach (DataRow row in ds.Tables["Projects"].Rows)
                    {
                        comboBoxProjects.Items.Add(row[0]);
                    }
                }
            }
        }

        public void ShowWeeks()
        {
            List<int> weeks = new List<int>();
            int k = 1;
            while(k <= 52)
            {
                weeks.Add(k);
                k += 1;
            }

            foreach (var i in weeks)
            {
                comboBoxWeek.Items.Add(i);
            }
        }

        public void ShowMembers()
        {
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = "SpQueryMembers";
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(ds, "Anställd");
                if (ds != null)
                {
                    foreach (DataRow row in ds.Tables["Anställd"].Rows)
                    {
                        Choice.Items.Add(row[0] + " " + row[1]);
                    }
                }
            }
        }

        private void Choice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Choice.SelectedIndex != -1))
            {
                GetTimeTable();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                var projects = comboBoxProjects.SelectedIndex + 1;
                var name = Choice.SelectedIndex + 1;
                var week = comboBoxWeek.SelectedItem;
                var hour = Int32.Parse(textBoxHour.Text);

                SqlCommand cmd = new SqlCommand();
                connection.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ny_kund";
                cmd.Parameters.Add("@week", SqlDbType.Int).Value = week;
                cmd.Parameters.Add("@hour", SqlDbType.Int).Value = hour;
                cmd.Parameters.Add("@personId", SqlDbType.Int).Value = name;
                cmd.Parameters.Add("@projectId", SqlDbType.Int).Value = projects;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
                listBox1.Items.Clear();
            }
            Choice_SelectedIndexChanged(this, e);
            comboBoxProjects.ResetText();
            comboBoxWeek.ResetText();
            textBoxHour.Clear();
        }
    }
}
