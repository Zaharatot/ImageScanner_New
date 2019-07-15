using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScanner_New
{
    /// <summary>
    /// Информация о файле для сканирования
    /// </summary>
    class FileScanInfo
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// Хеш превьюшки файла
        /// </summary>
        public byte[] hash { get; set; }
        /// <summary>
        /// Информация о картинке, для помещения в тег
        /// </summary>
        public ImageTagInfo tag { get; set; }
    }
}
