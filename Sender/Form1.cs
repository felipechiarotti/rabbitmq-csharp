using Rabbit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sender
{
    public partial class Form1 : Form
    {
        private IMensageriaService _service;
        Thread openConnectionThread;
        public Form1()
        {
            InitializeComponent();
            _service = new MensageriaService("localhost");
            openConnectionThread = new Thread(openRabbitConnection);
            openConnectionThread.IsBackground = true;
            openConnectionThread.Start();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            _service.deliverMessage(textBox1.Text, textBox2.Text, textBox3.Text);
        }

        private void openRabbitConnection()
        {
            _service.openConnection(formConnected);
            verifyConnection();
        }

        private void verifyConnection()
        {
            while (true)
            {
                formConnected(_service.isConnected());
                Thread.Sleep(5000);
            }
        }
        private void formConnected(bool success)
        {
            Invoke(new Action(() =>
            {
                textBox1.Enabled = success;
                textBox2.Enabled = success;
                button1.Enabled = success;
                textBox3.Enabled = success;
                panel1.BackColor = success ? Color.DarkGreen : Color.Firebrick;
                label4.Text = success ? "Conexão com o Rabbit Estabelecida" : "Falha ao conectar ao Rabbit... Tentando novamente em 5 segundos";
            }));
        }


    }
}
