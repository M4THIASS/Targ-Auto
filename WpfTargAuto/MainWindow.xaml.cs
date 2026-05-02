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

        // Dictionar culori -> brush vizual
        private readonly Dictionary<Culoare, Color> _culoriBrush = new()
        {
            { Culoare.Alb,      Color.FromRgb(0xF0, 0xF0, 0xF0) },
            { Culoare.Negru,    Color.FromRgb(0x22, 0x22, 0x22) },
            { Culoare.Rosu,     Color.FromRgb(0xE7, 0x4C, 0x3C) },
            { Culoare.Albastru, Color.FromRgb(0x29, 0x80, 0xB9) },
            { Culoare.Gri,      Color.FromRgb(0x95, 0xA5, 0xA6) },
            { Culoare.Argintiu, Color.FromRgb(0xBD, 0xC3, 0xC7) }
        };

        // Checkbox-uri optiuni pentru formularele de adaugare si editare
        private readonly Dictionary<Optiuni, CheckBox> _chkAddOptiuni = new();
        private readonly Dictionary<Optiuni, CheckBox> _chkEditOptiuni = new();

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                IStocareDate stocare = new StocareTextService("date");
                _stocManager = new StocManager(stocare);

                IncarcaComboBoxCulori(cmbAddCuloare);
                IncarcaComboBoxCulori(cmbEditCuloare);
                IncarcaOptiuni(pnlAddOptiuni, _chkAddOptiuni);
                IncarcaOptiuni(pnlEditOptiuni, _chkEditOptiuni);

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
            if (paginaLista == null) return;

            paginaLista.Visibility = Visibility.Collapsed;
            paginaCautare.Visibility = Visibility.Collapsed;
            paginaAdauga.Visibility = Visibility.Collapsed;
            paginaEditeaza.Visibility = Visibility.Collapsed;

            if (sender == btnMeniuLista) paginaLista.Visibility = Visibility.Visible;
            else if (sender == btnMeniuCautare) paginaCautare.Visibility = Visibility.Visible;
            else if (sender == btnMeniuAdauga) paginaAdauga.Visibility = Visibility.Visible;
            else if (sender == btnMeniuEditeaza)
            {
                paginaEditeaza.Visibility = Visibility.Visible;

                // Populam lista cu toate masinile la intrarea pe pagina
                Auto[] toate = _stocManager.GetToate();
                var listaText = new List<string>();
                foreach (Auto m in toate)
                    listaText.Add($"{m.Firma} {m.Model} ({m.AnFabricatie})");

                lstEditMasini.ItemsSource = null;
                lstEditMasini.ItemsSource = listaText;
                lstEditMasini.Tag = toate;

                pnlEditFormular.Visibility = Visibility.Collapsed;
                txtEditPlaceholder.Visibility = Visibility.Visible;
            }
        }

        // ════════════════════════════════════════════════════════════
        // LISTA MASINI — folosim ListBox
        // ════════════════════════════════════════════════════════════

        private void AfiseazaLista()
        {
            lstMasini.ItemsSource = null;

            Auto[] masini = _stocManager.GetToate();
            var listaText = new List<string>();
            foreach (Auto m in masini)
                listaText.Add($"{m.Firma} {m.Model} ({m.AnFabricatie})");

            lstMasini.ItemsSource = listaText;

            txtSubtitluLista.Text = $"{masini.Length} vehicule inregistrate";
            txtStatus.Text = $"{masini.Length} masini";
        }

        private void lstMasini_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lstMasini.SelectedIndex;
            if (index < 0) return;

            Auto[] masini = _stocManager.GetToate();
            if (index < masini.Length)
                AfiseazaDetalii(masini[index]);
        }

        private void AfiseazaDetalii(Auto m)
        {
            _masinaSelectata = m;

            txtDetaliuTitlu.Text = $"{m.Firma} {m.Model}";
            txtFirma.Text = m.Firma;
            txtModel.Text = m.Model;
            txtAn.Text = m.AnFabricatie.ToString();
            txtVin.Text = m.SerieSasiu;
            txtData.Text = m.DataAdaugarii != DateTime.MinValue
                                   ? m.DataAdaugarii.ToShortDateString()
                                   : "Nedefinita";

            txtCuloare.Text = m.Culoare.ToString();
            rectCuloare.Background = GetCuloareBrush(m.Culoare);

            txtOptiuni.Text = m.Optiuni == Optiuni.Niciuna
                ? "Nicio optiune"
                : m.Optiuni.ToString().Replace(", ", "\n");

            txtPlaceholder.Visibility = Visibility.Collapsed;
            pnlDetalii.Visibility = Visibility.Visible;
        }

        // ════════════════════════════════════════════════════════════
        // CAUTARE — rezultate in ListBox
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

            if (rbFirma.IsChecked == true)
                rezultate = Array.FindAll(toate,
                    m => m.Firma.Contains(termen, StringComparison.OrdinalIgnoreCase));
            else if (rbAn.IsChecked == true)
                rezultate = Array.FindAll(toate,
                    m => m.AnFabricatie.ToString().Contains(termen));
            else
                rezultate = Array.FindAll(toate,
                    m => m.SerieSasiu.Contains(termen, StringComparison.OrdinalIgnoreCase));

            // Afisam in ListBox
            var listaRez = new List<string>();
            foreach (Auto m in rezultate)
                listaRez.Add($"{m.Firma} {m.Model} ({m.AnFabricatie}) - {m.Culoare}");

            lstRezultate.ItemsSource = null;
            lstRezultate.ItemsSource = listaRez;

            txtRezCautare.Text = rezultate.Length == 0
                ? "Niciun rezultat gasit."
                : $"{rezultate.Length} rezultat(e) gasite:";
        }

        // ════════════════════════════════════════════════════════════
        // ADAUGA MASINA
        // ════════════════════════════════════════════════════════════

        private void IncarcaComboBoxCulori(ComboBox cmb)
        {
            cmb.Items.Clear();
            foreach (Culoare c in Enum.GetValues(typeof(Culoare)))
            {
                if (c == Culoare.Nedefinita) continue;
                cmb.Items.Add(c.ToString());
            }
        }

        private void IncarcaOptiuni(WrapPanel panel, Dictionary<Optiuni, CheckBox> dictionar)
        {
            panel.Children.Clear();
            dictionar.Clear();

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
                container.MouseLeftButtonDown += (s, ev) => chk.IsChecked = !chk.IsChecked;

                chk.Checked += (s, ev) =>
                {
                    container.BorderBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71));
                    container.Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x2A, 0x1A));
                };
                chk.Unchecked += (s, ev) =>
                {
                    container.BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66));
                    container.Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x3E));
                };

                dictionar[opt] = chk;
                panel.Children.Add(container);
            }
        }

        private void cmbAddCuloare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAddCuloare.SelectedItem is string culoareStr
                && Enum.TryParse(culoareStr, out Culoare culoare)
                && _culoriBrush.ContainsKey(culoare))
            {
                rectAddPreview.Background = new SolidColorBrush(_culoriBrush[culoare]);
                txtAddCuloareSelectata.Text = culoareStr;
                errAddCuloare.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSalveazaInline_Click(object sender, RoutedEventArgs e)
        {
            if (!ValideazaFormularAdd()) return;

            Optiuni optiuni = Optiuni.Niciuna;
            foreach (var p in _chkAddOptiuni)
                if (p.Value.IsChecked == true)
                    optiuni |= p.Key;

            Enum.TryParse(cmbAddCuloare.SelectedItem?.ToString(), out Culoare culoare);

            var masina = new Auto
            {
                Firma = txtAddFirma.Text.Trim(),
                Model = txtAddModel.Text.Trim(),
                AnFabricatie = int.Parse(txtAddAn.Text.Trim()),
                SerieSasiu = txtAddVin.Text.Trim(),
                Culoare = culoare,
                Optiuni = optiuni,
                DataAdaugarii = dtpAddData.SelectedDate ?? DateTime.Today
            };

            bool ok = _stocManager.Adauga(masina);
            if (ok)
            {
                ResetFormularAdd();
                AfiseazaLista();
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
            ResetFormularAdd();
            btnMeniuLista.IsChecked = true;
        }

        private void ResetFormularAdd()
        {
            txtAddFirma.Text = "";
            txtAddModel.Text = "";
            txtAddAn.Text = "";
            txtAddVin.Text = "";
            cmbAddCuloare.SelectedIndex = -1;
            rectAddPreview.Background = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x3E));
            txtAddCuloareSelectata.Text = "Niciuna";
            dtpAddData.SelectedDate = null;

            foreach (var p in _chkAddOptiuni) p.Value.IsChecked = false;

            errAddFirma.Visibility = Visibility.Collapsed;
            errAddModel.Visibility = Visibility.Collapsed;
            errAddAn.Visibility = Visibility.Collapsed;
            errAddVin.Visibility = Visibility.Collapsed;
            errAddCuloare.Visibility = Visibility.Collapsed;
        }

        // ════════════════════════════════════════════════════════════
        // EDITARE MASINA
        // ════════════════════════════════════════════════════════════

        // Se apeleaza cand se scrie in casuta de cautare din pagina editare
        private void txtEditCautare_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termen = txtEditCautare.Text.Trim();
            Auto[] toate = _stocManager.GetToate();

            Auto[] filtrate = string.IsNullOrEmpty(termen)
                ? toate
                : Array.FindAll(toate,
                    m => m.Firma.Contains(termen, StringComparison.OrdinalIgnoreCase)
                      || m.Model.Contains(termen, StringComparison.OrdinalIgnoreCase));

            var listaText = new List<string>();
            foreach (Auto m in filtrate)
                listaText.Add($"{m.Firma} {m.Model} ({m.AnFabricatie})");

            lstEditMasini.ItemsSource = null;
            lstEditMasini.ItemsSource = listaText;

            // Salvam masinile filtrate ca tag ca sa le recuperam la selectie
            lstEditMasini.Tag = filtrate;

            // Ascundem formularul cand se schimba cautarea
            pnlEditFormular.Visibility = Visibility.Collapsed;
            txtEditPlaceholder.Visibility = Visibility.Visible;
        }

        // Se apeleaza cand se selecteaza o masina din lista de editare
        private void lstEditMasini_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lstEditMasini.SelectedIndex;
            if (index < 0) return;

            // Recuperam masina din tag
            Auto[]? filtrate = lstEditMasini.Tag as Auto[];
            if (filtrate == null || index >= filtrate.Length) return;

            Auto m = filtrate[index];
            _masinaSelectata = m;

            // Populam formularul
            txtEditTitlu.Text = $"Editezi: {m.Firma} {m.Model}";
            txtEditFirma.Text = m.Firma;
            txtEditModel.Text = m.Model;
            txtEditAn.Text = m.AnFabricatie.ToString();
            txtEditVin.Text = m.SerieSasiu;
            dtpEditData.SelectedDate = m.DataAdaugarii != DateTime.MinValue
                                       ? m.DataAdaugarii : DateTime.Today;

            cmbEditCuloare.SelectedItem = m.Culoare.ToString();

            foreach (var p in _chkEditOptiuni)
                p.Value.IsChecked = m.Optiuni.HasFlag(p.Key);

            txtEditPlaceholder.Visibility = Visibility.Collapsed;
            pnlEditFormular.Visibility = Visibility.Visible;
        }
        private void cmbEditCuloare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbEditCuloare.SelectedItem is string culoareStr
                && Enum.TryParse(culoareStr, out Culoare culoare)
                && _culoriBrush.ContainsKey(culoare))
            {
                rectEditPreview.Background = new SolidColorBrush(_culoriBrush[culoare]);
                txtEditCuloareSelectata.Text = culoareStr;
            }
        }

        private void btnSalveazaEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_masinaSelectata == null) return;
            if (!ValideazaFormularEdit()) return;

            // Scoatem masina veche din stoc
            _stocManager.Scoate(_masinaSelectata.SerieSasiu);

            // Construim masina modificata
            Optiuni optiuni = Optiuni.Niciuna;
            foreach (var p in _chkEditOptiuni)
                if (p.Value.IsChecked == true)
                    optiuni |= p.Key;

            Enum.TryParse(cmbEditCuloare.SelectedItem?.ToString(), out Culoare culoare);

            var masinaEditata = new Auto
            {
                Firma = txtEditFirma.Text.Trim(),
                Model = txtEditModel.Text.Trim(),
                AnFabricatie = int.Parse(txtEditAn.Text.Trim()),
                SerieSasiu = _masinaSelectata.SerieSasiu, // VIN raman neschimbat
                Culoare = culoare,
                Optiuni = optiuni,
                DataAdaugarii = dtpEditData.SelectedDate ?? DateTime.Today
            };

            _stocManager.Adauga(masinaEditata);
            AfiseazaLista();
            btnMeniuLista.IsChecked = true;
            AfiseazaDetalii(masinaEditata);

            MessageBox.Show("Masina a fost modificata cu succes!",
                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAnuleazaEdit_Click(object sender, RoutedEventArgs e)
        {
            btnMeniuLista.IsChecked = true;
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

            if (confirmare != MessageBoxResult.Yes) return;

            _stocManager.Scoate(_masinaSelectata.SerieSasiu);
            _masinaSelectata = null;
            pnlDetalii.Visibility = Visibility.Collapsed;
            txtPlaceholder.Visibility = Visibility.Visible;
            AfiseazaLista();
        }

        // ════════════════════════════════════════════════════════════
        // VALIDARE
        // ════════════════════════════════════════════════════════════

        private bool ValideazaFormularAdd()
        {
            bool valid = true;

            string firma = txtAddFirma.Text.Trim();
            errAddFirma.Visibility = firma.Length < FIRMA_MIN || firma.Length > FIRMA_MAX
                ? Visibility.Visible : Visibility.Collapsed;
            if (errAddFirma.Visibility == Visibility.Visible) valid = false;

            string model = txtAddModel.Text.Trim();
            errAddModel.Visibility = model.Length < MODEL_MIN || model.Length > MODEL_MAX
                ? Visibility.Visible : Visibility.Collapsed;
            if (errAddModel.Visibility == Visibility.Visible) valid = false;

            bool anOk = int.TryParse(txtAddAn.Text.Trim(), out int an)
                        && an >= AN_MIN && an <= AN_MAX;
            errAddAn.Text = $"Anul trebuie sa fie intre {AN_MIN} si {AN_MAX}!";
            errAddAn.Visibility = anOk ? Visibility.Collapsed : Visibility.Visible;
            if (!anOk) valid = false;

            string vin = txtAddVin.Text.Trim();
            errAddVin.Visibility = vin.Length < VIN_MIN || vin.Length > VIN_MAX
                ? Visibility.Visible : Visibility.Collapsed;
            if (errAddVin.Visibility == Visibility.Visible) valid = false;

            errAddCuloare.Visibility = cmbAddCuloare.SelectedItem == null
                ? Visibility.Visible : Visibility.Collapsed;
            if (errAddCuloare.Visibility == Visibility.Visible) valid = false;

            return valid;
        }

        private bool ValideazaFormularEdit()
        {
            bool valid = true;

            string firma = txtEditFirma.Text.Trim();
            errEditFirma.Visibility = firma.Length < FIRMA_MIN || firma.Length > FIRMA_MAX
                ? Visibility.Visible : Visibility.Collapsed;
            if (errEditFirma.Visibility == Visibility.Visible) valid = false;

            string model = txtEditModel.Text.Trim();
            errEditModel.Visibility = model.Length < MODEL_MIN || model.Length > MODEL_MAX
                ? Visibility.Visible : Visibility.Collapsed;
            if (errEditModel.Visibility == Visibility.Visible) valid = false;

            bool anOk = int.TryParse(txtEditAn.Text.Trim(), out int an)
                        && an >= AN_MIN && an <= AN_MAX;
            errEditAn.Text = $"Anul trebuie sa fie intre {AN_MIN} si {AN_MAX}!";
            errEditAn.Visibility = anOk ? Visibility.Collapsed : Visibility.Visible;
            if (!anOk) valid = false;

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
            return _culoriBrush.ContainsKey(culoare)
                ? new SolidColorBrush(_culoriBrush[culoare])
                : new SolidColorBrush(Colors.DarkGray);
        }
    }
}