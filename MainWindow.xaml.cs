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

        // Загрузка направлений
        private void LoadDirections()
        {
            var directions = _context.Direction
                .Select(d => d.DirectionName)
                .ToList();
            directions.Insert(0, "Все направления");
            DirectionFilter.ItemsSource = directions;
            DirectionFilter.SelectedIndex = 0;
        }

        // Загрузка всех мероприятий
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
                    // Сопоставляем логотип по ID события
                    Logo = GetLogoPath(e.Id)
                })
                .ToList();
            EventList.ItemsSource = events;
        }

        // Применение фильтров
        private void ApplyFilter()
        {
            var filtered = _context.Event
                .Include(e => e.EventTypeDictionary)
                    .ThenInclude(etd => etd.IddictionaryNavigation)
                .AsQueryable();

            // Фильтр по направлению
            if (DirectionFilter.SelectedItem != null && DirectionFilter.SelectedItem.ToString() != "Все направления")
            {
                string selectedDirection = DirectionFilter.SelectedItem.ToString();
                filtered = filtered.Where(e => e.EventTypeDictionary
                    .Any(etd => etd.IddictionaryNavigation.DirectionName == selectedDirection));
            }

            // Фильтр по дате
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
                    // Сопоставляем логотип по ID события
                    Logo = GetLogoPath(e.Id)
                })
                .ToList();
            EventList.ItemsSource = result;
        }

        // Универсальный метод для получения пути к логотипу (сопоставление по ID события, цикл 1-20)
        private string GetLogoPath(int eventId)
        {
            int logoIndex = (eventId - 1) % 20 + 1; // Получаем 1..20 на основе eventId
            return $"pack://application:,,,/Resources/{logoIndex}.jpeg";
        }

        // Обработчики фильтров
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