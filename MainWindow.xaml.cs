using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace uchebnayaPractica
{
    public partial class MainWindow : Window
    {
        private List<Event> _allEvents;

        public MainWindow()
        {
            InitializeComponent();
            LoadEvents();
            FillDirectionFilter();
            ApplyFilter();
        }

        private void LoadEvents()
        {
            _allEvents = new List<Event>
            {
                new Event { Logo="Images/logo1.png", Title="Хакатон 2025", Direction="Программирование", Date=new DateTime(2025,11,20)},
                new Event { Logo="Images/logo2.png", Title="Дизайн-марафон", Direction="Дизайн", Date=new DateTime(2025,12,01)},
                new Event { Logo="Images/logo3.png", Title="Блокчейн-день", Direction="Криптотехнологии", Date=new DateTime(2025,11,25)},
                new Event { Logo="Images/logo4.png", Title="Data Science Meetup", Direction="Аналитика", Date=new DateTime(2025,12,05)},
            };
        }

        private void FillDirectionFilter()
        {
            var directions = _allEvents.Select(e => e.Direction).Distinct().ToList();
            directions.Insert(0, "Все направления");
            DirectionFilter.ItemsSource = directions;
            DirectionFilter.SelectedIndex = 0;
        }

        private void ApplyFilter()
        {
            var filtered = _allEvents.AsEnumerable();

            // Фильтр по направлению
            if (DirectionFilter.SelectedItem != null && DirectionFilter.SelectedItem.ToString() != "Все направления")
            {
                string dir = DirectionFilter.SelectedItem.ToString();
                filtered = filtered.Where(e => e.Direction == dir);
            }

            // Фильтр по дате
            if (DateFilter.SelectedDate != null)
            {
                DateTime date = DateFilter.SelectedDate.Value;
                filtered = filtered.Where(e => e.Date.Date == date.Date);
            }

            EventList.ItemsSource = filtered.ToList();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }
    }
}

      /*  private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
           // var loginWindow = new LoginWindow();
            //loginWindow.Show();
            this.Close();
        }
    }
}
*/