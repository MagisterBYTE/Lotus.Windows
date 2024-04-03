using System;
using System.ComponentModel;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsCommonQueries
	*@{*/
    /// <summary>
    /// Класс представляющий элемент запроса для числовых значений.
    /// </summary>
    /// <remarks>
    /// Поддерживаются стандартные операторы сравнения и стандартная операция BETWEEN.
    /// </remarks>
    public class CQueryItemNumber : CQueryItem
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonOperator = new PropertyChangedEventArgs(nameof(ComparisonOperator));
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueLeft = new PropertyChangedEventArgs(nameof(ComparisonValueLeft));
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueRight = new PropertyChangedEventArgs(nameof(ComparisonValueRight));
        #endregion

        #region Fields
        protected internal TComparisonOperator _comparisonOperator;
        protected internal double _comparisonValueLeft;
        protected internal double _comparisonValueRight;
        #endregion

        #region Properties
        /// <summary>
        /// Оператор сравнения.
        /// </summary>
        public TComparisonOperator ComparisonOperator
        {
            get
            {
                return _comparisonOperator;
            }
            set
            {
                if (_comparisonOperator != value)
                {
                    _comparisonOperator = value;
                    OnPropertyChanged(PropertyArgsComparisonOperator);
                    OnPropertyChanged(PropertyArgsSQLQueryItem);
                    if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(ComparisonOperator));
                }
            }
        }

        /// <summary>
        /// Значение для сравнения слева.
        /// </summary>
        public double ComparisonValueLeft
        {
            get
            {
                return _comparisonValueLeft;
            }
            set
            {
                if (Math.Abs(_comparisonValueLeft - value) > 0.000001)
                {
                    _comparisonValueLeft = value;
                    OnPropertyChanged(PropertyArgsComparisonValueLeft);
                    OnPropertyChanged(PropertyArgsSQLQueryItem);
                    if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(ComparisonValueLeft));
                }
            }
        }

        /// <summary>
        /// Значение для сравнения справа.
        /// </summary>
        public double ComparisonValueRight
        {
            get
            {
                return _comparisonValueRight;
            }
            set
            {
                if (Math.Abs(_comparisonValueRight - value) > 0.000001)
                {
                    _comparisonValueRight = value;
                    OnPropertyChanged(PropertyArgsComparisonValueRight);
                    OnPropertyChanged(PropertyArgsSQLQueryItem);
                    if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(_comparisonValueRight));
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CQueryItemNumber()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="comparison_operator">Оператор сравнения.</param>
        /// <param name="comparison_value">Значение для сравнения.</param>
        public CQueryItemNumber(TComparisonOperator comparison_operator, double comparison_value)
        {
            _comparisonOperator = comparison_operator;
            _comparisonValueLeft = comparison_value;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="comparison_value_left">Значение для сравнения слева.</param>
        /// <param name="comparison_value_right">Значение для сравнения справа.</param>
        public CQueryItemNumber(double comparison_value_left, double comparison_value_right)
        {
            _comparisonOperator = TComparisonOperator.Equality;
            _comparisonValueLeft = comparison_value_left;
            _comparisonValueRight = comparison_value_right;
        }
        #endregion

        #region System methods
        /// <summary>
        /// Преобразование к текстовому представлению.
        /// </summary>
        /// <returns>Наименование объекта.</returns>
        public override string ToString()
        {
            var name = "";
            ComputeSQLQuery(ref name);
            return name;
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Формирование SQL запроса.
        /// </summary>
        /// <param name="sql_query">SQL запрос.</param>
        /// <returns>Статус формирования элемента запроса.</returns>
        public override bool ComputeSQLQuery(ref string sql_query)
        {
            if (_notCalculation == false && double.IsInfinity(_comparisonValueLeft) == false &&
                double.IsNaN(_comparisonValueLeft) == false)
            {
                //if(_comparisonOperator == TComparisonQueryOperator.Between)
                //{
                //	if(_comparisonValueRight > _comparisonValueLeft)
                //	{
                //		sql_query += " " + _propertyName + " BETWEEN " + _comparisonValueLeft.ToString()
                //			+ " AND " + _comparisonValueRight.ToString();
                //		return (true);
                //	}
                //}
                //else
                //{
                //	sql_query += " " + _propertyName + _comparisonOperator.GetOperatorOfString() + _comparisonValueLeft.ToString();
                //	return (true);
                //}
            }

            return false;
        }
        #endregion

        #region Binding methods
#if USE_WINDOWS
        /// <summary>
        /// Привязка выпадающего списка к оператору сравнения.
        /// </summary>
        /// <param name="combo_box">Выпадающий список.</param>
        public void BindingComboBoxToComparisonOperator(in System.Windows.Controls.ComboBox combo_box)
        {
            if (combo_box != null)
            {
                var binding = new System.Windows.Data.Binding();
                binding.Source = this;
                binding.Path = new System.Windows.PropertyPath(path: nameof(ComparisonOperator));
                binding.Converter = EnumToStringConverter.Instance;

                combo_box.ItemsSource = XEnum.GetDescriptions(typeof(TComparisonOperator));
                System.Windows.Data.BindingOperations.SetBinding(combo_box,
                    System.Windows.Controls.ComboBox.SelectedValueProperty, binding);
            }
        }
#endif
        #endregion
    }
    /**@}*/
}