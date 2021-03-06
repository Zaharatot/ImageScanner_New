﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// Id картинки
        /// </summary>
        public string imageId { get; set; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// Хеш файла
        /// </summary>
        public byte[] hash { get; set; }
        /// <summary>
        /// Информация о картинке, для помещения в тег
        /// </summary>
        public ImageTagInfo tag { get; set; }

        /// <summary>
        /// Превьюшка картинки
        /// </summary>
        public Bitmap preview { get; set; }
        /// <summary>
        /// Размер изначальной картинки
        /// </summary>
        public string imageSize { get; set; }


    }
}
