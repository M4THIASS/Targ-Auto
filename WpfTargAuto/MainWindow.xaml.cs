using System;
using System.Collections.Generic;
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
        // ── Constante validare ───────────────────────────────────────
        private const int FIRMA_MIN = 2;
        private const int FIRMA_MAX = 30;
        private const int MODEL_MIN = 1;
        private const int MODEL_MAX = 30;
        private const int AN_MIN = 1990;
        private const int AN_MAX = 2025;
        private const int VIN_MIN = 5;
        private const int VIN_MAX = 20;

        private readonly StocManager _stocManager;
        private Auto? _masinaSelectata = null;

        // Pentru culori in formularul inline
        private Culoare? _culoareAddSelectata = null;
        private readonly Dictionary<Culoare, Border> _btnAddCulori = new();
        private readonly Dictionary<Optiuni, CheckBox> _chkAddOptiuni = new();

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                IStocareDate stocare = new StocareTextService("date");
                _stocManager = new StocManager(stocare);

                IncarcaFormularCulori();
                IncarcaFormularOptiuni();
                AfiseazaLista();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Eroare constructor");
            }
        }

        // ════════════════════════════════════════════════════════════
        // NAVIGARE MENIU
        // ════════════════════════════════════════════════════════════

        private void BtnMeniu_Checked(object sender, RoutedEventArgs e)
        {
            // Verificam ca paginile exista deja
            if (paginaLista == null || paginaCautare == null || paginaAdauga == null)
                return;

            paginaLista.Visibility = Visibility.Collapsed;
            paginaCautare.Visibility = Visibility.Collapsed;
            paginaAdauga.Visibility = Visibility.Collapsed;

            if (sender == btnMeniuLista)
                paginaLista.Visibility = Visibility.Visible;
            else if (sender == btnMeniuCautare)
                paginaCautare.Visibility = Visibility.Visible;
            else if (sender == btnMeniuAdauga)
                paginaAdauga.Visibility = Visibility.Visible;
        }

        // ════════════════════════════════════════════════════════════
        // LISTA MASINI
        // ════════════════════════════════════════════════════════════

        private void AfiseazaLista()
        {
            pnlLista.Children.Clear();
            Auto[] masini = _stocManager.GetToate();

            foreach (Auto m in masini)
            {
                var btn = new Button
                {
                    Content = $"{m.Firma} {m.Model} ({m.AnFabricatie})",
                    Tag = m,
                    Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x2E)),
                    Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA)),
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(10, 8, 10, 8),
                    Margin = new Thickness(0, 0, 0, 4),
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                btn.Click += BtnMasina_Click;
                pnlLista.Children.Add(btn);
            }

            txtSubtitluLista.Text = $"{masini.Length} vehicule inregistrate";
            txtStatus.Text = $"{masini.Length} masini";
        }

        private void BtnMasina_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Auto masina)
                AfiseazaDetalii(masina);
        }

        private void AfiseazaDetalii(Auto m)
        {
            _masinaSelectata = m;

            txtDetaliuTitlu.Text = $"{m.Firma} {m.Model}";
            txtFirma.Text = m.Firma;
            txtModel.Text = m.Model;
            txtAn.Text = m.AnFabricatie.ToString();
            txtVin.Text = m.SerieSasiu;

            txtCuloare.Text = m.Culoare.ToString();
            rectCuloare.Background = GetCuloareBrush(m.Culoare);

            txtOptiuni.Text = m.Optiuni == Optiuni.Niciuna
                ? "Nicio optiune"
                : m.Optiuni.ToString().Replace(", ", "\n");

            txtPlaceholder.Visibility = Visibility.Collapsed;
            pnlDetalii.Visibility = Visibility.Visible;
        }

        // ════════════════════════════════════════════════════════════
        // CAUTARE
        // ════════════════════════════════════════════════════════════

        private void btnCauta_Click(object sender, RoutedEventArgs e)
        {
            string termen = txtCautare.Text.Trim();
            if (string.IsNullOrEmpty(termen))
            {
                txtRezCautare.Text = "Introduceti un termen de cautare.";
                return;
            }

            Auto[] toate = _stocManager.GetToate();
            Auto[] rezultate;

            // Cautam dupa criteriul selectat cu RadioButton
            if (rbFirma.IsChecked == true)
            {
                rezultate = Array.FindAll(toate,
                    m => m.Firma.Contains(termen, StringComparison.OrdinalIgnoreCase));
            }
            else if (rbAn.IsChecked == true)
            {
                rezultate = Array.FindAll(toate,
                    m => m.AnFabricatie.ToString().Contains(termen));
            }
            else
            {
                rezultate = Array.FindAll(toate,
                    m => m.SerieSasiu.Contains(termen, StringComparison.OrdinalIgnoreCase));
            }

            // Afisam rezultatele
            pnlRezultate.Children.Clear();

            if (rezultate.Length == 0)
            {
                txtRezCautare.Text = "Niciun rezultat gasit.";
                return;
            }

            txtRezCautare.Text = $"{rezultate.Length} rezultat(e) gasite:";

            foreach (Auto m in rezultate)
            {
                var card = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x2E)),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(12, 8, 12, 8),
                    Margin = new Thickness(0, 0, 0, 6)
                };

                var sp = new StackPanel();
                sp.Children.Add(new TextBlock
                {
                    Text = $"{m.Firma} {m.Model} ({m.AnFabricatie})",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA))
                });
                sp.Children.Add(new TextBlock
                {
                    Text = $"VIN: {m.SerieSasiu}  |  {m.Culoare}",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x99)),
                    Margin = new Thickness(0, 3, 0, 0)
                });

                card.Child = sp;
                pnlRezultate.Children.Add(card);
            }
        }

        // ════════════════════════════════════════════════════════════
        // FORMULAR ADAUGA INLINE
        // ════════════════════════════════════════════════════════════

        private void IncarcaFormularCulori()
        {
            var culoriDisponibile = new Dictionary<Culoare, Color>
            {
                { Culoare.Alb,      Color.FromRgb(0xF0, 0xF0, 0xF0) },
                { Culoare.Negru,    Color.FromRgb(0x22, 0x22, 0x22) },
                { Culoare.Rosu,     Color.FromRgb(0xE7, 0x4C, 0x3C) },
                { Culoare.Albastru, Color.FromRgb(0x29, 0x80, 0xB9) },
                { Culoare.Gri,      Color.FromRgb(0x95, 0xA5, 0xA6) },
                { Culoare.Argintiu, Color.FromRgb(0xBD, 0xC3, 0xC7) }
            };

            foreach (var pereche in culoriDisponibile)
            {
                Culoare culoare = pereche.Key;
                var patrat = new Border
                {
                    Width = 32,
                    Height = 32,
                    CornerRadius = new CornerRadius(6),
                    Background = new SolidColorBrush(pereche.Value),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66)),
                    BorderThickness = new Thickness(2),
                    Margin = new Thickness(0, 0, 8, 8),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    ToolTip = culoare.ToString()
                };

                patrat.MouseLeftButtonDown += (s, e) => SelecteazaCuloareAdd(culoare);
                _btnAddCulori[culoare] = patrat;
                pnlAddCulori.Children.Add(patrat);
            }
        }

        private void SelecteazaCuloareAdd(Culoare culoare)
        {
            foreach (var p in _btnAddCulori)
            {
                p.Value.BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66));
                p.Value.BorderThickness = new Thickness(2);
            }

            _btnAddCulori[culoare].BorderBrush = new SolidColorBrush(Colors.White);
            _btnAddCulori[culoare].BorderThickness = new Thickness(3);

            _culoareAddSelectata = culoare;
            rectAddPreview.Background = _btnAddCulori[culoare].Background;
            txtAddCuloareSelectata.Text = culoare.ToString();
            txtAddCuloareSelectata.Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA));
            errAddCuloare.Visibility = Visibility.Collapsed;
        }

        private void IncarcaFormularOptiuni()
        {
            var optiuni = new Dictionary<Optiuni, string>
            {
                { Optiuni.AerConditionat, "Aer Conditionat" },
                { Optiuni.Navigatie,      "Navigatie GPS"   },
                { Optiuni.CutieAutomata,  "Cutie Automata"  },
                { Optiuni.ScaunePiele,    "Scaune Piele"    },
                { Optiuni.Xenon,          "Xenon"           },
                { Optiuni.Panoramic,      "Panoramic"       }
            };

            foreach (var pereche in optiuni)
            {
                Optiuni opt = pereche.Key;

                var chk = new CheckBox
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA)),
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                var container = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x3E)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(10, 7, 10, 7),
                    Margin = new Thickness(0, 0, 8, 8),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                var sp = new StackPanel { Orientation = Orientation.Horizontal };
                sp.Children.Add(chk);
                sp.Children.Add(new TextBlock
                {
                    Text = pereche.Value,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA)),
                    Margin = new Thickness(6, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                });

                container.Child = sp;
                container.MouseLeftButtonDown += (s, e) => chk.IsChecked = !chk.IsChecked;

                chk.Checked += (s, e) =>
                {
                    container.BorderBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71));
                    container.Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x2A, 0x1A));
                };
                chk.Unchecked += (s, e) =>
                {
                    container.BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66));
                    container.Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x3E));
                };

                _chkAddOptiuni[opt] = chk;
                pnlAddOptiuni.Children.Add(container);
            }
        }

        private void btnSalveazaInline_Click(object sender, RoutedEventArgs e)
        {
            if (!ValideazaFormular())
                return;

            Optiuni optiuni = Optiuni.Niciuna;
            foreach (var p in _chkAddOptiuni)
                if (p.Value.IsChecked == true)
                    optiuni |= p.Key;

            var masina = new Auto
            {
                Firma = txtAddFirma.Text.Trim(),
                Model = txtAddModel.Text.Trim(),
                AnFabricatie = int.Parse(txtAddAn.Text.Trim()),
                SerieSasiu = txtAddVin.Text.Trim(),
                Culoare = _culoareAddSelectata.Value,
                Optiuni = optiuni
            };

            bool ok = _stocManager.Adauga(masina);

            if (ok)
            {
                ResetFormular();
                AfiseazaLista();

                // Navigam automat la lista
                btnMeniuLista.IsChecked = true;
                AfiseazaDetalii(masina);

                MessageBox.Show($"{masina.Firma} {masina.Model} a fost adaugata!",
                    "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("VIN-ul exista deja in lista!",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnAnuleazaInline_Click(object sender, RoutedEventArgs e)
        {
            ResetFormular();
            btnMeniuLista.IsChecked = true;
        }

        private void ResetFormular()
        {
            txtAddFirma.Text = "";
            txtAddModel.Text = "";
            txtAddAn.Text = "";
            txtAddVin.Text = "";

            _culoareAddSelectata = null;
            rectAddPreview.Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x3E));
            txtAddCuloareSelectata.Text = "Niciuna";

            foreach (var p in _btnAddCulori)
            {
                p.Value.BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66));
                p.Value.BorderThickness = new Thickness(2);
            }

            foreach (var p in _chkAddOptiuni)
                p.Value.IsChecked = false;

            errAddFirma.Visibility = Visibility.Collapsed;
            errAddModel.Visibility = Visibility.Collapsed;
            errAddAn.Visibility = Visibility.Collapsed;
            errAddVin.Visibility = Visibility.Collapsed;
            errAddCuloare.Visibility = Visibility.Collapsed;
        }

        // ════════════════════════════════════════════════════════════
        // STERGERE
        // ════════════════════════════════════════════════════════════

        private void btnSterge_Click(object sender, RoutedEventArgs e)
        {
            if (_masinaSelectata == null)
            {
                MessageBox.Show("Selectati mai intai o masina din lista!",
                    "Atentie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult confirmare = MessageBox.Show(
                $"Sigur vrei sa stergi {_masinaSelectata.Firma} {_masinaSelectata.Model}?",
                "Confirmare stergere", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmare != MessageBoxResult.Yes)
                return;

            bool sters = _stocManager.Scoate(_masinaSelectata.SerieSasiu);

            if (sters)
            {
                _masinaSelectata = null;
                pnlDetalii.Visibility = Visibility.Collapsed;
                txtPlaceholder.Visibility = Visibility.Visible;
                AfiseazaLista();
            }
        }

        // ════════════════════════════════════════════════════════════
        // VALIDARE FORMULAR INLINE
        // ════════════════════════════════════════════════════════════

        private bool ValideazaFormular()
        {
            bool valid = true;
            var rosu = new SolidColorBrush(Colors.Red);
            var normal = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x99));

            string firma = txtAddFirma.Text.Trim();
            if (firma.Length < FIRMA_MIN || firma.Length > FIRMA_MAX)
            {
                errAddFirma.Visibility = Visibility.Visible;
                valid = false;
            }
            else errAddFirma.Visibility = Visibility.Collapsed;

            string model = txtAddModel.Text.Trim();
            if (model.Length < MODEL_MIN || model.Length > MODEL_MAX)
            {
                errAddModel.Visibility = Visibility.Visible;
                valid = false;
            }
            else errAddModel.Visibility = Visibility.Collapsed;

            bool anOk = int.TryParse(txtAddAn.Text.Trim(), out int an)
                        && an >= AN_MIN && an <= AN_MAX;
            if (!anOk)
            {
                errAddAn.Text = $"Anul trebuie sa fie intre {AN_MIN} si {AN_MAX}!";
                errAddAn.Visibility = Visibility.Visible;
                valid = false;
            }
            else errAddAn.Visibility = Visibility.Collapsed;

            string vin = txtAddVin.Text.Trim();
            if (vin.Length < VIN_MIN || vin.Length > VIN_MAX)
            {
                errAddVin.Visibility = Visibility.Visible;
                valid = false;
            }
            else errAddVin.Visibility = Visibility.Collapsed;

            if (_culoareAddSelectata == null)
            {
                errAddCuloare.Visibility = Visibility.Visible;
                valid = false;
            }
            else errAddCuloare.Visibility = Visibility.Collapsed;

            return valid;
        }

        // ════════════════════════════════════════════════════════════
        // HELPER
        // ════════════════════════════════════════════════════════════

        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            btnMeniuAdauga.IsChecked = true;
        }

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
    }
}