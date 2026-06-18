using System;
using System.Collections.Generic;
using System.Text;

namespace BinViewer
{
    public class HexRow
    {
        public long Address { get; set; }
        public string AddressHex => Address.ToString("X8");
        public byte[] Bytes { get; set; }

        // Количество реальных байт в этой строке (для конца файла)
        public int ValidBytesCount { get; set; }

        // Формируем красивую строку из 16 гекс-пар
        public string HexString
        {
            get
            {
                var sb = new System.Text.StringBuilder(47); // 16 * 2 символа + 15 пробелов
                for (int i = 0; i < 16; i++)
                {
                    if (i < ValidBytesCount)
                    {
                        sb.Append(Bytes[i].ToString("X2"));
                    }
                    else
                    {
                        sb.Append("  "); // Пустые места для выравнивания в конце файла
                    }

                    if (i < 15) sb.Append(" ");
                }
                return sb.ToString();
            }
        }

        public string AsciiString => GetAsciiString(Bytes, ValidBytesCount);

        private static string GetAsciiString(byte[] bytes, int validCount)
        {
            char[] chars = new char[16];
            for (int i = 0; i < 16; i++)
            {
                if (i < validCount)
                {
                    // Отображаем только печатные символы
                    chars[i] = (bytes[i] >= 32 && bytes[i] <= 126) ? (char)bytes[i] : '.';
                }
                else
                {
                    chars[i] = ' '; // Заполняем пустотой конец файла
                }
            }
            return new string(chars);
        }
    }


}
