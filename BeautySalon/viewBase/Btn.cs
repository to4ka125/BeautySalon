using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeautySalon.viewBase
{
    public class Btn : RadioButton
    {
        /// <summary>
        /// Класс наследуется от RadioButton → становится переключателем с возможностью выбора одного варианта из группы
        /// </summary>
        static Btn()
        {
            // Ключевая строка для кастомизации внешнего вида контрола
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Btn),// Указание типа, для которого переопределяются метаданные
                new FrameworkPropertyMetadata(typeof(Btn)));// Создание новых метаданных
        }
    }
}
