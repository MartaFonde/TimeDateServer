using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeDateClient
{
    public partial class Form1 : Form
    {
        const string IP_SERVER = "127.0.0.1";
        static int port = 31415;
        static IPAddress ip = IPAddress.Parse(IP_SERVER);
        static Form2 f2;
        bool ipCorrect = false;
        bool portCorrect = false;
        Socket server;

        public Form1()
        {
            InitializeComponent();
            port = 31415;
            btnTime.Tag = "TIME";
            btnDate.Tag = "DATE";
            btnAll.Tag = "DATETIME";
            btnClose.Tag = "CLOSE";
            lbl.Text = "";
        }

        private void connectionServer()
        {
            IPEndPoint ie = new IPEndPoint(ip, port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ie);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("No connection available", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
        }

        private void button_Click(object sender, EventArgs e)
        {            
            Button b = (Button)sender;
            connectionServer(); 
            try
            {
                using (NetworkStream ns = new NetworkStream(server))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    if (b.Tag != null)
                    {
                        sw.WriteLine(b.Tag);
                        sw.Flush();
                        lbl.Text = sr.ReadLine();
                        if (b.Tag.ToString() == "CLOSE")
                        {
                            server.Close();
                            this.Close();
                        }
                    }
                }
                server.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Connection fail: " + ex.Message);
            }                                    
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            f2 = new Form2();
            f2.txtIP.Text = IP_SERVER;
            f2.txtPort.Text = port.ToString();
            f2.lblIP.Text = "";
            f2.lblPort.Text = "";

            while (!ipCorrect || !portCorrect)
            {
                DialogResult res = f2.ShowDialog();
                if (res == DialogResult.OK)
                {
                    checkIP(f2.txtIP.Text);
                    checkPort(f2.txtPort.Text);                    
                }
                else
                {
                    this.Close();
                    break;
                }
            }
        }

        private bool checkIP(string txt)
        {
            f2.lblIP.Text = "";
            ipCorrect = true;

            if (txt != IP_SERVER)
            {
                if (txt.Length > 0)
                {
                    try
                    {
                        ip = IPAddress.Parse(txt);                        
                    }
                    catch (FormatException)
                    {
                        f2.lblIP.Text = "Error. Format IP invalid";
                        ipCorrect = false;
                    }
                }
                else
                {
                    f2.lblIP.Text = "Introduce IP";
                    ipCorrect = false;
                }
            }            
            return ipCorrect;
        }

        private bool checkPort(string txt)
        {
            f2.lblPort.Text = "";
            portCorrect = true;

            if (txt != port.ToString())
            {
                if (txt.Length > 0)
                {
                    if (!int.TryParse(txt, out port) || port < 1024 || port > 65535)
                    {
                        f2.lblPort.Text = "Error. Port invalid. Introduce a number between 1024-65535";
                        portCorrect = false;
                    }                    
                }
                else
                {
                    f2.lblPort.Text = "Introduce port";
                    portCorrect = false;
                }
            }            
            return portCorrect;
        }
    }
}