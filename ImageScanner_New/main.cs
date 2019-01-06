using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace ImageScanner_New
{
    public partial class main : Form
    {
        /// <summary>
        /// Класс сканера дублей
        /// </summary>
        private ImageScanner ims;

        public main()
        {
            InitializeComponent();
            //Инициализируем переменные
            init();
        }

        /// <summary>
        /// Инициализатор класса
        /// </summary>
        private void init()
        {
            //Инициализируем класс поиска дублей
            ims = new ImageScanner();
        }

        /// <summary>
        /// Кнопка запуска сканирования
        /// </summary>
        private void scanButton_Click(object sender, EventArgs e)
        {
            //Если папка для сканирования есть
            if (Directory.Exists(scanPathTextBox.Text))
            {

                new Thread(() => { 
                    //Получаем список дубликатов
                    var duList = ims.findDuplicates(scanPathTextBox.Text);
                    //Выполняем это в UI потоке, т.к. работа с контроллами
                    this.BeginInvoke(new Action(() => {
                        //Отображаем найденные дубли
                        visualizeDoublicates(duList);
                    }));
                }).Start();
            }
        }

        /// <summary>
        /// Отображаем на экране найденные дубли
        /// </summary>
        /// <param name="duList">Список дублей</param>
        private void visualizeDoublicates(List<List<FileScanInfo>> duList)
        {
            int imageId = 0;
            string sImageId;
            //длинна исходного пути
            int defaultPathLength = scanPathTextBox.Text.Length;
            Bitmap pic;
            ListViewGroup group;


            //Очищаем списки перед заполнением
            duplicatesList.Groups.Clear();
            ImagesList.Images.Clear();
            duplicatesList.Items.Clear();

            //Проходимся по списку
            foreach (var du in duList)
            {
                //Инициализируем группу
                group = new ListViewGroup("");
                //Добавляем группу
                duplicatesList.Groups.Add(group);

                //Проходимся по путям, с похожими картинками
                foreach (var file in du)
                {
                    //Получаем id картинки в виде строки
                    sImageId = imageId.ToString();
                    //Получаем картинку
                    pic = new Bitmap(file.path);

                    //Добавляем картинку в коллекцию
                    ImagesList.Images.Add(sImageId, pic);

                    //Добавляем элемент в коллекцию
                    duplicatesList.Items.Add(new ListViewItem(
                        file.path.Substring(defaultPathLength) + $"\r\n ({pic.Width} X {pic.Height})",
                        sImageId,
                        group)
                    {
                        //В тег пишем инфу о теге
                        Tag = file.tag
                    });

                    //Увеличиваем номер картинки
                    imageId++;

                    //Закрываем картинку
                    pic.Dispose();
                }
            }

            //Автоматическое выделение всех кроме первого элементов в группах
            autoSelect();

            MessageBox.Show("Сканирование на дубликаты успешно завершено.");
        }

        /// <summary>
        /// Автоматическое выделение всех кроме первого элементов в группах
        /// </summary>
        private void autoSelect()
        {
            int selId, max;
            ImageTagInfo tag;

            //Проходимся по всем группам
            foreach (ListViewGroup gr in duplicatesList.Groups)
            {
                //Проставляем значения
                selId = max = 0;
                //Проходимся по списку элементов группы и выделяем их все
                for (int i = 0; i < gr.Items.Count; i++)
                {
                    //Проставляем элементу флажок удаления
                    gr.Items[i].Checked = true;
                    //Ставим белый цвет заднему фону
                    gr.Items[i].BackColor = Color.White;
                    //Получаем тег из картинки
                    tag = (ImageTagInfo)gr.Items[i].Tag;

                    //Если эта картинка больше прошлой
                    if(tag.size > max)
                    {
                        //Запоминаем её как самую большую
                        max = tag.size;
                        selId = i;
                    }
                }

                //Убираем выделение на самой большой картинке
                gr.Items[selId].Checked = false;
            }
        }

        /// <summary>
        /// Кнопка удаления выбранных файлов
        /// </summary>
        private void removeButton_Click(object sender, EventArgs e)
        {
            //Список под удаление
            List<string> removeList = new List<string>();
            ImageTagInfo tag;
            bool start;
            //Получаем количество блоков без выделенных элементов
            int unselectBlocks = testOneDeSelect();

            //Проверяем на наличие полностью выбранных под удаление блоков блоков
            start = ((unselectBlocks == 0) || (MessageBox.Show(
                $"У вас есть {unselectBlocks} блоков, в которых нет ни одного выбранного элемента. Вы действительно хотите продолжить удаление?",
                "Запрос подтверждения удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            ) == DialogResult.Yes));


            //Если всё-таки удаление пойдёт
            if (start)
            {
                //Проходимся по списку элементов
                for (int i = 0; i < duplicatesList.Items.Count; i++)
                {
                    //Если стоит галочка
                    if (duplicatesList.Items[i].Checked)
                    {
                        //Получаем тег из картинки
                        tag = (ImageTagInfo)duplicatesList.Items[i].Tag;
                        //Путь к файлу в список на удаление
                        removeList.Add(tag.path);
                    }
                }

                //Если был выбран хоть 1 элемент
                if (removeList.Count > 0)
                {
                    //Запрос подтверждения на удаление
                    if (MessageBox.Show(
                            $"Вы действительно хотите удалить {removeList.Count} элементов?",
                            "Запрос удаления",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        //Проходимся по списку удаления
                        foreach (var path in removeList)
                            //Удаляем файл
                            File.Delete(path);

                        MessageBox.Show("Удаление файлов успешно завершено.");
                    }
                }
            }
        }

        /// <summary>
        /// Проверка того, чтобы в каждой группе хотя бы 
        /// один элемент не был выделен
        /// </summary>
        /// <returns>Количество блоков, в которых выбраны все элементы</returns>
        private int testOneDeSelect()
        {
            int ex = 0;
            bool check;

            //Проходимся по всем группам
            foreach (ListViewGroup gr in duplicatesList.Groups)
            {
                //Проставляем дефолтное значение
                check = false;
                //Проходимся по списку элементов группы и выделяем их все
                for (int i = 0; !check && (i < gr.Items.Count); i++)
                {
                    //Если флаг не стоит
                    if(!gr.Items[i].Checked)
                        //Говорим, что всё ок
                        check = true;
                }

                //Если элемент не был выделен
                if (!check)
                {
                    //Проходимся по списку элементов группы и выделяем их все
                    for (int i = 0; i < gr.Items.Count; i++)
                        //Выделяем ошибочное значение
                        gr.Items[i].BackColor = Color.Red;

                    //Говорим, что есть косяки
                    ex++;
                }
            }

            return ex;
        }
    }
}
