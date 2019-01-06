using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageScanner_New
{
    /// <summary>
    /// Получаем список изображений в директории, которые дублируют друг друга
    /// </summary>
    class ImageScanner
    {
        /// <summary>
        /// Размер наибольшей стороны превьюшки
        /// </summary>
        private const int prewiewSize = 64;
        /// <summary>
        /// Процент, при котором считаем, что файлы совпадают
        /// </summary>
        private const decimal eqalPercent = 95;
        /// <summary>
        /// Значение, на которое может разнится цвет пикселя
        /// </summary>
        private const int colorDiscrepancy = 3;
        
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

            try
            {
                //Сканируем все директории, на предмет наличия картинок, 
                //Для каждой из найденных формируем превьюшку, записываем
                //её хеш, и путь к исходной картинке
                scanDirectory(new DirectoryInfo(path), files);
                //Выбираем дубликаты
                ex = findDuplicates(files);
            }
            catch { ex = new List<List<FileScanInfo>>(); }

            //Возвращаем результат
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
                    //Говорим, что для этого файла дубли пока что не найдены
                    find = false;
                    //Проходимся по уже найденным файлам
                    for (int i = 0; i < ex.Count; i++)
                    {
                        //Сравниваем хеши с первым файлом.
                        //Если они похожи, то
                        if (eqalHash(ex[i].First().hash, file.hash))
                        {
                            //Добавляем этот файл в список дублей
                            ex[i].Add(file);
                            //Указываем, что дубли есть
                            find = true;
                            //Выходим из цикла
                            break;
                        }
                    }

                    //Если дублей не было
                    if (!find)
                        //Добавляем этот файл в общий список
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
        /// Сравниваем хеши файлов
        /// </summary>
        /// <param name="hash1">Хеш первого файла</param>
        /// <param name="hash2">Хеш второго файла</param>
        /// <returns>True - файлы похожи</returns>
        private bool eqalHash(byte[] hash1, byte[] hash2)
        {
            bool ex = false;
            decimal perc;
            int countEqal = 0;

            try
            {
                //На данный момент, сравниваем только файлы с одинаковыми 
                //размерами формата высота*ширина
                if (hash1.Length == hash2.Length)
                {
                    //Проходимся по хешу
                    for(int i = 0; i < hash1.Length; i++)
                    {
                        //Если расхождение в значении цветов меньше чем константа
                        if (Math.Abs(hash1[i] - hash2[i]) <= colorDiscrepancy)
                            //Добавляем поинт
                            countEqal++;
                    }

                    //Получаем процент соответствия файлов по этим хешам
                    perc = (countEqal * 100) / hash1.Length;
                    //Сравниваем полученный процент с эталонным
                    ex = (perc >= eqalPercent);
                }
            }
            catch { ex = false; }

            return ex;
        }


        /// <summary>
        /// Сканируем директорию и все её поддиректории на наличие картинок
        /// </summary>
        /// <param name="di">Информация о директории</param>
        /// <param name="files">Список, хранящий информацию о перебранных файлах</param>
        private void scanDirectory(DirectoryInfo di, List<FileScanInfo> files)
        {
            byte[] hash;
            int size;
            //Проходимся по всем файлам директории
            foreach (var file in di.GetFiles())
            {
                //Если расширение файла корректное
                if (testFileExt(file.Extension))
                {
                    //Получаем байты хеша
                    hash = getMiniFileBytes(file.FullName, out size);

                    //Добавляем информацию о файле в общий список
                    files.Add(new FileScanInfo()
                    {
                        path = file.FullName,
                        hash = hash,
                        tag = new ImageTagInfo() {
                            fileNameLength = file.Name.Length,
                            path = file.FullName,
                            size = size
                        }
                    });
                }
            }

            //Проходимся по всем дочерним директориям
            foreach (var dir in di.GetDirectories())
                //Начинаем сканирование директории и всех её дочерних элементов
                scanDirectory(dir, files);
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
        /// Получаем байты превьюшки картинки
        /// </summary>
        /// <param name="path">Путь к файлу картинки</param>
        /// <param name="size">Размер файла</param>
        /// <returns>Байты превьюшки</returns>
        private byte[] getMiniFileBytes(string path, out int size)
        {
            byte[] ex = null;
            //Ставим дефолтное значение
            size = 0;

            try
            {
                //Открываем исходную картинку
                using (Bitmap pic = new Bitmap(path))
                {
                    //Просчитываем размер картинки
                    size = pic.Width * pic.Height;
                    //Получаем актуальные размеры для превьюшки
                    getPrewiewSizes(pic, out int width, out int height);
                    //Создаём превьюшку
                    using(Bitmap prewiew = new Bitmap(pic, width, height))
                    {
                        //Получаем байты цветов превьюшки, в оттенках серого
                        ex = decolorizeBitmap(prewiew);
                    }
                }
            }
            catch
            {
                //Ставим дефолтное значение
                size = 0;
                ex = null;
            }

            return ex;
        }



        /// <summary>
        /// Убираем цвета у картинки
        /// </summary>
        /// <param name="pic">Картинка</param>
        /// <returns>Байты цветов пикселей</returns>
        private byte[] decolorizeBitmap(Bitmap pic)
        {
            List<byte> ex = new List<byte>();
            try
            {
                //Цвет пикселя
                Color pix;
                //Цвет в палитре серого
                byte gray;

                //Проходимся по всем пикселям картинки
                for(int i = 0; i < pic.Width; i++)
                    for(int j = 0; j < pic.Height; j++)
                    {
                        //Получаем цвет пикселя
                        pix = pic.GetPixel(i, j);
                        //Получаем цвет в палитре серого по хитрой формуле
                        gray = (byte)(pix.R * 0.3 + pix.G * 0.59 + pix.B * 0.11);
                        //Добавляем цвет в список
                        ex.Add(gray);
                    }
            }
            catch { ex = new List<byte>(); }

            return ex.ToArray();
        }


        /// <summary>
        /// Получаем размеры превьюшки
        /// </summary>
        /// <param name="pic">Исходное изображение</param>
        /// <param name="width">Ширина превьюшки</param>
        /// <param name="height">Высота превьюшки</param>
        private void getPrewiewSizes(Bitmap pic, out int width, out int height)
        {
            //Если картинка квадратнаяя
             if (pic.Height == pic.Width)
                //И размеры квадратные тоже
                width = height = prewiewSize;
            //Если ширина больше высоты
            else if (pic.Width > pic.Height)
            {
                //Ширина = константе
                width = prewiewSize;
                //Высоту считаем по 
                height = (prewiewSize * pic.Height) / pic.Width;
            }
            //Если высота больше ширины
            else
            {
                //Высота = константе
                height = prewiewSize;
                //Ширину считаем по 
                width = (prewiewSize * pic.Width) / pic.Height;
            }
        }

    }
}
