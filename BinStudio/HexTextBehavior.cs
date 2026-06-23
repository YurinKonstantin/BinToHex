using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio
{
    // ИСПРАВЛЕННЫЙ КЛАСС ОТРИСОВКИ (Добавлена защита от NullReferenceException)
    public static class HexTextBehavior
    {
        public static readonly DependencyProperty RowDataProperty =
            DependencyProperty.RegisterAttached("RowData", typeof(HexRowViewModel), typeof(HexTextBehavior), new PropertyMetadata(null, OnRowDataChanged));

        public static readonly DependencyProperty AsciiDataProperty =
            DependencyProperty.RegisterAttached("AsciiData", typeof(HexRowViewModel), typeof(HexTextBehavior), new PropertyMetadata(null, OnAsciiDataChanged));

        public static void SetRowData(DependencyObject element, HexRowViewModel value) => element.SetValue(RowDataProperty, value);
        public static HexRowViewModel GetRowData(DependencyObject element) => (HexRowViewModel)element.GetValue(RowDataProperty);

        public static void SetAsciiData(DependencyObject element, HexRowViewModel value) => element.SetValue(AsciiDataProperty, value);
        public static HexRowViewModel GetAsciiData(DependencyObject element) => (HexRowViewModel)element.GetValue(AsciiDataProperty);

        private static void OnRowDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                if (e.OldValue is HexRowViewModel oldRow)
                    oldRow.PropertyChanged -= (s, args) => { if (args.PropertyName == "RefreshInlines") RenderHexInlines(textBlock, oldRow); };

                if (e.NewValue is HexRowViewModel newRow)
                {
                    RenderHexInlines(textBlock, newRow);
                    newRow.PropertyChanged += (s, args) =>
                    {
                        if (args.PropertyName == "RefreshInlines" || args.PropertyName == "HexLine")
                            RenderHexInlines(textBlock, newRow);
                    };
                }
            }
        }

        private static void OnAsciiDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                if (e.OldValue is HexRowViewModel oldRow)
                    oldRow.PropertyChanged -= (s, args) => { if (args.PropertyName == "RefreshInlines") RenderAsciiInlines(textBlock, oldRow); };

                if (e.NewValue is HexRowViewModel newRow)
                {
                    RenderAsciiInlines(textBlock, newRow);
                    newRow.PropertyChanged += (s, args) =>
                    {
                        if (args.PropertyName == "RefreshInlines" || args.PropertyName == "AsciiLine")
                            RenderAsciiInlines(textBlock, newRow);
                    };
                }
            }
        }

        private static void RenderHexInlines(TextBlock tb, HexRowViewModel row)
        {
            tb.Inlines.Clear();
            if (row == null || string.IsNullOrEmpty(row.HexLine)) return;

            string[] parts = row.HexLine.Split(' ');

            for (int i = 0; i < parts.Length; i++)
            {
                string textWithSpace = parts[i] + (i < parts.Length - 1 ? " " : "");
                var run = new Run { Text = textWithSpace };

                long globalOffset = row.RowOffset + i;

                // 1. УСЛОВИЕ ПОДЦВЕТКИ ВЫДЕЛЕНИЯ
                if (row.ParentContext != null && row.ParentContext.IsByteSelected(globalOffset) && i < row.BytesCount)
                {
                    run.Foreground = new SolidColorBrush(Microsoft.UI.Colors.DodgerBlue);
                    run.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
                }
                // 2. УСЛОВИЕ ИЗМЕНЕННОГО БАЙТА: Исправлено обращение через BufferService!
                else if (row.ParentContext != null && row.ParentContext.BufferService.ModifiedBytes.ContainsKey(globalOffset))
                {
                    run.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                    run.FontWeight = Microsoft.UI.Text.FontWeights.SemiBold;
                }
                else
                {
                    if (Application.Current.Resources.TryGetValue("ApplicationForegroundThemeBrush", out object brush))
                        run.Foreground = brush as Brush;
                    else
                        run.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);

                    run.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
                }
                tb.Inlines.Add(run);
            }
        }


        private static void RenderAsciiInlines(TextBlock tb, HexRowViewModel row)
        {
            tb.Inlines.Clear();
            if (row == null || string.IsNullOrEmpty(row.AsciiLine)) return;

            for (int i = 0; i < row.AsciiLine.Length; i++)
            {
                var run = new Run { Text = row.AsciiLine[i].ToString() };

                long globalOffset = row.RowOffset + i;

                // 1. УСЛОВИЕ ПОДЦВЕТКИ ВЫДЕЛЕНИЯ
                if (row.ParentContext != null && row.ParentContext.IsByteSelected(globalOffset) && i < row.BytesCount)
                {
                    run.Foreground = new SolidColorBrush(Microsoft.UI.Colors.DodgerBlue);
                    run.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
                }
                // 2. УСЛОВИЕ ИЗМЕНЕННОГО БАЙТА (ASCII): Исправлено обращение через BufferService!
                else if (row.ParentContext != null && row.ParentContext.BufferService.ModifiedBytes.ContainsKey(globalOffset))
                {
                    run.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                    run.FontWeight = Microsoft.UI.Text.FontWeights.SemiBold;
                }
                else
                {
                    run.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
                    run.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
                }
                tb.Inlines.Add(run);
            }
        }
    }

}
