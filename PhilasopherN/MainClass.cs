using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhilasopherN
{
    public delegate void starter();
    class MainClass
    {
        public event starter OnStarter;
        public event StatusChanged UIStatus;
        public Philosopher ph { get; set; }
        public TCPServer Forward { get; set; }
        public TCPClientt BackWard { get; set; }
        private PhilosopherStatus _forwardStatus;
        private PhilosopherStatus _backwardStatus;
        public event StatusChanged OnForwardChanged;
        public event StatusChanged OnBackwardChanged;
        public PhilosopherStatus ForwardStatus { get => _forwardStatus;
            set
            {
                _forwardStatus = value;
                OnForwardChanged?.Invoke(value);
            }
        }
        public PhilosopherStatus BackWardStatus { get => _backwardStatus;
            set
            {
                _backwardStatus = value;
                OnBackwardChanged?.Invoke(value);
            }
                }
        public int Count { get; set; }
        public MainClass(string ip = "127.0.0.1")
        {
            string portl = string.Empty, portr = string.Empty, name = string.Empty, count = string.Empty;
            ReadFile(out portl, out portr, out name, out count);
            Count = int.Parse(count);
            Forward = new TCPServer(int.Parse(portr), ip);
            Forward.OnMessageRecive += Forward_OnMessageRecive;
            Forward.Start();
            BackWard = new TCPClientt(int.Parse(portl), ip);
            BackWard.OnMessageRecive += BackWard_OnMessageRecive;
            if (!name.Equals("1")) BackWard.Connect();
            if (name.Equals(Count.ToString())) BackWard.Send("go"); 
            ph = new Philosopher(name);
            ph.OnStatusChanged += Ph_OnStatusChanged;
        }

        private void Ph_OnStatusChanged(PhilosopherStatus phs)
        {
            Forward.Send(phs.ToString());
            BackWard.Send(phs.ToString());
            UIStatus?.Invoke(phs);
        }

        private void BackWard_OnMessageRecive(string s)
        {   
            //////////////////////////////config region
            if(s.Equals("start"))
            {
                Start();
                if (!ph.Name.Equals(Count.ToString())) Forward.Send("start");
            }
            ////////////////////////////status region
            if (s.Equals(PhilosopherStatus.eating.ToString())) BackWardStatus = PhilosopherStatus.eating;
            if (s.Equals(PhilosopherStatus.waiting.ToString())) BackWardStatus = PhilosopherStatus.waiting;
            if (s.Equals(PhilosopherStatus.thinking.ToString())) BackWardStatus = PhilosopherStatus.thinking;
            ///////////////////////////////////report region
            if (s.Equals("Get")) BackWard.Send(ph.Reporter());
            if(s.Contains("Report"))
            {
                ReportBackward = new ReportModel(s);
                NotifierBackward = true;
            }
            /////////////////////////////force region
            if (s.Equals("Force")) ph.ForcePutDown();
            /////////////////////////////////change region
            if (s.Contains("Change"))
            {
                ChangeBackward = new ChangeModel(s);
                BackWardStatus = ChangeBackward.Status;
                Forward.Restart(ChangeBackward.Port);
            }
            ///
            if (s.Equals("Stop"))
            {
                try
                {
                    Forward.Send("Stop");
                }
                catch { }
                System.Environment.Exit(1);
            }
        }

        private void Forward_OnMessageRecive(string s)
        {
            ////////////////////////////////config region
            if (s.Equals("go"))
            {
                if (ph.Name.Equals("1"))
                {
                    BackWard.Connect();
                    Start();
                    Forward.Send("start");
                }
                else
                {
                    BackWard.Send("go");
                }
            }
            //////////////////////////status region
            if (s.Equals(PhilosopherStatus.eating.ToString())) ForwardStatus = PhilosopherStatus.eating;
            if (s.Equals(PhilosopherStatus.waiting.ToString())) ForwardStatus = PhilosopherStatus.waiting;
            if (s.Equals(PhilosopherStatus.thinking.ToString())) ForwardStatus = PhilosopherStatus.thinking;
            ////////////////////////////report region
            if (s.Equals("Get")) Forward.Send(ph.Reporter());
            if (s.Contains("Report"))
            {
                ReportForward = new ReportModel(s);
                NotifierForward = true;
            }
            /////////////////////////////force region
            if (s.Equals("Force")) ph.ForcePutDown();
            /////////////////////////////change region
            if (s.Contains("Change"))
            {
                ChangeForward = new ChangeModel(s);
                ForwardStatus = ChangeForward.Status;
                Thread.Sleep(100);
                BackWard.Reconnect(ChangeForward.Port);
            }
        }
        private void Start()
        {
            Task.Factory.StartNew(() =>
            {
                OnStarter?.Invoke();
                while (true)
                {
                    if (ph.PhilosopherStatus == PhilosopherStatus.eating)
                    {
                        ph.EatTime -= 100;
                        ph.AteTime += 100;
                        if (ph.EatTime <= 100)
                        {
                            //ph.ForkStatus = ForkStatus.none;
                            ph.PhilosopherStatus = PhilosopherStatus.thinking;
                            ph.SetEatTime();
                        }
                    }
                    else if (ph.PhilosopherStatus == PhilosopherStatus.thinking)
                    {
                        ph.ThinkTime -= 100;
                        if (ph.ThinkTime <= 100)
                        {
                            ph.SetThinkTime();
                            ph.PhilosopherStatus = PhilosopherStatus.waiting;
                        }
                    }
                    else
                    {
                        ph.WaitedTime += 100;
                        ph.TotallWaitedTime += 100;
                        if(ForwardStatus==PhilosopherStatus.thinking
                        && BackWardStatus == PhilosopherStatus.thinking)
                        {
                            ph.PhilosopherStatus = PhilosopherStatus.eating;
                            ph.WaitedTime = 0;
                            continue;
                        }
                        if(ForwardStatus==PhilosopherStatus.thinking
                        && BackWardStatus == PhilosopherStatus.waiting)
                        {
                            ph.PhilosopherStatus = PhilosopherStatus.eating;
                            ph.WaitedTime = 0;
                            continue;
                        }
                        if (ForwardStatus == PhilosopherStatus.waiting
                        && BackWardStatus == PhilosopherStatus.thinking)
                        {
                            ph.PhilosopherStatus = PhilosopherStatus.eating;
                            ph.WaitedTime = 0;
                            continue;
                        }
                        if (ForwardStatus == PhilosopherStatus.waiting
                        && BackWardStatus == PhilosopherStatus.waiting)
                        {
                            NotifierForward = false;
                            Forward.Send("Get");
                            while (!NotifierForward) { }
                            NotifierBackward = false;
                            BackWard.Send("Get");
                            while (!NotifierBackward) { }
                            if (ReportForward.WaiteTime < ph.WaitedTime && ReportBackward.WaiteTime < ph.WaitedTime)
                            {
                                ph.PhilosopherStatus = PhilosopherStatus.eating;
                                ph.WaitedTime = 0;
                                continue;
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
            });

        }
        private bool NotifierForward;
        private bool NotifierBackward;
        private ReportModel ReportForward;
        private ReportModel ReportBackward;
        private ChangeModel ChangeForward;
        private ChangeModel ChangeBackward;
        private void ReadFile(out string portl, out string portr, out string name, out string count)
        {
            count = "";
            portr = "";
            portl = "";
            name = "";
            string[] lines = File.ReadAllLines("config.txt");
            int index = 0;
            foreach (var item in lines)
            {
                if (!item.Contains("R"))
                {
                    string[] parts = item.Split(';');
                    count = parts[0];
                    name = parts[1];
                    portl = parts[2];
                    portr = parts[3];
                    break;
                }
                index++;
            }
            lines[index] += ";R";
            File.WriteAllLines("config.txt", lines);
        }

    }
    class ReportModel
    {
        public ForkStatus Fork { get; set; }
        public int WaiteTime { get; set; }
        public int AteTime { get; set; }
        public ReportModel(string s)
        {
            var temp = s.Split(';');
            AteTime = int.Parse(temp[3]);
            WaiteTime = int.Parse(temp[2]);
            if (temp[1].Equals(ForkStatus.none.ToString())) Fork = ForkStatus.none;
            else if (temp[1].Equals(ForkStatus.forward.ToString())) Fork = ForkStatus.forward;
            else if (temp[1].Equals(ForkStatus.backward.ToString())) Fork = ForkStatus.backward;
        }
    }
    class ChangeModel
    {
        public int Port { get; set; }
        public PhilosopherStatus Status { get; set; }
        public ChangeModel(string s)
        {
            var array = s.Split(';');
            Port = int.Parse(array[1]);
            if (array[2] == PhilosopherStatus.eating.ToString()) Status = PhilosopherStatus.eating;
            else if (array[2] == PhilosopherStatus.waiting.ToString()) Status = PhilosopherStatus.waiting;
            else if (array[2] == PhilosopherStatus.thinking.ToString()) Status = PhilosopherStatus.thinking;
        }
    }
}
