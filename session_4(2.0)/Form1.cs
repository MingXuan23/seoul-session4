using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace session_4_2._0_
{
    public partial class Form1 : Form
    {

        ComXEntities ent=new ComXEntities();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            dateTimePicker1.Value = dateTimePicker1.MinDate;
            listView1.Visible = false;
            textBox1.Text = "Enter area name,attraction,property title,property type,amenities...";
            textBox1.ForeColor = Color.Gray;
            label1.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(dateTimePicker1.Value==dateTimePicker1.MinDate)
            {
                MessageBox.Show("Please select the date");
                return;
            }
            groupBox2.Visible = true;
            this.Text = "Seoul Stay - Search Results";
            loadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form=Application.OpenForms["Advanced"];
            if(form!=null)
            {
                form.Show();
            }
            else
            {
                new Advanced().Show();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            /*textBox1.Text = "Enter area name,attraction,property title,property type,amenities...";
            textBox1.ForeColor = Color.Gray;*/
            listView1.Visible = false;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.ForeColor==Color.Gray)
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 3 && textBox1.ForeColor==Color.Black)
            {
                listView1.Visible = true;
                listView1.Items.Clear();
                string keyword = textBox1.Text.ToLower();

                var list = ent.Areas.Where(x => x.Name.ToLower().Contains(keyword)).ToList();
                foreach(var item in list)
                {
                    ListViewItem listItem = new ListViewItem();
                    listItem.Text=item.Name;
                    listItem.SubItems.Add("Area");
                    listView1.Items.Add(listItem);

                }

                var list2 = ent.Attractions.Where(x => x.Name.ToLower().Contains(keyword)).ToList();
                foreach (var item in list2)
                {
                    ListViewItem listItem = new ListViewItem();
                    listItem.Text = item.Name;
                    listItem.SubItems.Add("Attraction");
                    listView1.Items.Add(listItem);

                }

                var list3 = ent.Items.Where(x => x.Title.ToLower().Contains(keyword)).ToList();
                foreach (var item in list3)
                {
                    ListViewItem listItem = new ListViewItem();
                    listItem.Text = item.Title;
                    listItem.SubItems.Add("Listing");
                    listView1.Items.Add(listItem);

                }

                var list4 = ent.ItemTypes.Where(x => x.Name.ToLower().Contains(keyword)).ToList();
                foreach (var item in list4)
                {
                    ListViewItem listItem = new ListViewItem();
                    listItem.Text = item.Name;
                    listItem.SubItems.Add("Item Type");
                    listView1.Items.Add(listItem);

                }

                var list5 = ent.Amenities.Where(x => x.Name.ToLower().Contains(keyword)).ToList();
                foreach (var item in list5)
                {
                    ListViewItem listItem = new ListViewItem();
                    listItem.Text = item.Name;
                    listItem.SubItems.Add("Amenity");
                    listView1.Items.Add(listItem);
                }

                foreach (var item in listView1.Items)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                listView1.Visible = false;
            }
        }

        public void loadData()
        {
            string keyword = textBox1.Text.ToLower();
            if (textBox1.ForeColor == Color.Gray || textBox1.Text=="")
                keyword = "";

            var list = ent.Items.Where(x => x.Area.Name.ToLower().Contains(keyword)).ToList();
            var tlist = ent.ItemAttractions.Where(x => x.Attraction.Name.ToLower().Contains(keyword)).Select(x => x.Item).ToList();

            list = list.Union(tlist).ToList();

            tlist = ent.Items.Where(x => x.Title.ToLower().Contains(keyword)).ToList();
            list = list.Union(tlist).ToList();

            tlist = ent.Items.Where(x => x.ItemType.Name.ToLower().Contains(keyword)).ToList();
            list = list.Union(tlist).ToList();

            tlist = ent.ItemAmenities.Where(x => x.Amenity.Name.ToLower().Contains(keyword)).Select(x => x.Item).ToList();
            list = list.Union(tlist).ToList();

            list=list.Where(x => x.ItemPrices.Select(y=>y.Date).Contains( dateTimePicker1.Value)).ToList();
            list = list.Where(x => x.MinimumNights <= int.Parse(numericUpDown1.Value.ToString())&& x.MaximumNights >= int.Parse(numericUpDown1.Value.ToString()) && x.Capacity >= int.Parse(numericUpDown2.Value.ToString())).ToList();
            list=list.OrderBy(x=>x.Title).ToList();

            dataGridView1.Rows.Clear();
            foreach(var i in list)
            {
                var row = new object[6];
                row[0] = i.ID;
                row[1] = i.Title;
                row[2] = i.Area.Name;
                row[3] = i.ItemScores.Select(x=>x.Value).Any()? decimal.Round((decimal)i.ItemScores.Select(x => x.Value).ToList().Average(),2).ToString():"-";
                row[4]=i.ItemPrices.Where(x=>x.Date<=DateTime.Now &&!(x.BookingDetails.Select(y=>y.isRefund).Contains(true))).ToList().Count;
                var price = i.ItemPrices.Where(x => x.Date == dateTimePicker1.Value).FirstOrDefault();
                if (price != null)
                    row[5] = price.Price;
                else
                    row[5] = "-";
                dataGridView1.Rows.Add(row);

            }

            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if(row.Index%2==0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            label5.Text = $"Displaying {dataGridView1.RowCount - 1} option";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string keyword = listView1.SelectedItems[0].Text.ToString();
            textBox1.Text = keyword;
        }


    }
}
