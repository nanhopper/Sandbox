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

namespace GUI
{
    public partial class Form1 : Form
    {
        private Presenter Presenter = new Presenter();

        public Form1()
        {
            InitializeComponent();
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.DataBindings.Clear();
            Presenter.RetrieveAndFillGrid();
        }

        public void SetDataSourceOnList(BindingList<int> source)
        {
            listBox1.DataSource = source;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Presenter.SendMessage += Presenter_SendMessage;
        }

        private void Presenter_SendMessage(object sender, SendMessageArgs args)
        {
            Invoke(new Action(()=> label1.Text = args.Message));
        }
    }

    public class Presenter
    {
        public delegate void SendMessageHandler(object sender, SendMessageArgs args);

        public event SendMessageHandler SendMessage;

        private void OnMessageSend(SendMessageArgs args)
        {
            if(SendMessage != null)
            {
                SendMessage(this, args);
            }
        }

        internal void RetrieveAndFillGrid()
        {
            DoLongTask1();
        }

        private void DoLongTask0()
        {
            var results = DoWork();
            WriteResult(results);
        }

        private async Task DoLongTask1()
        {
            var results = await Task.Run(() => DoWork());
            WriteResult(results);
        }

        private async Task DoLongTask2()
        {
            var task = new Task<IEnumerable<int>>(() => DoWork());
            task.Start();
            var result = await task;
            WriteResult(result);
        }

        private async Task DoLongTask3()
        {
            var results = await Task.Factory.StartNew(() => DoWork());
            WriteResult(results);
        }

        private IEnumerable<int> DoWork()
        {
            WriteLine("Starting long task...");
            foreach (var result in GetResults())
            {
                yield return result;
            }
            WriteLine("Task done.");
        }

        private void WriteResult(int result)
        {
            WriteLine("Long task done. Result is: " + result);
        }

        private void WriteResult(IEnumerable<int> results)
        {
            var i = 1;
            foreach (var result in results)
            {
                WriteLine(string.Format("Intermediary result {0} is: " + result, i));
                i++;
            }
        }

        private void WriteLine(string message)
        {
            OnMessageSend(new SendMessageArgs { Message = string.Format("{0} [{1}]", message, Thread.CurrentThread.ManagedThreadId) });
        }

        private IEnumerable<int> GetResults()
        {
            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                yield return random.Next();
            }
        }

        private int GetResult()
        {
            return random.Next();
        }

        private Random random = new Random(DateTime.Now.Millisecond);
    }

    public class SendMessageArgs
    {
        public string Message { get; set; }
    }
}
