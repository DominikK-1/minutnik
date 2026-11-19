using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace minutnik;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private TimeSpan _timeRemaining;
    private bool _isRunning;
    private bool _isBreak;
    private int _completedCycles;
    private readonly List<string> _history = new();
    private int _workDurationMinutes = 25;
    private int _breakDurationMinutes = 5;

    public MainWindow()
    {
        _timeRemaining = TimeSpan.FromMinutes(_workDurationMinutes);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;

        InitializeComponent();
        UpdateDisplay();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_timeRemaining > TimeSpan.FromSeconds(1))
        {
            _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1));
        }
        else
        {
            if (!_isBreak)
            {
                _completedCycles++;
                _history.Add($"{DateTime.Now:HH:mm} — cykl {_completedCycles}");
                HistoryList.ItemsSource = _history.ToArray();
                _isBreak = true;
                _timeRemaining = TimeSpan.FromMinutes(_breakDurationMinutes);
            }
            else
            {
                _isBreak = false;
                _timeRemaining = TimeSpan.FromMinutes(_workDurationMinutes);
                _timer.Stop();
                _isRunning = false;
            }
        }

        UpdateDisplay();
    }

    private void StartPauseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_isRunning)
        {
            _timer.Stop();
            _isRunning = false;
        }
        else
        {
            _timer.Start();
            _isRunning = true;
        }

        UpdateDisplay();
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        _timer.Stop();
        _isRunning = false;
        _isBreak = false;
        _completedCycles = 0;
        _timeRemaining = TimeSpan.FromMinutes(_workDurationMinutes);

        UpdateDisplay();
    }

    private void WorkDurationNumericUpDown_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (TimerTextBlock == null)
        {
            return;
        }

        if (WorkDurationNumericUpDown?.Value.HasValue == true)
        {
            int newDuration = Math.Max(1, (int)WorkDurationNumericUpDown.Value.Value);
            _workDurationMinutes = newDuration;

            if (!_isRunning && !_isBreak)
            {
                _timeRemaining = TimeSpan.FromMinutes(_workDurationMinutes);
                UpdateDisplay();
            }
        }
    }

    private void BreakDurationNumericUpDown_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (TimerTextBlock == null)
        {
            return;
        }

        if (BreakDurationNumericUpDown?.Value.HasValue == true)
        {
            int newDuration = Math.Max(1, (int)BreakDurationNumericUpDown.Value.Value);
            _breakDurationMinutes = newDuration;

            if (!_isRunning && _isBreak)
            {
                _timeRemaining = TimeSpan.FromMinutes(_breakDurationMinutes);
                UpdateDisplay();
            }
        }
    }

    private void UpdateDisplay()
    {
        if (TimerTextBlock != null)
        {
            TimerTextBlock.Text = $"{(int)_timeRemaining.TotalMinutes:D2}:{_timeRemaining.Seconds:D2}";
        }

        if (CycleCountTextBlock != null)
        {
            CycleCountTextBlock.Text = _completedCycles.ToString();
        }

        if (ModeTextBlock != null)
        {
            ModeTextBlock.Text = _isBreak ? "Przerwa" : "Praca";
            ModeTextBlock.Foreground = _isBreak
                ? new SolidColorBrush(Color.Parse("#16A34A"))
                : new SolidColorBrush(Color.Parse("#0284C7"));
        }

        if (StartPauseButton != null)
        {
            StartPauseButton.Content = _isRunning ? "Pauza" : "Start";
        }
    }
}
