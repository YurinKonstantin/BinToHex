using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace BinViewer
{
    public static class RateAppManager
    {
        private const string KeySaveCount = "User_SuccessSavesCount";
        private const string KeyHasRated = "User_HasRatedApp";

        // Проверяет, пришел ли идеальный момент для показа диалога
        public static bool ShouldRequestRating()
        {
            var settings = ApplicationData.Current.LocalSettings.Values;

            // Если пользователь уже оценивал приложение или нажимал "Больше не показывать", не беспокоим его
            if (settings.ContainsKey(KeyHasRated) && (bool)settings[KeyHasRated])
                return false;

            // Считаем количество успешных сохранений файлов
            int saveCount = settings.ContainsKey(KeySaveCount) ? (int)settings[KeySaveCount] : 0;
            saveCount++;
            settings[KeySaveCount] = saveCount;

            // Идеальный триггер: пользователь успешно сохранил уже 3-й файл (значит, активно пользуется программой)
            return saveCount >= 3;
        }

        public static void MarkAsRated()
        {
            ApplicationData.Current.LocalSettings.Values[KeyHasRated] = true;
        }
    }
}
