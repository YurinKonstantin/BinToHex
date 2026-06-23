using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio.Services
{
    public class SelectionService
    {
        public long StartOffset { get; set; } = -1;
        public long EndOffset { get; set; } = -1;
        public bool IsSelecting { get; set; } = false;
        public bool IsEditingMode { get; set; } = false;
        public bool IsHighNibbleEntered { get; set; } = false;
        public char HighNibbleChar { get; set; } = '0';

        public void StartSelection(long offset)
        {
            IsSelecting = true;
            IsEditingMode = false;
            IsHighNibbleEntered = false;
            StartOffset = offset;
            EndOffset = offset;
        }

        public bool UpdateEndOffset(long offset)
        {
            if (EndOffset == offset) return false;
            EndOffset = offset;
            return true;
        }

        public void ClearSelection()
        {
            StartOffset = -1;
            EndOffset = -1;
            IsEditingMode = false;
            IsHighNibbleEntered = false;
        }

        public bool IsByteSelected(long offset)
        {
            if (StartOffset == -1 || EndOffset == -1) return false;
            long min = Math.Min(StartOffset, EndOffset);
            long max = Math.Max(StartOffset, EndOffset);
            return offset >= min && offset <= max;
        }

        public string GetSelectionStatusText()
        {
            if (StartOffset == -1 || EndOffset == -1) return "нет выделения";
            long min = Math.Min(StartOffset, EndOffset);
            long max = Math.Max(StartOffset, EndOffset);
            long count = max - min + 1;
            return count == 1 ? $"0x{min:X8}" : $"0x{min:X8} - 0x{max:X8} ({count:N0} байт)";
        }
    }
}
