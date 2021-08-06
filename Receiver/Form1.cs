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

namespace Receiver
{
    public partial class Form1 : Form
    {
        public IMensageriaService _service;
        public Form1()
        {
            InitializeComponent();
            _service = new MensageriaService("localhost");
            Thread openConnThread = new Thread(openRabbitConnection);
            openConnThread.IsBackground = true;
            openConnThread.Start();
  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            _service.receiveMessage(textBox1.Text,
                (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Invoke(new Action(() =>
                    {
                        richTextBox1.Text += "[" + DateTime.Now + "] " + message + "\n";
                    }));
                    
                });
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
                richTextBox1.Enabled = success;
                button1.Enabled = success;
                label3.Text = success ? "Conexão com o Rabbit Estabelecida" : "Falha ao conectar ao Rabbit... Tentando novamente em 5 segundos";
                panel1.BackColor = success ? Color.DarkGreen : Color.Firebrick;
            }));
        }
    }
}
