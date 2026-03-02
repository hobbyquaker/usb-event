using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// ── Konfigurations-Editor ──────────────────────────────────────────────────────

sealed class ConfigEditorForm : Form
{
    readonly string _configPath;
    readonly Action _onSaved;

    readonly CheckBox _rawCheck;
    readonly TextBox  _rawTextBox;
    readonly Panel    _rawPanel;
    readonly Panel    _visualPanel;
    readonly Panel    _cardContainer;
    readonly Panel    _scrollContainer;

    static readonly Color Bg         = Theme.Dark ? Color.FromArgb(32, 32, 32)    : Color.FromArgb(243, 243, 243);
    static readonly Color CardBg     = Theme.Dark ? Color.FromArgb(45, 45, 45)    : Color.White;
    static readonly Color AccentBlue = Color.FromArgb(0, 103, 192);
    static readonly Color Border     = Theme.Dark ? Color.FromArgb(62, 62, 62)    : Color.FromArgb(218, 218, 218);
    static readonly Color TextMuted  = Theme.Dark ? Color.FromArgb(155, 155, 155) : Color.FromArgb(86, 86, 86);
    static readonly Color TextNorm   = Theme.Dark ? Color.FromArgb(240, 240, 240) : Color.FromArgb(30, 30, 30);

    public ConfigEditorForm(string configPath, Action onSaved)
    {
        _configPath   = configPath;
        _onSaved      = onSaved;
        Text          = Loc.T.MenuEditConfig.TrimEnd('.','…');
        Size          = new Size(720, 580);
        MinimumSize   = new Size(520, 420);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor     = Bg;
        ForeColor     = TextNorm;
        Font          = new Font("Segoe UI", 9.5f);

        // ── Top bar ───────────────────────────────────────────────────────────
        var topBar = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = CardBg };
        topBar.Paint += (_, e) =>
            e.Graphics.DrawLine(new Pen(Border), 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);

        _rawCheck = new CheckBox
        {
            Text      = Loc.T.RawLabel,
            AutoSize  = true,
            Location  = new Point(14, 12),
            Font      = new Font("Segoe UI", 9.5f),
            ForeColor = TextNorm,
            Cursor    = Cursors.Hand,
        };

        var hintLabel = new Label
        {
            Text      = Loc.T.QuickSelectHint,
            AutoSize  = true,
            Font      = new Font("Segoe UI", 8.5f),
            ForeColor = TextMuted,
            Cursor    = Cursors.Default,
        };
        topBar.Controls.Add(_rawCheck);
        topBar.Controls.Add(hintLabel);
        topBar.Resize += (_, _) =>
            hintLabel.Location = new Point(topBar.Width - hintLabel.Width - 14,
                (topBar.Height - hintLabel.Height) / 2);

        // ── Raw panel ─────────────────────────────────────────────────────────
        _rawPanel   = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(12, 8, 12, 8), BackColor = Bg };
        _rawTextBox = new TextBox
        {
            Multiline   = true,
            ScrollBars  = ScrollBars.Both,
            WordWrap    = false,
            Font        = new Font("Consolas", 10f),
            Dock        = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor   = CardBg,
            ForeColor   = TextNorm,
        };
        _rawPanel.Controls.Add(_rawTextBox);

        // ── Visual panel ──────────────────────────────────────────────────────
        _visualPanel     = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 12, 4), BackColor = Bg };
        _scrollContainer = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Bg };
        _cardContainer   = new Panel { BackColor = Bg, Location = Point.Empty };
        _scrollContainer.Controls.Add(_cardContainer);
        _scrollContainer.Resize += (_, _) => RelayoutCards();

        var addRow = new Panel { Dock = DockStyle.Bottom, Height = 44, BackColor = Bg };
        var addBtn = new Button
        {
            Text      = Loc.T.AddDevice,
            AutoSize  = true,
            Location  = new Point(2, 8),
            FlatStyle = FlatStyle.Flat,
            ForeColor = AccentBlue,
            BackColor = CardBg,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
        };
        addBtn.FlatAppearance.BorderColor = AccentBlue;
        addBtn.FlatAppearance.BorderSize  = 1;
        addBtn.Click += (_, _) => { AddCard(); RelayoutCards(); };
        addRow.Controls.Add(addBtn);

        _visualPanel.Controls.Add(_scrollContainer);
        _visualPanel.Controls.Add(addRow);

        // ── Content container ─────────────────────────────────────────────────
        var content = new Panel { Dock = DockStyle.Fill, BackColor = Bg };
        content.Controls.Add(_rawPanel);
        content.Controls.Add(_visualPanel);

        // ── Button bar ────────────────────────────────────────────────────────
        var btnBar = new Panel { Dock = DockStyle.Bottom, Height = 54, BackColor = CardBg };
        btnBar.Paint += (_, e) =>
            e.Graphics.DrawLine(new Pen(Border), 0, 0, btnBar.Width, 0);

        var btnSave   = MakePrimaryBtn(Loc.T.BtnSave);
        var btnCancel = MakeSecondaryBtn(Loc.T.BtnCancel);
        btnSave.Anchor = btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSave.Location   = new Point(btnBar.Width - 106, 11);
        btnCancel.Location = new Point(btnBar.Width - 208, 11);
        btnCancel.Click += (_, _) => Close();
        btnSave.Click   += (_, _) => Save();
        btnBar.Controls.AddRange(new Control[] { btnSave, btnCancel });

        // ── Assemble ──────────────────────────────────────────────────────────
        Controls.Add(content);
        Controls.Add(btnBar);
        Controls.Add(topBar);

        _rawCheck.CheckedChanged += (_, _) => SwitchMode();
        Load += (_, _) =>
        {
            RelayoutCards();
            if (Theme.Dark) NativeMethods.SetDarkTitleBar(Handle);
        };

        var yaml = File.Exists(configPath)
            ? File.ReadAllText(configPath, Encoding.UTF8)
            : string.Empty;
        _rawTextBox.Text = yaml;
        TryPopulateVisual(yaml);
    }

    // ── Mode switching ────────────────────────────────────────────────────────

    void SwitchMode()
    {
        if (_rawCheck.Checked)
        {
            _rawTextBox.Text     = BuildYaml();
            _rawPanel.Visible    = true;
            _visualPanel.Visible = false;
        }
        else
        {
            _cardContainer.Controls.Clear();
            TryPopulateVisual(_rawTextBox.Text);
            RelayoutCards();
            _rawPanel.Visible    = false;
            _visualPanel.Visible = true;
        }
    }

    void TryPopulateVisual(string yaml)
    {
        try
        {
            var cfg = string.IsNullOrWhiteSpace(yaml)
                ? new AppConfig()
                : new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build()
                    .Deserialize<AppConfig>(yaml) ?? new AppConfig();
            foreach (var d in cfg.Devices)
                AddCard(d.DeviceId, d.Executable, d.Arguments ?? string.Empty);
        }
        catch { }
    }

    // ── Card management ───────────────────────────────────────────────────────

    void AddCard(string id = "", string exe = "", string args = "")
        => _cardContainer.Controls.Add(BuildCard(id, exe, args));

    void RelayoutCards()
    {
        int w = Math.Max(_scrollContainer.ClientSize.Width - 2, 400);
        int y = 0;
        foreach (Control c in _cardContainer.Controls)
        {
            c.Location = new Point(0, y);
            c.Width    = w;
            y         += c.Height + 8;
        }
        _cardContainer.Size = new Size(w, Math.Max(y, 1));
    }

    Control BuildCard(string id, string exe, string args)
    {
        const int LW = 92, P = 14, Gap = 10;

        var txtId   = MakeInput(id);
        var txtExe  = MakeInput(exe);
        var txtArgs = MakeInput(args);

        int rh    = txtId.PreferredHeight;
        int cardH = P + rh * 3 + Gap * 2 + P + 2;

        var card = new Panel { Height = cardH, BackColor = CardBg };
        card.Paint += (_, e) =>
        {
            using var pen = new Pen(Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        var removeBtn = new Button
        {
            Text      = "✕",
            Size      = new Size(24, 24),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Theme.Dark ? Color.FromArgb(160, 160, 160) : Color.FromArgb(110, 110, 110),
            BackColor = Color.Transparent,
            Font      = new Font("Segoe UI", 8.5f),
            Anchor    = AnchorStyles.Top | AnchorStyles.Right,
            Cursor    = Cursors.Hand,
        };
        removeBtn.FlatAppearance.BorderSize = 0;
        removeBtn.Click += (_, _) =>
        {
            var result = MessageBox.Show(
                Loc.T.DeleteConfirmBody,
                Loc.T.DeleteConfirmTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes) return;

            _cardContainer.Controls.Remove(card);
            card.Dispose();
            RelayoutCards();
        };

        var browseBtn = new Button
        {
            Width     = 30,
            Height    = rh,
            Text      = "…",
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Dark ? Color.FromArgb(62, 62, 62) : Color.FromArgb(238, 238, 238),
            ForeColor = TextNorm,
            Font      = new Font("Segoe UI", 9f),
            Cursor    = Cursors.Hand,
        };
        browseBtn.FlatAppearance.BorderColor = Border;
        browseBtn.Click += (_, _) =>
        {
            using var dlg = new OpenFileDialog
            {
                Title  = Loc.T.BrowseTitle,
                Filter = Loc.T.BrowseFilter,
            };
            if (!string.IsNullOrEmpty(txtExe.Text)) dlg.FileName = txtExe.Text;
            var dir = Path.GetDirectoryName(txtExe.Text);
            if (Directory.Exists(dir)) dlg.InitialDirectory = dir;
            if (dlg.ShowDialog() == DialogResult.OK)
                txtExe.Text = dlg.FileName;
        };

        var historyBtn = new Button
        {
            Width     = 30,
            Height    = rh,
            Text      = "▾",
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Dark ? Color.FromArgb(62, 62, 62) : Color.FromArgb(238, 238, 238),
            ForeColor = TextNorm,
            Font      = new Font("Segoe UI", 9f),
            Cursor    = Cursors.Hand,
        };
        historyBtn.FlatAppearance.BorderColor = Border;
        historyBtn.Click += (_, _) =>
        {
            var recent = DeviceHistory.Load();
            var popup  = new ContextMenuStrip();
            if (Theme.Dark) popup.Renderer = new DarkMenuRenderer();
            if (!recent.Any())
            {
                popup.Items.Add(new ToolStripMenuItem(Loc.T.NoRecentDevices) { Enabled = false });
            }
            else
            {
                foreach (var d in recent)
                {
                    var label    = d.Name != d.DeviceId ? $"{d.Name}  —  {d.DeviceId}" : d.DeviceId;
                    var captured = d.DeviceId;
                    popup.Items.Add(label, null, (_, _) => txtId.Text = captured);
                }
            }
            popup.Show(txtId, new Point(0, txtId.Height));
        };

        card.Controls.AddRange(new Control[]
            { removeBtn, MakeLabel(Loc.T.LabelDeviceId), txtId, historyBtn,
                         MakeLabel(Loc.T.LabelProgram),  txtExe, browseBtn,
                         MakeLabel(Loc.T.LabelArgs),     txtArgs });

        card.Tag = new CardInputs(txtId, txtExe, txtArgs);

        void Layout()
        {
            int iw = card.Width - LW - P * 2;
            int x  = P + LW;
            removeBtn.Location = new Point(card.Width - 32, 6);

            // all three inputs share the same width, constrained by the tightest row:
            // row 1 needs room for historyBtn (30) + gap (4) + removeBtn (24) + gap (8)
            int txtWidth = iw - historyBtn.Width - 4 - removeBtn.Width - 8;

            int y = P;
            card.Controls[1].Location = new Point(P, y + 3);   // lblId
            txtId.Location      = new Point(x, y); txtId.Width = txtWidth;
            historyBtn.Location = new Point(x + txtWidth + 4, y);

            y += rh + Gap;
            card.Controls[4].Location = new Point(P, y + 3);   // lblExe
            txtExe.Location     = new Point(x, y); txtExe.Width = txtWidth;
            browseBtn.Location  = new Point(x + txtWidth + 4, y);

            y += rh + Gap;
            card.Controls[7].Location = new Point(P, y + 3);   // lblArgs
            txtArgs.Location    = new Point(x, y); txtArgs.Width = txtWidth;
        }

        Layout();
        card.Resize += (_, _) => Layout();
        return card;
    }

    // ── Save / YAML ───────────────────────────────────────────────────────────

    void Save()
    {
        try
        {
            var yaml = _rawCheck.Checked ? _rawTextBox.Text : BuildYaml();
            File.WriteAllText(_configPath, yaml, Encoding.UTF8);
            _onSaved();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(string.Format(Loc.T.SaveError, ex.Message), Loc.T.ErrorTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    string BuildYaml()
    {
        var sb = new StringBuilder();
        sb.AppendLine("devices:");
        foreach (Control c in _cardContainer.Controls)
        {
            if (c.Tag is CardInputs d)
            {
                sb.AppendLine($"  - deviceId: {Ys(d.Id.Text.Trim())}");
                sb.AppendLine($"    executable: {Ys(d.Exe.Text.Trim())}");
                if (!string.IsNullOrWhiteSpace(d.Args.Text))
                    sb.AppendLine($"    arguments: {Ys(d.Args.Text.Trim())}");
            }
        }
        return sb.ToString();
    }

    static string Ys(string v) => string.IsNullOrEmpty(v) ? "''"
        : v.Contains('\'')
            ? $"\"{v.Replace("\\", "\\\\").Replace("\"", "\\\"")}\""
            : $"'{v}'";

    // ── Control factories ─────────────────────────────────────────────────────

    static Label MakeLabel(string text) => new()
    {
        Text      = text,
        AutoSize  = true,
        ForeColor = TextMuted,
        Font      = new Font("Segoe UI", 9f),
    };

    static TextBox MakeInput(string text) => new()
    {
        Text        = text,
        BorderStyle = BorderStyle.FixedSingle,
        Font        = new Font("Segoe UI", 9.5f),
        BackColor   = CardBg,
        ForeColor   = TextNorm,
    };

    static Button MakePrimaryBtn(string text)
    {
        var b = new Button
        {
            Text      = text,
            Width     = 94,
            Height    = 32,
            FlatStyle = FlatStyle.Flat,
            BackColor = AccentBlue,
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
        };
        b.FlatAppearance.BorderSize          = 0;
        b.FlatAppearance.MouseOverBackColor  = Color.FromArgb(0, 84, 166);
        b.FlatAppearance.MouseDownBackColor  = Color.FromArgb(0, 66, 140);
        return b;
    }

    static Button MakeSecondaryBtn(string text)
    {
        var b = new Button
        {
            Text      = text,
            Width     = 94,
            Height    = 32,
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Dark ? Color.FromArgb(62, 62, 62) : Color.FromArgb(238, 238, 238),
            ForeColor = TextNorm,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
        };
        b.FlatAppearance.BorderColor        = Border;
        b.FlatAppearance.MouseOverBackColor = Theme.Dark ? Color.FromArgb(76, 76, 76) : Color.FromArgb(225, 225, 225);
        b.FlatAppearance.MouseDownBackColor = Theme.Dark ? Color.FromArgb(50, 50, 50) : Color.FromArgb(210, 210, 210);
        return b;
    }
}

record CardInputs(TextBox Id, TextBox Exe, TextBox Args);
