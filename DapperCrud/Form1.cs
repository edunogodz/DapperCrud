using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DapperCrud
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=.\SQL2017;Initial Catalog=DapperDB;Integrated Security=True;
Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        int empId = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();

                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@EmpId", empId);
                    param.Add("@Name", txtName.Text.Trim());
                    param.Add("@Mobile", txtMobile.Text.Trim());
                    param.Add("@Address", txtAddress.Text.Trim());
                    con.Execute("EmpAddOrEdit", param, commandType: CommandType.StoredProcedure);
                    if (empId == 0)
                        MessageBox.Show("Saved Successfully");
                    else
                        MessageBox.Show("Updated Successfully");

                    FillDataGridView();
                    Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        void FillDataGridView()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@SearchText", txtSearch.Text.Trim());
            List<Employee> list = con.Query<Employee>("EmpViewOrSearch", param, commandType:
                CommandType.StoredProcedure).ToList<Employee>();

            dgvEmployee.DataSource = list;
            dgvEmployee.Columns[0].Visible = false;

        }

        class Employee
        {
            public int EmpID { get; set; }
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }


        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataGridView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                FillDataGridView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Clear();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtName.Text = txtMobile.Text = txtAddress.Text = "";
            empId = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure to delete this record?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("EmpId", empId);
                    con.Execute("EmpDeleteByID", param, commandType: CommandType.StoredProcedure);
                    Clear();
                    FillDataGridView();
                    MessageBox.Show("Deleted Successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DgvEmployee_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmployee.CurrentRow.Index != -1)
                {
                    empId = Convert.ToInt32(dgvEmployee.CurrentRow.Cells[0].Value.ToString());
                    txtName.Text = dgvEmployee.CurrentRow.Cells[1].Value.ToString();
                    txtMobile.Text = dgvEmployee.CurrentRow.Cells[2].Value.ToString();
                    txtAddress.Text = dgvEmployee.CurrentRow.Cells[3].Value.ToString();
                    btnSave.Text = "Update";
                    btnDelete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
