using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using LibrarieModele;

namespace WpfTargAuto
{
    public partial class AdaugaMasinaWindow : Window
    {
        // ── Constante pentru validare ────────────────────────────────
        private const int FIRMA_MIN = 2;
        private const int FIRMA_MAX = 30;
        private const int MODEL_MIN = 1;
        private const int MODEL_MAX = 30;
        private const int AN_MIN = 1990;
        private const int AN_MAX = 2025;
        private const int VIN_MIN = 5;
        private const int VIN_MAX = 20;

        private readonly SolidColorBrush _culoareNormala = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
        private readonly SolidColorBrush _culoareEroare = new SolidColorBrush(Colors.Red);

        // Retinem culoarea selectata curent
        private Culoare? _culoareSelectata = null;

        // Butoanele de culoare ca sa putem reseta border-ul
        private readonly Dictionary<Culoare, Border> _btnCulori = new Dictionary<Culoare, Border>();

        // Checkbox-urile pentru optiuni
        private readonly Dictionary<Optiuni, CheckBox> _chkOptiuni = new Dictionary<Optiuni, CheckBox>();

        public Auto MasinaAdaugata { get; private set; }

        public AdaugaMasinaWindow()
        {
            InitializeComponent();
            IncarcaCulori();
            IncarcaOptiuni();
        }

        // ── Construieste butoanele de culoare ────────────────────────
        private void IncarcaCulori()
        {
            // Fiecare culoare = un Border patrat colorat cu tooltip
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
                Color color = pereche.Value;

                // Patratel colorat
                var patrat = new Border
                {
                    Width = 36,
                    Height = 36,
                    CornerRadius = new CornerRadius(6),
                    Background = new SolidColorBrush(color),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66)),
                    BorderThickness = new Thickness(2),
                    Margin = new Thickness(0, 0, 8, 8),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    ToolTip = culoare.ToString()
                };

                // Salvam referinta ca sa putem modifica border-ul la selectie
                _btnCulori[culoare] = patrat;

                // Click pe patrat = selectare culoare
                patrat.MouseLeftButtonDown += (sender, e) =>
                {
                    SelecteazaCuloare(culoare);
                };

                pnlCulori.Children.Add(patrat);
            }
        }

        // ── Construieste checkbox-urile pentru optiuni ────────────────
        private void IncarcaOptiuni()
        {
            var optiuniDisponibile = new Dictionary<Optiuni, string>
            {
                { Optiuni.AerConditionat, "❄  Aer Conditionat"  },
                { Optiuni.Navigatie,      "🗺  Navigatie GPS"    },
                { Optiuni.CutieAutomata,  "⚙  Cutie Automata"   },
                { Optiuni.ScaunePiele,    "💺  Scaune Piele"     },
                { Optiuni.Xenon,          "💡  Xenon"            },
                { Optiuni.Panoramic,      "🌅  Trapa Panoramica" }
            };

            foreach (var pereche in optiuniDisponibile)
            {
                Optiuni optiune = pereche.Key;
                string etich = pereche.Value;

                // Fiecare optiune = un Border cu checkbox si text inauntru
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
                    Padding = new Thickness(10, 8, 10, 8),
                    Margin = new Thickness(0, 0, 8, 8),
                    Cursor = System.Windows.Input.Cursors.Hand
                };

                // Text langa checkbox
                var sp = new StackPanel { Orientation = Orientation.Horizontal };
                sp.Children.Add(chk);
                sp.Children.Add(new TextBlock
                {
                    Text = etich,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA)),
                    Margin = new Thickness(6, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                });

                container.Child = sp;

                // Click pe container = toggle checkbox
                container.MouseLeftButtonDown += (sender, e) =>
                {
                    chk.IsChecked = !chk.IsChecked;
                };

                // Cand checkbox-ul se schimba, actualizam aspectul containerului
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

                _chkOptiuni[optiune] = chk;
                pnlOptiuni.Children.Add(container);
            }
        }

        // ── Selectare culoare ────────────────────────────────────────
        private void SelecteazaCuloare(Culoare culoare)
        {
            // Resetam border-ul tuturor patratelor
            foreach (var pereche in _btnCulori)
            {
                pereche.Value.BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x66));
                pereche.Value.BorderThickness = new Thickness(2);
            }

            // Marcam culoarea selectata cu border alb si mai gros
            _btnCulori[culoare].BorderBrush = new SolidColorBrush(Colors.White);
            _btnCulori[culoare].BorderThickness = new Thickness(3);

            // Salvam selectia
            _culoareSelectata = culoare;

            // Actualizam preview-ul
            rectPreviewCuloare.Background = _btnCulori[culoare].Background;
            txtCuloareSelectata.Text = culoare.ToString();
            txtCuloareSelectata.Foreground = new SolidColorBrush(Color.FromRgb(0xEA, 0xEA, 0xEA));

            // Ascundem eroarea daca exista
            lblCuloare.Foreground = _culoareNormala;
            errCuloare.Visibility = Visibility.Collapsed;
        }

        // ── Buton Salveaza ───────────────────────────────────────────
        private void btnSalveaza_Click(object sender, RoutedEventArgs e)
        {
            if (!Valideaza())
                return;

            // Construim optiunile bifate
            Optiuni optiuniSelectate = Optiuni.Niciuna;
            foreach (var pereche in _chkOptiuni)
            {
                if (pereche.Value.IsChecked == true)
                    optiuniSelectate |= pereche.Key;
            }

            MasinaAdaugata = new Auto
            {
                Firma = txtFirma.Text.Trim(),
                Model = txtModel.Text.Trim(),
                AnFabricatie = int.Parse(txtAn.Text.Trim()),
                SerieSasiu = txtVin.Text.Trim(),
                Culoare = _culoareSelectata.Value,
                Optiuni = optiuniSelectate
            };

            DialogResult = true;
            Close();
        }

        // ── Buton Anuleaza ───────────────────────────────────────────
        private void btnAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ── Validare ─────────────────────────────────────────────────
        private bool Valideaza()
        {
            bool valid = true;

            // Firma
            string firma = txtFirma.Text.Trim();
            if (firma.Length < FIRMA_MIN || firma.Length > FIRMA_MAX)
            {
                lblFirma.Foreground = _culoareEroare;
                errFirma.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                lblFirma.Foreground = _culoareNormala;
                errFirma.Visibility = Visibility.Collapsed;
            }

            // Model
            string model = txtModel.Text.Trim();
            if (model.Length < MODEL_MIN || model.Length > MODEL_MAX)
            {
                lblModel.Foreground = _culoareEroare;
                errModel.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                lblModel.Foreground = _culoareNormala;
                errModel.Visibility = Visibility.Collapsed;
            }

            // An
            bool anValid = int.TryParse(txtAn.Text.Trim(), out int an)
                           && an >= AN_MIN && an <= AN_MAX;
            if (!anValid)
            {
                lblAn.Foreground = _culoareEroare;
                errAn.Text = $"Anul trebuie sa fie intre {AN_MIN} si {AN_MAX}!";
                errAn.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                lblAn.Foreground = _culoareNormala;
                errAn.Visibility = Visibility.Collapsed;
            }

            // VIN
            string vin = txtVin.Text.Trim();
            if (vin.Length < VIN_MIN || vin.Length > VIN_MAX)
            {
                lblVin.Foreground = _culoareEroare;
                errVin.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                lblVin.Foreground = _culoareNormala;
                errVin.Visibility = Visibility.Collapsed;
            }

            // Culoare
            if (_culoareSelectata == null)
            {
                lblCuloare.Foreground = _culoareEroare;
                errCuloare.Visibility = Visibility.Visible;
                valid = false;
            }
            else
            {
                lblCuloare.Foreground = _culoareNormala;
                errCuloare.Visibility = Visibility.Collapsed;
            }

            return valid;
        }
    }
}