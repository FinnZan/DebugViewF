using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    internal class MainWindowViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IEventSource _source;
        private bool _filterEnabled = true;
        private object _eventLock = new object();

        #region Properties ==========================

        public bool FilterEnabled
        {
            get
            {
                return _filterEnabled;
            }

            set
            {
                _filterEnabled = value;
                UpdateEventList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilterEnabled"));
            }
        }

        /// <summary>
        /// List with Filter on, the actual list is stroed in event source 
        /// </summary>        
        public ObservableCollection<LogEvent> DisplayEvents
        {
            get; private set;
        }

        public List<Filter> Filters
        {
            get; private set;
        } = new List<Filter>();

        public List<Filter> FiltersEx
        {
            get; private set;
        } = new List<Filter>();

        #endregion
        
        public MainWindowViewModel(IEventSource source)
        {
            _source = source;
            _source.Updated += new SourceEventHandler(UpdateEventList);
        }
     
        public bool AddFilter(FilterType type, string key, bool include)
        {
            try
            {
                Filter f = null;

                if (type == FilterType.ThreadID)
                {
                    int id;
                    if (int.TryParse(key, out id))
                    {
                        f = new Filter(type, key);
                    }
                }
                else
                {
                    f = new Filter(type, key);
                }

                if (f != null)
                {
                    if (include)
                    {
                        Filters.Add(f);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filters"));   
                    }
                    else
                    {
                        FiltersEx.Add(f);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FiltersEx"));
                    }
                }

                UpdateEventList();
                return true;
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
                return false;
            }
        }

        public bool DeleteFilter(Filter filter, bool include)
        {
            try
            {
                if (include)
                {
                    Filters.Remove(filter);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filters"));
                }
                else
                {
                    FiltersEx.Remove(filter);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FiltersEx"));
                }

                UpdateEventList();
                return true;
            }
            catch (Exception ex)
            {
                return false;                
            }
        }

        public void Clear()
        {
            _source.Events.Clear();
            UpdateEventList();
        }

        private void UpdateEventList()
        {
            try
            {
                if (FilterEnabled)
                {
                    DisplayEvents = new ObservableCollection<LogEvent>(_source.Events.Where(o => TestFilterEvent(o)).ToList<LogEvent>());
                }
                else
                {
                    DisplayEvents = new ObservableCollection<LogEvent>(_source.Events);
                }
                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisplayEvents"));
            }
            catch (Exception ex)
            {

            }
        }
               
        private bool TestFilterEvent(LogEvent e)
        {
            bool b = true;

            try
            {
                if (Filters.Count > 0)
                {
                    b = false;
                }

                foreach (var f in Filters)
                {
                    if (f.Type == FilterType.AppDomain)
                    {
                        b |= e.AppDomain.ToLower().Contains(f.Key.ToLower());
                    }

                    if (f.Type == FilterType.ThreadID)
                    {
                        b |= e.ThreadID == int.Parse(f.Key);
                    }

                    if (f.Type == FilterType.Source)
                    {
                        b |= e.Source.ToLower().Contains(f.Key.ToLower());
                    }

                    if (f.Type == FilterType.Event)
                    {
                        b |= e.Event.ToLower().Contains(f.Key.ToLower());
                    }
                }

                foreach (var f in FiltersEx)
                {
                    if (f.Type == FilterType.AppDomain && e.AppDomain.ToLower().Contains(f.Key.ToLower()))
                    {
                        b = false;
                    }

                    if (f.Type == FilterType.ThreadID && e.ThreadID == int.Parse(f.Key))
                    {
                        b = false;
                    }

                    if (f.Type == FilterType.Source && e.Source.ToLower().Contains(f.Key.ToLower()))
                    {
                        b = false;
                    }

                    if (f.Type == FilterType.Event && e.Event.ToLower().Contains(f.Key.ToLower()))
                    {
                        b = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return b;
        }                
    }
}
