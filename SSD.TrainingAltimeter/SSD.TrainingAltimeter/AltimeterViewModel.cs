using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace SSD.TrainingAltimeter
{
    class AltimeterViewModel : INotifyPropertyChanged
    {
        private bool _isStarted = false;
        private DispatcherTimer _timer = null;
        private DateTime _lastUpdateTime;

        private int _velocity;
        private double _altitude;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand StartCommand { get; set; }

        public AltimeterViewModel()
        {
            // TODO HARDCODE!!!
            _velocity = 50;
            _altitude = 3000;

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            StartCommand = new CommandStart(
                p => Start(),
                p => CanStart());
        }

        public int Velocity
        {
            get { return _velocity; }
            set
            {
                _velocity = value;
                OnPropertyChanged(nameof(Velocity));
            }
        }

        public double Altitude
        {
            get { return _altitude; }
            set
            {
                _altitude = value;
                OnPropertyChanged(nameof(Altitude));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanStart() => !_isStarted;

        private bool CanStop() => _isStarted;

        private void Stop()
        {
            _timer.Stop();
            _isStarted = false;

            Debug.WriteLine("Stopped!");
        }

        private void Start()
        {
            Stop();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Start();
            _isStarted = true;

            _lastUpdateTime = DateTime.Now;

            Debug.WriteLine("Started!");
        }

        private void OnTimerTick(object sender, object e)
        {
            TimeSpan timeDiff = DateTime.Now - _lastUpdateTime;

            Debug.WriteLine(timeDiff.TotalSeconds.ToString());

            UpdateAltitude(Altitude - Velocity * timeDiff.TotalSeconds);

            _lastUpdateTime = DateTime.Now;
        }

        private void UpdateAltitude(double altitude)
        {
            if (altitude < 0)
            {
                altitude = 0;
                Stop();
            }
            Altitude = altitude;
        }
    }
}
