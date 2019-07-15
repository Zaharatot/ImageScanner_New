using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace ImageScanner_New
{
    /// <summary>
    /// Получаем список изображений в директории, которые дублируют друг друга
    /// </summary>
    class ImageScanner
    {
        /// <summary>
        /// Делегат события обновления процесса сканирования
        /// </summary>
        /// <param name="maxScan">Максимальное значение для сканирования</param>
        /// <param name="valScan">Текущее значение сканирования</param>
        public delegate void updateScan(int maxScan, int valScan);
        /// <summary>
        /// Событие обновления процесса сканирования
        /// </summary>
        public event updateScan onUpdateScan;

        /// <summary>
        /// Размер иконки изображения
        /// </summary>
        private const int sizeIcon = 16;

        /// <summary>
        /// Чувствительность поиска
        /// </summary>
        private const int searchSensivity = 5;

        /// <summary>
        /// Максимальное количество потоков
        /// </summary>
        private const int maxCountTasks = 8;

        /// <summary>
        /// Размер превьюшки изображения
        /// </summary>
        private const int previewSize = 128;

        /// <summary>
        /// Массив расширений файлов, которые мы проверяем
        /// </summary>
        private string[] fileExtensions;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public ImageScanner()
        {
            //Инициализируем расширения
            fileExtensions = new string[] {
                ".jpg", ".jpeg", ".png", ".gif", ".bmp"
            };
        }

        /// <summary>
        /// Производим поиск файлов дубликатов в указанной папке и её подпапках
        /// </summary>
        /// <param name="path">Путь к корневой папке поиска</param>
        /// <returns>Список файлов - дубликатов</returns>
        public List<List<FileScanInfo>> findDuplicates(string path)
        {
            //Список списков дублей
            List<List<FileScanInfo>> ex = new List<List<FileScanInfo>>();
            //Список, хранящий информацию о перебранных файлах
            List<FileScanInfo> files = new List<FileScanInfo>();

            List<string> filePaths = new List<string>();

            try
            {
                //Сканируем все директории, на предмет наличия картинок
                scanDirectory(new DirectoryInfo(path), filePaths);
                //Для каждой из найденных формируем превьюшку, записываем
                //её хеш, и путь к исходной картинке
                files = createFileInfos(filePaths);
                //Выбираем дубликаты
                ex = findDuplicates(files);
            }
            catch { ex = new List<List<FileScanInfo>>(); }
            
            return ex;
        }

        /// <summary>
        /// Формируем информацию о файлах
        /// </summary>
        /// <param name="filePaths">Список путей к файлам</param>
        /// <returns>Список информации о файлах</returns>
        private List<FileScanInfo> createFileInfos(List<string> filePaths)
        {
            List<FileScanInfo> ex = new List<FileScanInfo>();

            try
            {
                //Выполняем параллельно
                Parallel.For(0, filePaths.Count, (i) => {

                    //Считаем инфу о картинке, и возвращаем в выходной список
                    ex.Add(createImageInfo(filePaths[i], i.ToString()));
                    //Вызываем ивент с инфой
                    onUpdateScan?.Invoke(filePaths.Count, ex.Count);
                });
               
            }
            catch { ex = new List<FileScanInfo>(); }

            return ex;
        }


        /// <summary>
        /// Формируем инфу о файле, включающую его хеш
        /// </summary>
        /// <param name="id">id файла в списке</param>
        /// <param name="filePath">Путь к файлу</param>
        private FileScanInfo createImageInfo(string filePath, string id)
        {
            FileScanInfo ex = new FileScanInfo();
            byte[] hash;
            int size;

            try
            {
                //Грузим картинку
                using (Bitmap pic = new Bitmap(filePath))
                {
                    //Получаем байты хеша
                    hash = getImageHash(pic, out size);

                    //Добавляем информацию о файле в общий список
                    ex = new FileScanInfo()
                    {
                        path = filePath,
                        hash = hash,
                        imageId = id,
                        imageSize = $"({pic.Width} X {pic.Height})",
                        preview = new Bitmap(pic, new Size(previewSize, previewSize)),
                        tag = new ImageTagInfo()
                        {
                            fileNameLength = filePath.Length,
                            path = filePath,
                            size = size
                        }
                    };
                }
            }
            catch { ex = new FileScanInfo(); }

            return ex;
        }

        /// <summary>
        /// Выполняем поиск дубликатов в полученном списке
        /// </summary>
        /// <param name="files">Список файлов</param>
        /// <returns>Словарь списков дубликатов</returns>
        private List<List<FileScanInfo>> findDuplicates(List<FileScanInfo> files)
        {
            //Список списков дублей
            List<List<FileScanInfo>> ex = new List<List<FileScanInfo>>();
            //Флаг нахождения дубля
            bool find;

            try
            {
                //Проходимся по списку файлов
                foreach (var file in files)
                {
                    //Стафим флаг ненайденности
                    find = false;
                    //Проходимся по выходному массиву
                    for(int i = 0; i < ex.Count; i++)
                    {
                        //Если мы нашли копию в данном списке
                        if(checkDuplicates(ex[i], file.hash))
                        {
                            //Добавляем копию в список
                            ex[i].Add(file);
                            //Ставим флаг нахождения копии
                            find = true;
                            //Выходим из цикла
                            break;
                        }
                    }

                    //Если копии файла не были найдены
                    if (!find)
                        //Добавляем их в выходной список, как новое изображение
                        ex.Add(new List<FileScanInfo>() { file });
                }

            }
            catch { ex = new List<List<FileScanInfo>>(); }

            //Удаляем из списка все варианты с 1 найденым файлом (без дублей)
            ex.RemoveAll(du => (du.Count == 1));

            //Возвращаем результат
            return ex;
        }

        /// <summary>
        /// Ищем дубликаты хеша в выходном массиве
        /// </summary>
        /// <param name="files">Список схожих файлов</param>
        /// <param name="hash">Хеш файла</param>
        /// <returns>True - файлы похожи</returns>
        private bool checkDuplicates(List<FileScanInfo> files, byte[] hash) =>
            //Если в массиве нет файлов совпадающих по хешу
            (files.Count(fl => eqalImageHash(fl.hash, hash)) != 0);



        /// <summary>
        /// Сканируем директорию и все её поддиректории на наличие картинок
        /// </summary>
        /// <param name="di">Информация о директории</param>
        /// <param name="filePaths">Список путей к файлам</param>
        private void scanDirectory(DirectoryInfo di, List<string> filePaths)
        {
            //Проходимся по всем файлам директории
            foreach (var file in di.GetFiles())
                //Если расширение файла корректное
                if (testFileExt(file.Extension))
                    //Добавляем полный путь к файлу в общий список
                    filePaths.Add(file.FullName);                
            
            //Проходимся по всем дочерним директориям
            foreach (var dir in di.GetDirectories())
                //Начинаем сканирование директории и всех её дочерних элементов
                scanDirectory(dir, filePaths);
        }


        /// <summary>
        /// Проверяем расширение файла на соответствие доступным
        /// </summary>
        /// <param name="ext">Расширение файла</param>
        /// <returns>True - подходит</returns>
        private bool testFileExt(string ext) =>
            //Проверяем, чтобы расширение в нижнем регистре было в списке допустимых
            fileExtensions.Contains(ext.ToLower());



        /// <summary>
        /// Сравниваем хеши изображений
        /// </summary>
        /// <param name="a">Хеш первой картинки</param>
        /// <param name="b">Хеш второй картинки</param>
        /// <returns>True - картинки схожы</returns>
        private bool eqalImageHash(byte[] a, byte[] b)
        {
            int ex = 0;

            try
            {
                //Проходимся по байтам
                for (int i = 31; i >= 0; i--)
                    //Проходимся по битам числа
                    for (int j = 0; j < 8; j++)
                        //Получаем биты и сравниваем
                        if ((b[i] >> j & 1) != (a[i] >> j & 1))
                            //Если биты не равны - увеличиваем выход
                            ex++;
            }
            catch { ex = 999; }

            //В зависимости от разности хешей 
            //возвращаем результат проверки
            return (ex < searchSensivity);
        }

        /// <summary>
        /// Генерируем хеш изображения из его байт
        /// </summary>
        /// <param name="pic">Загруженная картинка</param>
        /// <param name="size">Размер изображения в байтах</param>
        /// <returns>Байты хеша</returns>
        public byte[] getImageHash(Bitmap pic, out int size)
        {
            byte[] ex = null;
            byte[] pixels;
            //Ставим дефолтное значение
            size = -1;

            try
            {
                //Получаем массив пикселов изображения
                pixels = getImagePixels(new Bitmap(pic, new Size(sizeIcon, sizeIcon)));                
                //Получаем байты изображения в монохромном формате
                byte[] grayscale = toGrayScale(pixels);
                //Находим среднее значение цвета
                byte average = getAverage(grayscale);
                //Получаем хеш картинки
                ex = getHash(grayscale, average);
                //Возвращаем размер картинки в байтах
                size = pixels.Length;
            }
            catch
            {
                ex = null;
                //Ставим дефолтное значение
                size = -1;
            }

            return ex;
        }

        /// <summary>
        /// Получаем наш хеш в виде числа
        /// </summary>
        /// <param name="grayscale">Массив оттенков серого</param>
        /// <param name="average">Среднее значение цвета</param>
        /// <returns>Значение хеша</returns>
        private byte[] getHash(byte[] grayScale, byte average)
        {
            byte[] ex = new byte[32];

            try
            {
                //id последнего цвета
                int last = grayScale.Length - 1;


                //Проходимся по цветам
                for (int i = 0; i < 32; i++)
                {
                    //Обнуляем байт
                    ex[i] = 0;
                    //Проходимся по битам
                    for (int j = 0; j < 8; j++)
                    {
                        //Прибавляем 1 бит, в зависимости от того
                        //больше среднего цвет, или нет
                        ex[i] += (byte)((grayScale[i * j] > average) ? 1 : 0);
                        //Если мы не дошли до последнего бита
                        if (i < last)
                            //Сдвигаем биты картинки
                            ex[i] = (byte)(ex[i] << 1);
                    }
                }
            }
            catch { ex = null; }

            return ex;
        }

        /// <summary>
        /// Находим среднее значение цвета
        /// </summary>
        /// <param name="grayscale">Массив оттенков серого</param>
        /// <returns>Среднее значение цвета</returns>
        private byte getAverage(byte[] grayscale)
        {
            int ex = 0;

            //Складываем все значения цветов
            for (int i = 0; i < grayscale.Length; i++)
                ex += grayscale[i];

            //Делим на их количество
            ex /= grayscale.Length;

            return (byte)ex;
        }


        /// <summary>
        /// Получаем пиксели изображения в виде одномерного массива
        /// </summary>
        /// <param name="img">Исходное изображение</param>
        /// <returns>Массив цветов</returns>
        private byte[] getImagePixels(Bitmap img)
        {
            //Выходной массив
            byte[] ex = null;


            try
            {
                //Получаем размер считываемого массива
                int size = img.Width * img.Height * 4;

                //Инициализируем выходной массив
                ex = new byte[size];

                //Лочим биты картинки
                var bd = img.LockBits(
                    new Rectangle(0, 0, img.Width, img.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                //Считываем пиксели изображения
                Marshal.Copy(bd.Scan0, ex, 0, size);

                //Разблокируем пиксели изображения
                img.UnlockBits(bd);
            }
            catch { ex = null; }

            return ex;
        }


        /// <summary>
        /// Переводим картинку в режим градаций серого
        /// </summary>
        /// <param name="pixels">Массив цветов</param>
        /// <returns>Массив цветов пикселов изображения</returns>
        public byte[] toGrayScale(byte[] pixels)
        {
            //Выходной массив
            byte[] ex = null;
            //Получаем размер итогового массива
            int size = pixels.Length / 4;
            //Счётчик для выходного массива
            int j = 0;

            try
            {
                //Инициализируем выходной массив
                ex = new byte[size];

                //Проходимся по массиву пикселов
                for (int i = 0; i < pixels.Length; i += 4)
                    //Переводим пиксель в режим градаций серого и втыкаем на место
                    ex[j++] = colorToGrayScale(pixels[i + 1], pixels[i + 2], pixels[i + 3]);

                /*
                 Мы сознательно не учитываем альфа-канал.
                 В данном случае - он нахрен не нужен.
                 */
            }
            catch { ex = null; }

            return ex;
        }

        /// <summary>
        /// Возвращаем цвет, переведённый в градации серого
        /// </summary>
        /// <param name="r">Компонента красного цвета в изображении</param>
        /// <param name="g">Компонента зелёного цвета в изображении</param>
        /// <param name="b">Компонента синего цвета в изображении</param>
        /// <returns>Яркость пикселя в градациях серого</returns>
        private byte colorToGrayScale(int r, int g, int b)
        {
            //Получаем цвет в градациях серого
            int gray = (int)(0.299 * r + 0.587 * g + 0.114 * b);
            //Возвращаем результат перевода цвета в градации серого
            return (byte)gray;
        }
    }
}
