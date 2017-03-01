using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfStavkaRacuni
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<StavkaRacuna> stavka = new List<StavkaRacuna>();
        List<Prodavnice> prodavnice = new List<Prodavnice>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DtpDatum.SelectedDate = DateTime.Now.Date;
            SqlConnection SqlConn = new SqlConnection(@"Data Source=DESKTOP-3QT4TNV\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True");
            SqlConn.Open();
            SqlCommand cmd = new SqlCommand("SELECT ProdavnicaId, Ime, Adresa FROM Prodavnica", SqlConn);
            SqlDataReader read = cmd.ExecuteReader();
            while (read.Read())
            {
                Prodavnice pr = new Prodavnice();
                int ProdavnicaId = read.GetInt32(0);
                string Ime = read.GetString(1);
                string Adresa = read.GetString(2);
                pr.ProdavnicaId = ProdavnicaId;
                pr.Ime = Ime;
                pr.Adresa = Adresa;
                prodavnice.Add(pr);
            }
            comboBox.ItemsSource = null;
            comboBox.ItemsSource = prodavnice;
        }

        private void buttonDodaj_Click(object sender, RoutedEventArgs e)
        {          
            int a;
            if (string.IsNullOrWhiteSpace(textBoxProizvod.Text) || !int.TryParse(textBoxKolicina.Text, out a) || comboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Niste uneli sve podatke", "Poruka", MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            else
            {
                Prodavnice pr = new Prodavnice();
                StavkaRacuna st = new StavkaRacuna();
                st.Proizvod = textBoxProizvod.Text;
                st.Kolicina = int.Parse(textBoxKolicina.Text);
                pr.Ime = comboBox.SelectedItem.ToString();

                stavka.Add(st);
                prodavnice.Add(pr);
                             
            }
            listBoxDodajProizvod.ItemsSource = null;
            listBoxDodajProizvod.ItemsSource = stavka;
            textBoxProizvod.Clear();
            textBoxKolicina.Clear();
            textBoxProizvod.Focus();
        }

        private void buttonSnimi_Click(object sender, RoutedEventArgs e)
        {

                SqlConnection SqlConn = new SqlConnection(@"Data Source=DESKTOP-3QT4TNV\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True");
                SqlConn.Open();
                SqlTransaction trans = SqlConn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted, "Transkacija Citanja");
                SqlCommand cmd = new SqlCommand("INSERT INTO Racun(DatumRacuna, ProdavnicaId) VALUES(@DatumRacuna, @ProdavnicaId) SELECT SCOPE_IDENTITY()", SqlConn, trans);
                cmd.Parameters.Add(new SqlParameter("@DatumRacuna", DtpDatum.SelectedDate));
                cmd.Parameters.Add(new SqlParameter("@ProdavnicaId", (comboBox.SelectedItem as Prodavnice).ProdavnicaId));
                int RacunId = int.Parse(cmd.ExecuteScalar().ToString());

                foreach (StavkaRacuna sr in stavka)
                {
                    SqlCommand command = new SqlCommand("INSERT INTO StavkaRacuna(RacunId, Proizvod, Kolicina) VALUES(@RacunId, @Proizvod, @Kolicina)", SqlConn, trans);
                    command.Parameters.Add(new SqlParameter("@RacunId", RacunId));
                    command.Parameters.Add(new SqlParameter("@Proizvod", sr.Proizvod));
                    command.Parameters.Add(new SqlParameter("@Kolicina", sr.Kolicina));

                int izvresno = command.ExecuteNonQuery();
                    if (izvresno == 0)
                    {
                        trans.Rollback();
                        MessageBox.Show("greska", "greska", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                MessageBox.Show("Proizvod je dodat");
                trans.Commit();
                SqlConn.Close();
                stavka.Clear();
                listBoxDodajProizvod.ItemsSource = null;
                listBoxDodajProizvod.ItemsSource = stavka;
                comboBox.SelectedIndex = -1;
                textBoxProizvod.Focus();
            }          
        }
    }   
