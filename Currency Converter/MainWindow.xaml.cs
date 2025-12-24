using Microsoft.Data.SqlClient;
using System.Configuration;
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
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

        public MainWindow()
        {


            InitializeComponent();
            DataBinding(Currency_combobox);
            DataBinding(Currency_combobox1);
            GetData();
        }

        public void mycon()
        {
            string Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open();
        }

        private void DataBinding(ComboBox Currency)
        {
            //open connection 
            mycon();
            //Create an object for Datatable
            DataTable dt = new DataTable();
            //Write query to get data from table 
            cmd = new SqlCommand("select Id, CurrencyName from Currency_master", con);
            //commandtype we using for writing query
            cmd.CommandType = CommandType.Text;

            //data opvragen
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            //assigning de select row 
            DataRow Newrow = dt.NewRow();
            Newrow["Id"] = 0;
            Newrow["CurrencyName"] = "--SELECT--";
            dt.Rows.InsertAt(Newrow, 0);

            if (dt.Rows is not null && dt.Rows.Count > 0)
            {
                Currency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            Currency.DisplayMemberPath = "CurrencyName";
            Currency.SelectedValuePath = "Id";
            Currency.SelectedIndex = 0;
        }


        private void Cvrt_bttn_Click(object sender, RoutedEventArgs e)
        {

            bool parseinput = decimal.TryParse(input.Text, out decimal Amount);
            bool parseExchange = decimal.TryParse(Currency_combobox.SelectedValue.ToString(), out decimal ExchangeRate);
            bool parseExchange1 = decimal.TryParse(Currency_combobox1.SelectedValue.ToString(), out decimal ExchangeRate1);

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

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            DataRowView drv = dg.CurrentItem as DataRowView;

            if (drv != null)
            {
                if (Dgv_CurrencyMaster.Items.Count > 0)
                {
                    if (dg.SelectedCells.Count > 0)
                    {
                        CurrencyId = int.Parse(drv["Id"].ToString());

                        if (dg.SelectedCells[0].Column.DisplayIndex == 0)
                        {

                            Amount.Text = drv["Amount"].ToString();
                            Currency.Text = drv["CurrencyName"].ToString();
                            Save_bttn.Content = "Update";
                        }

                        if (dg.SelectedCells[0].Column.DisplayIndex == 1)
                        {
                            if (MessageBox.Show("Are u sure u want to delete?", "information", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    mycon();
                                    cmd = new SqlCommand("delete from Currency_master where Id=@Id", con);
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Delete statement succeeded", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch(Exception ex) 
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    con.Close();
                                    ClearMaster();
                                }
                            }

                        }
                    }
                }
            }
        }


        private void Save_bttn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Amount.Text.Trim()))
            {
                Error_Label_Data.Content = "The amount u gave is null or empty";
                Amount.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(Currency.Text.Trim()))
            {
                Error_Label_Data.Content = "The Currency u gave is null or empty";
                Currency.Focus();
                return;
            }
            else
            {
                if (CurrencyId > 0)
                {
                    if (MessageBox.Show("Are u sure u want to update?", "information", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            mycon();
                            cmd = new SqlCommand("update Currency_master SET CurrencyName = @CurrencyName, Amount = @Amount where Id=@Id", con);
                            cmd.Parameters.AddWithValue("@CurrencyName", Currency.Text);
                            cmd.Parameters.AddWithValue("@Amount", Amount.Text);
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Update statement succeeded","Information",MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            con.Close();
                            ClearMaster();
                        }
                    }
                }
                else
                {
                    if(MessageBox.Show("Are u sure u want to Insert?", "information", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            mycon();
                            cmd = new SqlCommand("insert into Currency_master values (@CurrencyName, @Amount)", con);
                            cmd.Parameters.AddWithValue("@CurrencyName", Currency.Text);
                            cmd.Parameters.AddWithValue("@Amount", Amount.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Insert statement succeeded", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            con.Close();
                            ClearMaster();
                        }
                    }
                }

            }
        }

        private void Cancel_bttn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            finally
            {
                MessageBox.Show("canceled action");
            }
        }

        private void ClearMaster()
        {
            Amount.Clear();
            Currency.Clear();
            Save_bttn.Content = "Save";
            GetData();
            CurrencyId = 0;
            DataBinding(Currency_combobox);
            DataBinding(Currency_combobox1);
            Amount.Focus();
        }

        private void GetData() 
        {
            //open connection 
            mycon();
            //Create an object for Datatable
            DataTable dt = new DataTable();
            //Write query to get data from table 
            cmd = new SqlCommand("select * from Currency_master", con);
            //commandtype we using for writing query
            cmd.CommandType = CommandType.Text;

            //data opvragen
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                Dgv_CurrencyMaster.ItemsSource = dt.DefaultView;
            }
            else
            {
                Dgv_CurrencyMaster.ItemsSource = null;
            }
            con.Close();

        }

    }
}