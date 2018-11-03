﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIL;

namespace SimCityHelper
{
    public partial class Form1 : Form
    {
        private TestBIL bil = new TestBIL();
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(System.Object sender, System.EventArgs e)
        {
            SetupDataGridView();

            this.dataGridView1.DataSource = bindingSource1;
            PopulateDataGridView();
        }

        private void SetupDataGridView()
        {
            //5列数据
            //this.dataGridView1.ColumnCount = 5;

            //列头样式
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font(this.dataGridView1.Font, FontStyle.Bold);

            //边框颜色
            this.dataGridView1.GridColor = Color.Red;
            this.dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical;
            this.dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            this.dataGridView1.RowHeadersVisible = false;

            //自动换行
            this.dataGridView1.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //列名
            /*
            this.dataGridView1.Columns[0].Name = "材料名称";
            this.dataGridView1.Columns[1].Name = "生产时间";
            this.dataGridView1.Columns[2].Name = "最低卖出价格";
            this.dataGridView1.Columns[3].Name = "最高卖出价格";
            this.dataGridView1.Columns[4].Name = "最低买入价格";
            */

            this.dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            this.dataGridView1.MultiSelect = false;

        }

        private void PopulateDataGridView()
        {
            //填充数据
            bindingSource1.DataSource = bil.GetDataFromDatabase();
            
            //自动调整列宽
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);

        }



    }
}
