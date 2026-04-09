using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RestoAdmin.Common;
using RestoAdmin.Models;
using RestoAdmin.Services;

namespace RestoAdmin.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TableService _tableService;
        private readonly CustomerService _customerService;
        private readonly BookingService _bookingService;
        private List<Table> _allTables;
        private Table? _selectedTable;
        private string _statusText = string.Empty;
        private string _selectedTableInfo = "Не выбран";

        public MainViewModel(TableService tableService, CustomerService customerService, BookingService bookingService)
        {
            _tableService = tableService;
            _customerService = customerService;
            _bookingService = bookingService;
            LoadTables();
        }

        public ObservableCollection<Table> Tables { get; set; } = new ObservableCollection<Table>();
        public TableFilter Filter { get; set; } = new TableFilter();

        public Table? SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged();
                if (value != null)
                {
                    SelectedTableInfo = $"Стол №{value.TableNumber} | {value.Capacity} персоны | {_tableService.GetZoneName(value.ZoneId)}";
                }
                else
                {
                    SelectedTableInfo = "Не выбран";
                }
            }
        }

        public string SelectedTableInfo
        {
            get => _selectedTableInfo;
            set
            {
                _selectedTableInfo = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public void LoadTables()
        {
            _allTables = _tableService.GetAllTables();
            UpdateTablesDisplay();
            StatusText = "Добро пожаловать в ресторан «АдминСтол». Выберите столик на схеме зала.";
        }

        public void ApplyFilter()
        {
            var filtered = _tableService.GetFilteredTables(Filter);
            UpdateTablesDisplay(filtered);
            int freeCount = filtered.Count(t => t.Status == TableStatus.Free);
            StatusText = $"Найдено {freeCount} столиков, подходящих под пожелания клиента.";
        }

        public void ClearFilter()
        {
            Filter = new TableFilter();
            UpdateTablesDisplay();
            StatusText = "Фильтр сброшен. Выберите столик на схеме зала.";
        }

        public bool TrySelectTable(Table table)
        {
            if (table.Status != TableStatus.Free)
            {
                StatusText = $"Столик №{table.TableNumber} уже {_tableService.GetStatusText(table.Status)}. Выберите свободный столик.";
                return false;
            }

            SelectedTable = table;
            StatusText = $"Выбран столик №{table.TableNumber}. Заполните данные клиента и подтвердите бронирование.";
            return true;
        }

        public string CreateBooking(BookingData data)
        {
            if (SelectedTable == null)
                throw new System.InvalidOperationException("Столик не выбран");

            var customer = _customerService.GetOrCreateCustomer(data.CustomerName, data.CustomerPhone, data.CustomerEmail);
            data.CustomerId = customer.Id;
            data.TableId = SelectedTable.Id;

            var booking = _bookingService.CreateBooking(data);
            _tableService.BookTable(SelectedTable, booking);

            SelectedTable = null;
            LoadTables();

            return booking.BookingNumber;
        }

        private void UpdateTablesDisplay(List<Table>? filteredTables = null)
        {
            Tables.Clear();
            var displayList = filteredTables ?? _allTables;
            foreach (var table in displayList)
            {
                Tables.Add(table);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}