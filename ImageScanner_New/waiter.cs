using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageScanner_New
{
    /// <summary>
    /// Форма ожидания завершения процесса сканирвоания
    /// </summary>
    public partial class waiter : Form
    {

        /// <summary>
        /// Конструктор формы
        /// </summary>
        public waiter()
        {
            //Инициализатор формы
            InitializeComponent();
        }


        /// <summary>
        /// Проставляем значеняи на форме
        /// </summary>
        /// <param name="maxScan">Максмальное значение прогрессбара сканирования</param>
        /// <param name="valScan">Текущее значение прогрессбара сканирования</param>
        /// <param name="maxVisual">Максмальное значение прогрессбара отображения</param>
        /// <param name="valVisual">Текущее значение прогрессбара отображения</param>
        public void setValues(int maxScan, int valScan, int maxVisual, int valVisual)
        {
            //Проставляем значения, если они есть
            if(maxScan != -1)
                scanProgress.Maximum = maxScan;
            if (valScan != -1)
                scanProgress.Value = valScan;
            if (maxVisual != -1)
                visualizeProgress.Maximum = maxVisual;
            if (valVisual != -1)
                visualizeProgress.Value = valVisual;
        }
    }
}
