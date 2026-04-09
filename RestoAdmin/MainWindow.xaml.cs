using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using RestoAdmin.Common;
using RestoAdmin.Database;
using RestoAdmin.Models;
using RestoAdmin.Services;

namespace RestoAdmin
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly TableService _tableService;
        private readonly CustomerService _customerService;
        private readonly BookingService _bookingService;
        private Table _selectedTable;
        private List<Table> _allTables;
        private System.Windows.Threading.DispatcherTimer _timer;
        private Booking _editingBooking;

        public MainWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _tableService = new TableService(_context);
            _customerService = new CustomerService(_context);
            _bookingService = new BookingService(_context);

            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => UpdateCurrentDateTime();
            _timer.Start();
            UpdateCurrentDateTime();

            DatePickerBooking.SelectedDate = DateTime.Today;
            DatePickerBooking.DisplayDateStart = DateTime.Today;

            LoadTables();
        }

        private void UpdateCurrentDateTime()
        {
            TxtCurrentTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private void LoadTables()
        {
            _allTables = _tableService.GetAllTables();
            DrawHallScheme();
        }

        private void DrawHallScheme()
        {
            HallCanvas.Children.Clear();

            foreach (var table in _allTables)
            {
                LinearGradientBrush backgroundBrush;
                switch (table.Status)
                {
                    case TableStatus.Free:
                        backgroundBrush = new LinearGradientBrush
                        {
                            StartPoint = new Point(0, 0),
                            EndPoint = new Point(1, 1),
                            GradientStops = new GradientStopCollection
                            {
                                new GradientStop((Color)ColorConverter.ConvertFromString("#00B4DB"), 0),
                                new GradientStop((Color)ColorConverter.ConvertFromString("#0083B0"), 1)
                            }
                        };
                        break;
                    case TableStatus.Booked:
                        backgroundBrush = new LinearGradientBrush
                        {
                            StartPoint = new Point(0, 0),
                            EndPoint = new Point(1, 1),
                            GradientStops = new GradientStopCollection
                            {
                                new GradientStop((Color)ColorConverter.ConvertFromString("#F2994A"), 0),
                                new GradientStop((Color)ColorConverter.ConvertFromString("#F2C94C"), 1)
                            }
                        };
                        break;
                    default:
                        backgroundBrush = new LinearGradientBrush
                        {
                            StartPoint = new Point(0, 0),
                            EndPoint = new Point(1, 1),
                            GradientStops = new GradientStopCollection
                            {
                                new GradientStop((Color)ColorConverter.ConvertFromString("#EB3349"), 0),
                                new GradientStop((Color)ColorConverter.ConvertFromString("#F45C43"), 1)
                            }
                        };
                        break;
                }

                Border border = new Border
                {
                    Width = 70,
                    Height = 70,
                    Background = backgroundBrush,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(10),
                    Tag = table,
                    Cursor = Cursors.Hand,
                    Effect = new DropShadowEffect { BlurRadius = 5, ShadowDepth = 2, Opacity = 0.3 }
                };

                border.MouseLeftButtonDown += Table_Click;

                string zoneIcon = table.IsVipZone ? "VIP " : "";
                TextBlock text = new TextBlock
                {
                    Text = $"{zoneIcon}{table.TableNumber}\n{table.Capacity} мест",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Grid grid = new Grid();
                grid.Children.Add(border);
                grid.Children.Add(text);

                Canvas.SetLeft(grid, table.XPosition);
                Canvas.SetTop(grid, table.YPosition);
                HallCanvas.Children.Add(grid);
            }
        }

        private void Table_Click(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            Table table = border.Tag as Table;

            DateTime selectedDate = DatePickerBooking.SelectedDate ?? DateTime.Today;
            string selectedTimeStr = ((ComboBoxItem)ComboTime.SelectedItem)?.Content.ToString() ?? "12:00";
            TimeSpan selectedTime = TimeSpan.Parse(selectedTimeStr);
            int duration = int.Parse(((ComboBoxItem)ComboDuration.SelectedItem)?.Content.ToString()?.Split(' ')[0] ?? "2");

            if (_selectedTable != null && _selectedTable.Id == table.Id)
            {
                if (_editingBooking != null)
                {
                    CancelEditing();
                }
                else
                {
                    HighlightTable(_selectedTable, false);
                    _selectedTable = null;
                    TxtSelectedTable.Text = "Не выбран";
                    TxtStatus.Text = "Выбор столика отменен. Выберите другой столик.";
                    BtnConfirmBooking.IsEnabled = false;
                }
                return;
            }

            bool isAvailable = _tableService.IsTableAvailable(table.Id, selectedDate, selectedTime, duration);

            if (table.Status != TableStatus.Free || !isAvailable)
            {
                TxtStatus.Text = $"Столик {table.TableNumber} уже забронирован на {selectedDate:dd.MM.yyyy} в {selectedTimeStr}";
                return;
            }

            if (_selectedTable != null)
            {
                HighlightTable(_selectedTable, false);
            }

            _selectedTable = table;
            string zoneName = _tableService.GetZoneName(table.ZoneId);
            TxtSelectedTable.Text = $"Стол {table.TableNumber} | {table.Capacity} персоны | {zoneName}";
            TxtStatus.Text = $"Выбран столик {table.TableNumber} на {selectedDate:dd.MM.yyyy} в {selectedTimeStr}. Заполните данные клиента и подтвердите бронирование.";
            BtnConfirmBooking.IsEnabled = true;

            HighlightTable(table, true);
        }

        private void HighlightTable(Table table, bool highlight)
        {
            foreach (var child in HallCanvas.Children)
            {
                Grid grid = child as Grid;
                if (grid != null)
                {
                    Border border = grid.Children[0] as Border;
                    Table t = border.Tag as Table;
                    if (t.Id == table.Id)
                    {
                        if (highlight)
                        {
                            border.BorderThickness = new Thickness(4);
                            border.BorderBrush = Brushes.Orange;
                            border.Effect = new DropShadowEffect { BlurRadius = 12, ShadowDepth = 4, Opacity = 0.5, Color = Colors.Orange };
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(2);
                            border.BorderBrush = Brushes.White;
                            border.Effect = new DropShadowEffect { BlurRadius = 5, ShadowDepth = 2, Opacity = 0.3 };
                        }
                        break;
                    }
                }
            }
        }

        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            var filter = new TableFilter
            {
                HasWindowView = ChkWindowView.IsChecked ?? false,
                HasStageView = ChkStageView.IsChecked ?? false,
                IsVipZone = ChkVipZone.IsChecked ?? false,
                IsQuietZone = ChkQuietZone.IsChecked ?? false,
             
            };

            var filteredTables = _tableService.GetFilteredTables(filter);

            HallCanvas.Children.Clear();

            foreach (var table in _allTables)
            {
                LinearGradientBrush backgroundBrush;
                bool isMatch = filteredTables.Contains(table);

                if (table.Status == TableStatus.Free && isMatch)
                {
                    backgroundBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop((Color)ColorConverter.ConvertFromString("#11998E"), 0),
                            new GradientStop((Color)ColorConverter.ConvertFromString("#38EF7D"), 1)
                        }
                    };
                }
                else if (table.Status == TableStatus.Free && !isMatch)
                {
                    backgroundBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop((Color)ColorConverter.ConvertFromString("#FF6B6B"), 0),
                            new GradientStop((Color)ColorConverter.ConvertFromString("#FF8E53"), 1)
                        }
                    };
                }
                else if (table.Status == TableStatus.Booked)
                {
                    backgroundBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop((Color)ColorConverter.ConvertFromString("#F2994A"), 0),
                            new GradientStop((Color)ColorConverter.ConvertFromString("#F2C94C"), 1)
                        }
                    };
                }
                else
                {
                    backgroundBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop((Color)ColorConverter.ConvertFromString("#EB3349"), 0),
                            new GradientStop((Color)ColorConverter.ConvertFromString("#F45C43"), 1)
                        }
                    };
                }

                Border border = new Border
                {
                    Width = 70,
                    Height = 70,
                    Background = backgroundBrush,
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(10),
                    Tag = table,
                    Cursor = Cursors.Hand,
                    Effect = new DropShadowEffect { BlurRadius = 5, ShadowDepth = 2, Opacity = 0.3 }
                };

                border.MouseLeftButtonDown += Table_Click;

                string zoneIcon = table.IsVipZone ? "VIP " : "";
                TextBlock text = new TextBlock
                {
                    Text = $"{zoneIcon}{table.TableNumber}\n{table.Capacity} мест",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Grid grid = new Grid();
                grid.Children.Add(border);
                grid.Children.Add(text);

                Canvas.SetLeft(grid, table.XPosition);
                Canvas.SetTop(grid, table.YPosition);
                HallCanvas.Children.Add(grid);
            }

            int freeMatchingCount = filteredTables.Count(t => t.Status == TableStatus.Free);
            TxtStatus.Text = $"Найдено {freeMatchingCount} столиков, подходящих под пожелания клиента.";
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            ChkWindowView.IsChecked = false;
            ChkStageView.IsChecked = false;
            ChkVipZone.IsChecked = false;
            ChkQuietZone.IsChecked = false;
       
            DrawHallScheme();
            TxtStatus.Text = "Фильтр сброшен. Выберите столик на схеме зала.";
        }

        private void BtnConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTable == null)
            {
                MessageBox.Show("Выберите столик на схеме зала.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtCustomerName.Text))
            {
                MessageBox.Show("Введите имя клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtCustomerPhone.Text))
            {
                MessageBox.Show("Введите телефон клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtPersonsCount.Text, out int personsCount) || personsCount <= 0)
            {
                MessageBox.Show("Введите корректное количество персон.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (personsCount > _selectedTable.Capacity)
            {
                MessageBox.Show($"Столик {_selectedTable.TableNumber} рассчитан на {_selectedTable.Capacity} персон.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime selectedDate = DatePickerBooking.SelectedDate ?? DateTime.Today;
            string selectedTimeStr = ((ComboBoxItem)ComboTime.SelectedItem)?.Content.ToString() ?? "12:00";
            TimeSpan selectedTime = TimeSpan.Parse(selectedTimeStr);
            int duration = int.Parse(((ComboBoxItem)ComboDuration.SelectedItem)?.Content.ToString()?.Split(' ')[0] ?? "2");

            bool isAvailable = _tableService.IsTableAvailable(_selectedTable.Id, selectedDate, selectedTime, duration);
            if (!isAvailable && _editingBooking == null)
            {
                MessageBox.Show($"Столик {_selectedTable.TableNumber} уже забронирован на {selectedDate:dd.MM.yyyy} в {selectedTimeStr}.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_editingBooking != null)
                {
                    var existingBooking = _context.Bookings.FirstOrDefault(b => b.Id == _editingBooking.Id);
                    if (existingBooking != null)
                    {
                        var oldTable = _context.Tables.FirstOrDefault(t => t.Id == existingBooking.TableId);
                        if (oldTable != null)
                        {
                            oldTable.Status = TableStatus.Free;
                        }

                        existingBooking.BookingDate = selectedDate;
                        existingBooking.BookingTime = selectedTime;
                        existingBooking.DurationHours = duration;
                        existingBooking.PersonsCount = personsCount;
                        existingBooking.SpecialRequests = TxtSpecialRequests.Text;
                        existingBooking.NeedTableDecoration = ChkTableDecoration.IsChecked ?? false;
                        existingBooking.NeedFlowers = ChkFlowers.IsChecked ?? false;
                        existingBooking.NeedCandles = ChkCandles.IsChecked ?? false;
                        existingBooking.NeedBalloons = ChkBalloons.IsChecked ?? false;
                        existingBooking.NeedLiveMusic = ChkLiveMusic.IsChecked ?? false;
                        existingBooking.IsFamilyCelebration = RbtnFamily.IsChecked ?? false;
                        existingBooking.IsRomanticDinner = RbtnRomantic.IsChecked ?? false;
                        existingBooking.IsCorporateEvent = RbtnCorporate.IsChecked ?? false;
                        existingBooking.IsBanquet = RbtnBanquet.IsChecked ?? false;

                        var customer = _customerService.GetOrCreateCustomer(TxtCustomerName.Text, TxtCustomerPhone.Text, TxtCustomerEmail.Text);
                        existingBooking.CustomerId = customer.Id;
                        existingBooking.TableId = _selectedTable.Id;

                        _selectedTable.Status = TableStatus.Booked;
                        _context.Entry(_selectedTable).State = EntityState.Modified;
                        _context.SaveChanges();

                        MessageBox.Show($"Бронирование обновлено!\n\nНомер: {existingBooking.BookingNumber}\nКлиент: {customer.Name}\nСтолик: {_selectedTable.TableNumber}\nПерсон: {personsCount}\nДата: {selectedDate:dd.MM.yyyy}\nВремя: {selectedTimeStr}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                        CancelEditing();
                    }
                }
                else
                {
                    var bookingData = new BookingData
                    {
                        CustomerName = TxtCustomerName.Text,
                        CustomerPhone = TxtCustomerPhone.Text,
                        CustomerEmail = TxtCustomerEmail.Text,
                        PersonsCount = personsCount,
                        SpecialRequests = TxtSpecialRequests.Text,
                        
                        NeedTableDecoration = ChkTableDecoration.IsChecked ?? false,
                        NeedFlowers = ChkFlowers.IsChecked ?? false,
                        NeedCandles = ChkCandles.IsChecked ?? false,
                        NeedBalloons = ChkBalloons.IsChecked ?? false,
                        NeedLiveMusic = ChkLiveMusic.IsChecked ?? false,
                        IsFamilyCelebration = RbtnFamily.IsChecked ?? false,
                        IsRomanticDinner = RbtnRomantic.IsChecked ?? false,
                        IsCorporateEvent = RbtnCorporate.IsChecked ?? false,
                        IsBanquet = RbtnBanquet.IsChecked ?? false,
                        BookingDate = selectedDate,
                        BookingTime = selectedTime,
                        DurationHours = duration
                    };

                    var customer = _customerService.GetOrCreateCustomer(bookingData.CustomerName, bookingData.CustomerPhone, bookingData.CustomerEmail);
                    bookingData.CustomerId = customer.Id;
                    bookingData.TableId = _selectedTable.Id;

                    var booking = _bookingService.CreateBooking(bookingData);
                    _tableService.BookTable(_selectedTable, booking);

                    MessageBox.Show($"Бронирование успешно создано!\n\nНомер брони: {booking.BookingNumber}\nКлиент: {customer.Name}\nСтолик: {_selectedTable.TableNumber}\nПерсон: {personsCount}\nДата: {selectedDate:dd.MM.yyyy}\nВремя: {selectedTimeStr}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ResetForm();
                LoadTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditBooking_Click(object sender, RoutedEventArgs e)
        {
            var bookings = _context.Bookings.Include(b => b.Table).Include(b => b.Customer).ToList();

            if (bookings.Count == 0)
            {
                MessageBox.Show("Нет бронирований для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectDialog = new Window
            {
                Title = "Выберите бронь для редактирования",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var listBox = new ListBox();
            listBox.DisplayMemberPath = "DisplayText";
            listBox.ItemsSource = bookings.Select(b => new { Id = b.Id, DisplayText = $"{b.BookingNumber} - Стол {b.Table?.TableNumber} - {b.Customer?.Name} - {b.BookingDate:dd.MM.yyyy} {b.BookingTime}" }).ToList();

            listBox.SelectionChanged += (s, args) =>
            {
                var selected = listBox.SelectedItem;
                var idProperty = selected.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    int bookingId = (int)idProperty.GetValue(selected);
                    LoadBookingForEdit(bookingId);
                    selectDialog.Close();
                }
            };

            selectDialog.Content = listBox;
            selectDialog.ShowDialog();
        }

        private void LoadBookingForEdit(int bookingId)
        {
            var booking = _context.Bookings.Include(b => b.Table).Include(b => b.Customer).FirstOrDefault(b => b.Id == bookingId);
            if (booking == null) return;

            _editingBooking = booking;
            _selectedTable = booking.Table;

            DatePickerBooking.SelectedDate = booking.BookingDate;

            string timeStr = booking.BookingTime.ToString(@"hh\:mm");
            foreach (ComboBoxItem item in ComboTime.Items)
            {
                if (item.Content.ToString() == timeStr)
                {
                    ComboTime.SelectedItem = item;
                    break;
                }
            }

            string durationStr = booking.DurationHours.ToString();
            foreach (ComboBoxItem item in ComboDuration.Items)
            {
                if (item.Content.ToString().StartsWith(durationStr))
                {
                    ComboDuration.SelectedItem = item;
                    break;
                }
            }

            TxtCustomerName.Text = booking.Customer?.Name;
            TxtCustomerPhone.Text = booking.Customer?.Phone;
            TxtCustomerEmail.Text = booking.Customer?.Email;
            TxtPersonsCount.Text = booking.PersonsCount.ToString();
            TxtSpecialRequests.Text = booking.SpecialRequests;

           
            ChkTableDecoration.IsChecked = booking.NeedTableDecoration;
            ChkFlowers.IsChecked = booking.NeedFlowers;
            ChkCandles.IsChecked = booking.NeedCandles;
            ChkBalloons.IsChecked = booking.NeedBalloons;
            ChkLiveMusic.IsChecked = booking.NeedLiveMusic;

            RbtnFamily.IsChecked = booking.IsFamilyCelebration;
            RbtnRomantic.IsChecked = booking.IsRomanticDinner;
            RbtnCorporate.IsChecked = booking.IsCorporateEvent;
            RbtnBanquet.IsChecked = booking.IsBanquet;

            TxtSelectedTable.Text = $"Стол {booking.Table?.TableNumber} | {booking.Table?.Capacity} персоны";
            BtnConfirmBooking.IsEnabled = true;
            BtnConfirmBooking.Content = "Обновить бронь";
            TxtStatus.Text = $"Редактирование брони {booking.BookingNumber}. Внесите изменения и нажмите Обновить бронь.";

            if (booking.Table != null)
            {
                HighlightTable(booking.Table, true);
            }
        }

        private void BtnDeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            var bookings = _context.Bookings.Include(b => b.Table).Include(b => b.Customer).ToList();

            if (bookings.Count == 0)
            {
                MessageBox.Show("Нет бронирований для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectDialog = new Window
            {
                Title = "Выберите бронь для удаления",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var listBox = new ListBox();
            listBox.DisplayMemberPath = "DisplayText";
            listBox.ItemsSource = bookings.Select(b => new { Id = b.Id, DisplayText = $"{b.BookingNumber} - Стол {b.Table?.TableNumber} - {b.Customer?.Name} - {b.BookingDate:dd.MM.yyyy} {b.BookingTime}" }).ToList();

            listBox.SelectionChanged += (s, args) =>
            {
                var selected = listBox.SelectedItem;
                var idProperty = selected.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    int bookingId = (int)idProperty.GetValue(selected);

                    var result = MessageBox.Show("Вы уверены, что хотите удалить эту бронь?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
                        if (booking != null)
                        {
                            var table = _context.Tables.FirstOrDefault(t => t.Id == booking.TableId);
                            if (table != null)
                            {
                                table.Status = TableStatus.Free;
                            }
                            _context.Bookings.Remove(booking);
                            _context.SaveChanges();

                            MessageBox.Show("Бронирование успешно удалено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                            if (_editingBooking != null && _editingBooking.Id == bookingId)
                            {
                                CancelEditing();
                            }

                            LoadTables();
                        }
                    }
                    selectDialog.Close();
                }
            };

            selectDialog.Content = listBox;
            selectDialog.ShowDialog();
        }

        private void CancelEditing()
        {
            _editingBooking = null;
            BtnConfirmBooking.Content = "Подтвердить бронирование";
            ResetForm();
            TxtStatus.Text = "Редактирование отменено. Выберите столик на схеме зала.";
        }

        private void ResetForm()
        {
            if (_selectedTable != null)
            {
                HighlightTable(_selectedTable, false);
            }
            _selectedTable = null;
            TxtSelectedTable.Text = "Не выбран";
            TxtCustomerName.Text = "";
            TxtCustomerPhone.Text = "";
            TxtCustomerEmail.Text = "";
            TxtPersonsCount.Text = "2";
            TxtSpecialRequests.Text = "";
            ChkTableDecoration.IsChecked = false;
            ChkFlowers.IsChecked = false;
            ChkCandles.IsChecked = false;
            ChkBalloons.IsChecked = false;
            ChkLiveMusic.IsChecked = false;
            RbtnFamily.IsChecked = false;
            RbtnRomantic.IsChecked = false;
            RbtnCorporate.IsChecked = false;
            RbtnBanquet.IsChecked = false;
            BtnConfirmBooking.IsEnabled = false;
            BtnConfirmBooking.Content = "Подтвердить бронирование";
            DatePickerBooking.SelectedDate = DateTime.Today;
            ComboTime.SelectedIndex = 6;
            ComboDuration.SelectedIndex = 1;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTables();
            _selectedTable = null;
            TxtSelectedTable.Text = "Не выбран";
            BtnConfirmBooking.IsEnabled = false;
            TxtStatus.Text = "Схема обновлена. Выберите столик на схеме зала.";
        }
    }
}