using System;
using System.Linq;
using System.Windows;
using uchebnayaPractica.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace uchebnayaPractica
{
    public partial class MainWindow : Window
    {
        private readonly Praktika2Context _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new Praktika2Context();
            LoadDirections();
            LoadEvents();
        }

        private void LoadDirections()
        {
            var directions = _context.Direction
                .Select(d => d.DirectionName)
                .ToList();
            directions.Insert(0, "Все направления");
            DirectionFilter.ItemsSource = directions;
            DirectionFilter.SelectedIndex = 0;
        }

        private void LoadEvents()
        {
            var events = _context.Event
                .Include(e => e.EventTypeDictionary)
                    .ThenInclude(etd => etd.IddictionaryNavigation)
                .ToList()
                .Select(e => new
                {
                    Title = e.NameEvent,
                    Date = e.DateStart,
                    Days = e.Days,
                    Direction = string.Join(", ",
                        e.EventTypeDictionary
                            .Select(etd => etd.IddictionaryNavigation.DirectionName)
                            .Distinct()),
                    Logo = GetLogoPath(e.Id)
                })
                .ToList();
            EventList.ItemsSource = events;
        }

        private void ApplyFilter()
        {
            var filtered = _context.Event
                .Include(e => e.EventTypeDictionary)
                    .ThenInclude(etd => etd.IddictionaryNavigation)
                .AsQueryable();

            if (DirectionFilter.SelectedItem != null && DirectionFilter.SelectedItem.ToString() != "Все направления")
            {
                string selectedDirection = DirectionFilter.SelectedItem.ToString();
                filtered = filtered.Where(e => e.EventTypeDictionary
                    .Any(etd => etd.IddictionaryNavigation.DirectionName == selectedDirection));
            }

            if (DateFilter.SelectedDate.HasValue)
            {
                DateOnly selectedDate = DateOnly.FromDateTime(DateFilter.SelectedDate.Value);
                filtered = filtered.Where(e => e.DateStart == selectedDate);
            }

            var result = filtered
                .ToList()
                .Select(e => new
                {
                    Title = e.NameEvent,
                    Date = e.DateStart,
                    Days = e.Days,
                    Direction = string.Join(", ",
                        e.EventTypeDictionary
                            .Select(etd => etd.IddictionaryNavigation.DirectionName)
                            .Distinct()),
                    Logo = GetLogoPath(e.Id)
                })
                .ToList();
            EventList.ItemsSource = result;
        }

        private string GetLogoPath(int eventId)
        {
            int logoIndex = (eventId - 1) % 20 + 1;
            return $"pack://application:,,,/Resources/{logoIndex}.jpeg";
        }

        private void DirectionFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void DateFilter_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            var authWindow = new Auth();
            authWindow.Show();
            this.Close();
        }
    }
}