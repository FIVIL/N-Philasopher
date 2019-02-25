using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace PhilasopherN
{
    public enum PhilosopherStatus
    {
        thinking,
        eating,
        waiting
    }
    public enum ForkStatus
    {
        forward,
        backward,
        both,
        none
    }
    public delegate void StatusChanged(PhilosopherStatus phs);
    public delegate void ForkChanged(ForkStatus fs);
    public class Philosopher
    {
        
        private PhilosopherStatus _philosopherStatus;
        public PhilosopherStatus PhilosopherStatus {
            get => _philosopherStatus;
            set
            {
                _philosopherStatus = value;
                OnStatusChanged?.Invoke(value);
            }
        }
        public event ForkChanged OnForkChanged;
        public int TotallWaitedTime { get; set; }
        private ForkStatus _forkStatus;
        public ForkStatus ForkStatus { get => _forkStatus;
            set
            {
                _forkStatus = value;
                OnForkChanged?.Invoke(value);
            }
        }
        public int ThinkTime { get; set; }
        public int WaitedTime { get; set; }
        public int EatTime { get; set; }
        public int AteTime { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        private Random rnd;
        public event StatusChanged OnStatusChanged;
        public Philosopher(string name)
        {
            rnd = new Random();
            ThinkTime = rnd.Next(1000, 5000);
            EatTime = rnd.Next(1500, 3500);
            PhilosopherStatus = PhilosopherStatus.thinking;
            Name = name;
            //ForkStatus = ForkStatus.forward;
            //if (name == "1") ForkStatus = ForkStatus.none;
            //if (name == "2") ForkStatus = ForkStatus.both;
            WaitedTime = 0;
            AteTime = 0;
            TotallWaitedTime = 0;
        }
        public Philosopher(string name,string longname):this(name)
        {
            LongName = longname;
        }
        public void ForcePutDown()
        {
            if (ForkStatus != ForkStatus.both) ForkStatus = ForkStatus.none;
        }
        public string Reporter ()
        {
            string s = string.Empty;
            s += "Report;";
            s += ForkStatus.ToString();
            s += ";";
            s += WaitedTime.ToString();
            s += ";";
            s += AteTime.ToString();
            return s;
        }
        public void SetEatTime()
        {
            EatTime = rnd.Next(1000,4000);
        }

        public void SetThinkTime()
        {
            ThinkTime = rnd.Next(2000, 6000);
        }
    }
}
