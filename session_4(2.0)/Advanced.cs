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
    public partial class Advanced : Form
    {
        ComXEntities ent=new ComXEntities();
        int resetn = 0;
        public Advanced()
        {
            InitializeComponent();

        }

        private void Advanced_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'comXDataSet4.Amenities' table. You can move, or remove it, as needed.
            this.amenitiesTableAdapter2.Fill(this.comXDataSet4.Amenities);
            // TODO: This line of code loads data into the 'comXDataSet3.Amenities' table. You can move, or remove it, as needed.
            this.amenitiesTableAdapter1.Fill(this.comXDataSet3.Amenities);
            // TODO: This line of code loads data into the 'comXDataSet2.Amenities' table. You can move, or remove it, as needed.
            this.amenitiesTableAdapter.Fill(this.comXDataSet2.Amenities);
            // TODO: This line of code loads data into the 'comXDataSet1.ItemTypes' table. You can move, or remove it, as needed.
            this.itemTypesTableAdapter.Fill(this.comXDataSet1.ItemTypes);
            // TODO: This line of code loads data into the 'comXDataSet.Areas' table. You can move, or remove it, as needed.
            this.areasTableAdapter.Fill(this.comXDataSet.Areas);
            reset();
            comboBox2.Visible = comboBox3.Visible = false;

        }

        public void reset()
        {
            foreach (Control control in groupBox1.Controls)
            {
                if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;
                    comboBox.SelectedIndex = -1;
                }
                if (control is DateTimePicker)
                {
                    DateTimePicker datePicker = (DateTimePicker)control;
                    datePicker.Value = datePicker.MinDate;
                }
                if (control is NumericUpDown)
                {
                    NumericUpDown numericUpDown = (NumericUpDown)control;
                    numericUpDown.Value = numericUpDown.Minimum;
                }
                if(control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Visible = true;
                    textBox.Text = "";
                }
                
                

            }
            resetn++;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            reset();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<string>keyword=new List<string>();
            

            var list=ent.Items.ToList();
            if (comboBox1.SelectedIndex != -1)
            {
                var id = long.Parse(comboBox1.SelectedValue.ToString());
                var area = ent.Items.Where(x => x.AreaID == id).ToList();
                list=list.Intersect(area).ToList();
            }
            if (textBox1.Visible == true)
            {
                if(textBox1.Text!="")
                {
                    var att = ent.ItemAttractions.Where(x => x.Attraction.Name.ToLower().Contains(textBox1.Text.ToLower())).Select(x => x.Item).ToList();
                    list = list.Intersect(att).ToList();
                }
               
                if (textBox2.Text != "")
                {
                    var item = ent.Items.Where(x => x.Title.ToLower().Contains(textBox2.Text.ToLower())).ToList();
                    list = list.Intersect(item).ToList();
                }            
            }
            else
            {
                if (comboBox2.SelectedIndex != -1)
                {
                    var att = ent.ItemAttractions.Where(x => x.Attraction.Name.ToLower().Contains(comboBox2.SelectedItem.ToString().ToLower())).Select(x => x.Item).ToList();
                    list = list.Intersect(att).ToList();
                }
                if (comboBox3.SelectedIndex != -1)
                {
                    var item = ent.Items.Where(x => x.Title.ToLower().Contains(comboBox3.SelectedItem.ToString().ToLower())).ToList();
                    list = list.Intersect(item).ToList();
                }
            }
               
               

            
            var start = dateTimePicker1.Value;
            var end = (dateTimePicker2.Value==dateTimePicker2.MinDate)?dateTimePicker2.MaxDate :dateTimePicker2.Value;
            list=list.Where(x => x.ItemPrices.Select(y => y.Date >= start && y.Date <= end).Any()).ToList();

            list = list.Where(x => x.Capacity >= int.Parse(numericUpDown1.Value.ToString()) &&x.MinimumNights<= int.Parse(numericUpDown2.Value.ToString()) && x.MaximumNights >= int.Parse(numericUpDown3.Value.ToString())).ToList();

            

            if(comboBox4.SelectedIndex!=-1)
            {
                list = list.Where(x => x.ItemTypeID == long.Parse(comboBox4.SelectedValue.ToString())).ToList();
            }
            ComboBox[] cb = new ComboBox[] { comboBox5, comboBox6, comboBox7 };
            foreach(var c in cb)
            {
                if(c.SelectedIndex>-1)
                {
                    var Aid = long.Parse(c.SelectedValue.ToString());
                    list = list.Where(x => x.ItemAmenities.Select(y => y.AmenityID).Contains(Aid)).ToList();
                }
            }

            list=list.OrderBy(x => x.Title).ToList();
            dataGridView1.Rows.Clear();
            foreach (var i in list)
            {
                var Start = dateTimePicker1.Value;
                var End = (dateTimePicker2.Value == dateTimePicker2.MinDate) ? dateTimePicker2.MaxDate : dateTimePicker2.Value;
                var subList = i.ItemPrices.Where(y => y.Date >= start && y.Date <= end).ToList();

                foreach(var sub in subList)
                {
                    var row = new object[7];
                    row[0] = i.ID;
                    row[1] = i.Title;
                    row[2] = i.Area.Name;
                    row[3] = i.ItemScores.Select(x => x.Value).Any() ? decimal.Round((decimal)i.ItemScores.Select(x => x.Value).ToList().Average(), 2).ToString() : "-";
                    row[4] = i.ItemPrices.Where(x => x.Date <= DateTime.Now && !(x.BookingDetails.Select(y => y.isRefund).Contains(true))).ToList().Count;
                    var price = sub.Price;
                    var startprice = numericUpDown3.Value;
                    var endprice = (numericUpDown4.Value == numericUpDown4.Minimum) ? numericUpDown4.Maximum : numericUpDown4.Value;
                    if (price < startprice || price > endprice)
                        continue;
                    else
                        row[5] = price;
                    row[6] = sub.Date.ToShortDateString();
                    dataGridView1.Rows.Add(row);
                }
                

            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Index % 2 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            label15.Text = $"Displaying {dataGridView1.RowCount - 1} option";


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form = Application.OpenForms["Form1"];
            if (form != null)
            {
                form.Show();
            }
            else
            {
                new Form1().Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;
            var alist=ent.Attractions.Where(x => x.AreaID == (long)comboBox1.SelectedValue).ToList();
            comboBox2.Items.Clear();
            foreach(var a in alist)
            {
                comboBox2.Items.Add(a.Name);
            }

            var tlist = ent.Items.Where(x => x.AreaID == (long)comboBox1.SelectedValue);
            comboBox3.Items.Clear();
            foreach (var t in tlist)
            {
                comboBox3.Items.Add(t.Title);
            }
            textBox1.Visible=textBox2.Visible=false;
            comboBox2.Visible=comboBox3.Visible=true;
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(resetn>0)
            check();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resetn > 0)
                check();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resetn > 0) 
                check();
        }

        public void check()
        {
            ComboBox[] cb = new ComboBox[] { comboBox5, comboBox6, comboBox7 };
            List<object> validList = new List<object>();
            foreach (var a in cb)
            {
                if (a.SelectedIndex == -1)
                    continue;
                Console.WriteLine(a.SelectedValue.ToString());
                if (validList.Contains(a.SelectedValue))
                {
                    MessageBox.Show("This amenity has been chosen");
                    a.SelectedIndex = -1;
                    return;
                }
                validList.Add(a.SelectedValue);
            }
        }
    }
}
