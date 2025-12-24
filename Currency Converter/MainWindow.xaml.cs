using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Currency_Converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataBinding(Currency_combobox);
            DataBinding(Currency_combobox1);
        }

        private void DataBinding(ComboBox Currency)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Currency");
            dt.Columns.Add("Exchange Rate");

            dt.Rows.Add("--Select--", 0);
            dt.Rows.Add("EUR", 1);
            dt.Rows.Add("POND", 2);
            dt.Rows.Add("USD", 1.2);
            dt.Rows.Add("INR", 2);

            Currency.ItemsSource = dt.DefaultView;

            Currency.DisplayMemberPath = "Currency";
            Currency.SelectedValuePath = "Exchange Rate";
            Currency.SelectedIndex = 0;
        }


        private void Cvrt_bttn_Click(object sender, RoutedEventArgs e)
        {

            bool parseinput = decimal.TryParse(input.Text, out decimal Amount);
            bool parseExchange = decimal.TryParse((string)Currency_combobox.SelectedValue, out decimal ExchangeRate);
            bool parseExchange1 = decimal.TryParse((string)Currency_combobox1.SelectedValue, out decimal ExchangeRate1);

            Error_label.Content = "";

            if (input.Text == null || input.Text == "")
            {
                Error_label.Content = "Please give an amount";
                input.Focus();
            }
            else if (ExchangeRate == 0 || Currency_combobox.SelectedIndex == 0)
            {
                Error_label.Content = "Please select the from exchangeRate";
                Currency_combobox.Focus();
            }
            else if (ExchangeRate1 == 0 || Currency_combobox1.SelectedIndex == 0)
            {
                Error_label.Content = "Please select the to exchangeRate";
                Currency_combobox1.Focus();
            }
            else if (!parseinput || !parseExchange)
            {
                Error_label.Content = "Please give a correct number, number too big or too small";
                input.Focus();

            }
            else
            {

                decimal ConvertedValue = Convertvalue(Amount, ExchangeRate, ExchangeRate1);
                Currency_Lbl.Content = $"{Currency_combobox1.Text}: {ConvertedValue.ToString("N3")}";
            }

        }

        private void Clear_bttn_Click(object sender, RoutedEventArgs e)
        {
            input.Clear();
            Error_label.Content = "";

        }

        public decimal Convertvalue(decimal value, decimal exchange_rate, decimal exchange_rate1)
        {
            return (value * exchange_rate) / exchange_rate1;
        }

        private void input_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex("[^0-9]+,[^0-9]");
            e.Handled = reg.IsMatch(e.Text);

        }
    }
}