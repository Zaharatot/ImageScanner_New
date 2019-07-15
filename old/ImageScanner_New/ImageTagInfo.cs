using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScanner_New
{
    /// <summary>
    /// Информация о картинке, которая будет храниться в теге
    /// </summary>
    class ImageTagInfo
    {
        /// <summary>
        /// Размеры картинки
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// Полынй путь к файлу картинки
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// Длинна имени файла
        /// </summary>
        public int fileNameLength { get; set; }
    }
}
