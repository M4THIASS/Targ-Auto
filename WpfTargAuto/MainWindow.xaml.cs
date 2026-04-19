using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibrarieModele;
using NivelStocareDate;
using NivelStocareDate.Stoc;

namespace WpfTargAuto
{
    public partial class MainWindow : Window
    {
        private readonly StocManager _stocManager;
        private Auto _masinaSelectata = null; // masina pe care a dat click userul

        public MainWindow()
        {
            InitializeComponent();

            IStocareDate stocare = new StocareTextService("date");
            _stocManager = new StocManager(stocare);

            AfiseazaLista();
        }

        // Adaugam cateva masini de test
        private void IncarcaDateDemo()
        {
            _stocManager.Adauga(new Auto
            {
                Firma = "BMW",
                Model = "Seria 3",
                AnFabricatie = 2018,
                SerieSasiu = "VIN123456789",
                Culoare = Culoare.Negru,
                Optiuni = Optiuni.AerConditionat | Optiuni.Navigatie | Optiuni.CutieAutomata
            });

            _stocManager.Adauga(new Auto
            {
                Firma = "Dacia",
                Model = "Logan",
                AnFabricatie = 2021,
                SerieSasiu = "VIN987654321",
                Culoare = Culoare.Alb,
                Optiuni = Optiuni.AerConditionat
            });

            _stocManager.Adauga(new Auto
            {
                Firma = "Volkswagen",
                Model = "Golf 8",
                AnFabricatie = 2022,
                SerieSasiu = "VIN555000AAA",
                Culoare = Culoare.Gri,
                Optiuni = Optiuni.AerConditionat | Optiuni.Navigatie | Optiuni.ScaunePiele
            });
        }

        // Construieste butoanele din lista stanga
        private void AfiseazaLista()
        {
            pnlLista.Children.Clear();
            Auto[] masini = _stocManager.GetToate();

            foreach (Auto m in masini)
            {
                Button btn = new Button
                {
                    Content = $"{m.Firma} {m.Model} ({m.AnFabricatie})",
                    Tag = m,
                    Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x3E)),
                    Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA)),
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(12, 10, 12, 10),
                    Margin = new Thickness(0, 0, 0, 6),
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 13,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                btn.Click += BtnMasina_Click;
                pnlLista.Children.Add(btn);
            }

            txtStatus.Text = $"{masini.Length} masini disponibile";
        }

        // Cand utilizatorul apasa pe o masina din lista
        private void BtnMasina_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Auto masina)
            {
                AfiseazaDetalii(masina);
            }
        }

        // Afiseaza detaliile masinii selectate in panoul din dreapta
        private void AfiseazaDetalii(Auto m)
        {
            _masinaSelectata = m; // retinem masina selectata pentru stergere

            txtTitlu.Text = $"{m.Firma} {m.Model}";
            txtSubtitlu.Text = $"An: {m.AnFabricatie}  |  VIN: {m.SerieSasiu}";

            txtFirma.Text = m.Firma;
            txtModel.Text = m.Model;
            txtAn.Text = m.AnFabricatie.ToString();
            txtVin.Text = m.SerieSasiu;

            txtCuloare.Text = m.Culoare.ToString();
            rectCuloare.Background = GetCuloareBrush(m.Culoare);

            if (m.Optiuni == Optiuni.Niciuna)
                txtOptiuni.Text = "Nicio optiune";
            else
                txtOptiuni.Text = m.Optiuni.ToString().Replace(", ", "\n");

            pnlDetalii.Visibility = Visibility.Visible;
            txtStatus.Text = $"Selectat: {m.Firma} {m.Model}";
        }

        // Returneaza o culoare vizuala corespunzatoare enum-ului Culoare
        private SolidColorBrush GetCuloareBrush(Culoare culoare)
        {
            switch (culoare)
            {
                case Culoare.Alb: return new SolidColorBrush(Colors.WhiteSmoke);
                case Culoare.Negru: return new SolidColorBrush(Colors.DimGray);
                case Culoare.Rosu: return new SolidColorBrush(Colors.Crimson);
                case Culoare.Albastru: return new SolidColorBrush(Colors.SteelBlue);
                case Culoare.Gri: return new SolidColorBrush(Colors.Gray);
                case Culoare.Argintiu: return new SolidColorBrush(Colors.Silver);
                default: return new SolidColorBrush(Colors.DarkGray);
            }
        }

        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            AdaugaMasinaWindow fereastra = new AdaugaMasinaWindow();
            fereastra.Owner = this;

            bool? rezultat = fereastra.ShowDialog();

            if (rezultat == true)
            {
                bool adaugat = _stocManager.Adauga(fereastra.MasinaAdaugata);

                if (adaugat)
                {
                    AfiseazaLista();
                    AfiseazaDetalii(fereastra.MasinaAdaugata);
                    MessageBox.Show("Masina a fost adaugata cu succes!",
                        "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("VIN-ul exista deja in lista!",
                        "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void btnSterge_Click(object sender, RoutedEventArgs e)
        {
            // Verificam daca userul a selectat o masina
            if (_masinaSelectata == null)
            {
                MessageBox.Show("Selectati mai intai o masina din lista!",
                    "Atentie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Cerem confirmare inainte de stergere
            MessageBoxResult confirmare = MessageBox.Show(
                $"Sigur vrei sa stergi {_masinaSelectata.Firma} {_masinaSelectata.Model}?",
                "Confirmare stergere",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmare != MessageBoxResult.Yes)
                return;

            bool sters = _stocManager.Scoate(_masinaSelectata.SerieSasiu);

            if (sters)
            {
                _masinaSelectata = null;
                pnlDetalii.Visibility = Visibility.Collapsed;
                txtTitlu.Text = "Selectati o masina";
                txtSubtitlu.Text = "Apasati pe o masina din lista";
                txtStatus.Text = "Masina stearsa cu succes";
                AfiseazaLista();
            }
            else
            {
                MessageBox.Show("Nu s-a putut sterge masina.",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}