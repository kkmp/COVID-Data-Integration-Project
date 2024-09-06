using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using Nancy.Json;
using System.Xml.Serialization;
using Projekt.Data.Models;
using System.Windows;
using Microsoft.Win32;

namespace Client.Models
{
    public class DataViewModel : INotifyPropertyChanged
    {
        private SeriesCollection? seriesCollection;
        private string[]? labels;
        private Func<double, string>? yFormatter;
        private ObservableCollection<Country> countries;
        private List<DailyCase> data;

        public ObservableCollection<Country>? Countries
        {
            get => countries; 
            set
            {
                countries = value;
                NotifyPropertyChanged();
            }
        }
        public Country? SelectedCountry { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? StopDate { get; set; } = DateTime.Now;
        public bool ButtonEnabled { get; set; } = true;
        public bool ChartCreated { get; set; } = false;
        
        public ICommand CreateChart { get; set; }
        public ICommand LoadChart { get; set; }
        public ICommand SaveJson { get; set; }
        public ICommand SaveXML { get; set; }
        public SeriesCollection? SeriesCollection { get => seriesCollection; 
            set
            {
                seriesCollection = value;
                NotifyPropertyChanged();
            }
        }
        public string[]? Labels
        {
            get => labels; 
            set
            {
                labels = value;
                NotifyPropertyChanged();
            }
        }

        public Func<double, string>? YFormatter { get => yFormatter; 
            set 
            { 
                yFormatter = value;
                NotifyPropertyChanged();
            } 
        }

        public DataViewModel()
        {
            Task.Run(async () => Countries = new ObservableCollection<Country>(await ConnectionHandler.Instance.GetCountries()));
            CreateChart = new CommandHandler(CreateChartAsync, CanCreateChart);
            LoadChart = new CommandHandler(LoadChartData, () => true);
            SaveJson = new CommandHandler(SaveChartJson, CanSaveFile);
            SaveXML = new CommandHandler(SaveChartXML, CanSaveFile);
        }

        private async Task SaveChartJson()
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Json files (.json)|.json";
            sf.FilterIndex = 2;
            sf.RestoreDirectory = true;
            if (sf.ShowDialog() == true)
            {
                var path = sf.FileName.ToString();
                await using FileStream createStream = File.Create(path);
                await System.Text.Json.JsonSerializer.SerializeAsync(createStream, data);
                var json = new JavaScriptSerializer().Serialize(data);
                MessageBox.Show("Zapisano JSON do pliku " + path);
            }
        }
        private async Task SaveChartXML()
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "XML files (.xml)|.xml";
            sf.FilterIndex = 2;
            sf.RestoreDirectory = true;
            if (sf.ShowDialog() == true)
            {
                var path = sf.FileName.ToString();
                using (StreamWriter r = new StreamWriter(path))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<DailyCase>));
                    xml.Serialize(r, data);
                    MessageBox.Show("Zapisano XML do pliku " + path);
                }
            }
        }
        private async Task LoadChartData()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName.ToString();
                using (StreamReader r = new StreamReader(path))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<DailyCase>));
                    data = xml.Deserialize(r) as List<DailyCase>;
                    ButtonEnabled = false;

                    int lastConfirmed = 0;
                    int lastDeaths = 0;
                    data = data.OrderBy(x => x.Date).ToList();
                    data.ForEach(x =>
                    {
                        int tmp = x.NewConfirmed;
                        x.NewConfirmed = Math.Abs(x.NewConfirmed - lastConfirmed);
                        lastConfirmed = tmp;

                        tmp = x.NewDeaths;
                        x.NewDeaths = Math.Abs(x.NewDeaths - lastDeaths);
                        lastDeaths = tmp;
                    });
                    data = data.Skip(1).ToList();

                    SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "New confirmed",
                        Values = new ChartValues<int>(data.Select(x => x.NewConfirmed))
                    },
                    new LineSeries
                    {
                        Title = "New Deaths",
                        Values = new ChartValues<int>(data.Select(x => x.NewDeaths))
                    },
                };

                    Labels = data.Select(x => x.Date.Value.ToShortDateString()).ToArray();
                    YFormatter = value => value.ToString();
                    ButtonEnabled = true;
                    ChartCreated = true;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool CanCreateChart()
        {
            return SelectedCountry is Country && StartDate != null && StopDate != null && ButtonEnabled;
        }
        private bool CanSaveFile()
        {
            return ChartCreated && data != null;
        }
        private async Task CreateChartAsync()
        {
            ButtonEnabled = false;
            data = await ConnectionHandler.Instance.GetResults(SelectedCountry.Id, StartDate.Value.AddDays(-1), StopDate.Value);

            int lastConfirmed = 0;
            int lastDeaths = 0;
            data = data.OrderBy(x => x.Date).ToList();
            data.ForEach(x =>
            {
                int tmp = x.NewConfirmed;
                x.NewConfirmed = Math.Abs(x.NewConfirmed - lastConfirmed);
                lastConfirmed = tmp;

                tmp = x.NewDeaths;
                x.NewDeaths = Math.Abs(x.NewDeaths - lastDeaths);
                lastDeaths = tmp;
            });
            data = data.Skip(1).ToList();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "New confirmed",
                    Values = new ChartValues<int>(data.Select(x => x.NewConfirmed))
                },
                new LineSeries
                {
                    Title = "New Deaths",
                    Values = new ChartValues<int>(data.Select(x => x.NewDeaths))
                },
            };

            Labels = data.Select(x => x.Date.Value.ToShortDateString()).ToArray();
            YFormatter = value => value.ToString();
            ButtonEnabled = true;
            ChartCreated = true;
        }
        public XmlDocument JsonToXML(string json)
        {
            XmlDocument doc = new XmlDocument();

            using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
            {
                XElement xml = XElement.Load(reader);
                doc.LoadXml(xml.ToString());
            }

            return doc;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
