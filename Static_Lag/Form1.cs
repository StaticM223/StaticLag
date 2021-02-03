using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Static_LagSwitch
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll")]
        private static extern bool GetAsyncKeyState(ushort vKey);

        private bool active = false;
        private string gamepath;
        private ushort hotkey;
        private int duration;


        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = Enum.GetValues(typeof(VirtualKeys));
        }


        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog selectExecutable = new OpenFileDialog())
            {
                if (selectExecutable.ShowDialog() == DialogResult.OK)
                {
                    gamepath = textBox1.Text = selectExecutable.FileName;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            hotkey = (ushort)Enum.Parse(typeof(VirtualKeys), comboBox1.SelectedItem.ToString());
            duration = Convert.ToInt32(comboBox2.SelectedItem);
            if (gamepath.Length == 0)
            {
                MessageBox.Show("No game EXE was set", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            active = !active;

            if (active)
            {
                button2.Text = "Stop";
                timer1.Start();
            }
            else
            {
                button2.Text = "Start";
                timer1.Stop();
            }
        }


        //Timer to check for hotkey press
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(hotkey))
            {
                ProcessStartInfo AddRuleIn = new ProcessStartInfo("cmd.exe", "/c netsh advfirewall firewall add rule name=\"StaticSwitch\" dir=in action=block program=\"" + gamepath + "\" enable=yes");
                ProcessStartInfo AddRuleOut = new ProcessStartInfo("cmd.exe", "/c netsh advfirewall firewall add rule name=\"StaticSwitch\" dir=out action=block program=\"" + gamepath + "\" enable=yes");
                ProcessStartInfo DeleteRule = new ProcessStartInfo("cmd.exe", "/c netsh advfirewall firewall delete rule name=\"StaticSwitch\" program=\"" + gamepath + "\"");
                AddRuleIn.WindowStyle = ProcessWindowStyle.Hidden;
                AddRuleOut.WindowStyle = ProcessWindowStyle.Hidden;
                DeleteRule.WindowStyle = ProcessWindowStyle.Hidden;


                //Add the firewall rules
                Process.Start(AddRuleIn);
                Process.Start(AddRuleOut);
                Console.Beep();
                //Sleep for the set duration
                Thread.Sleep(duration);
                //Delete the firewall rule
                Process.Start(DeleteRule);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
