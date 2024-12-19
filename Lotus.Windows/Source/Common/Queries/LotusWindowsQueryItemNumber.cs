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
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonOperator = new(nameof(ComparisonOperator));
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueLeft = new(nameof(ComparisonValueLeft));
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueRight = new(nameof(ComparisonValueRight));
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
                    QueryOwned?.OnNotifyUpdated(this, nameof(ComparisonOperator));
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
                    QueryOwned?.OnNotifyUpdated(this, nameof(ComparisonValueLeft));
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
                    QueryOwned?.OnNotifyUpdated(this, nameof(_comparisonValueRight));
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
        /// <param name="comparisonOperator">Оператор сравнения.</param>
        /// <param name="comparisonValue">Значение для сравнения.</param>
        public CQueryItemNumber(TComparisonOperator comparisonOperator, double comparisonValue)
        {
            _comparisonOperator = comparisonOperator;
            _comparisonValueLeft = comparisonValue;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="comparisonValueLeft">Значение для сравнения слева.</param>
        /// <param name="comparisonValueRight">Значение для сравнения справа.</param>
        public CQueryItemNumber(double comparisonValueLeft, double comparisonValueRight)
        {
            _comparisonOperator = TComparisonOperator.Equality;
            _comparisonValueLeft = comparisonValueLeft;
            _comparisonValueRight = comparisonValueRight;
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
        /// <param name="sqlQuery">SQL запрос.</param>
        /// <returns>Статус формирования элемента запроса.</returns>
        public override bool ComputeSQLQuery(ref string sqlQuery)
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
        /// <param name="comboBox">Выпадающий список.</param>
        public void BindingComboBoxToComparisonOperator(in System.Windows.Controls.ComboBox comboBox)
        {
            if (comboBox != null)
            {
                var binding = new System.Windows.Data.Binding
                {
                    Source = this,
                    Path = new System.Windows.PropertyPath(path: nameof(ComparisonOperator)),
                    Converter = EnumToStringConverter.Instance
                };

                comboBox.ItemsSource = XEnumHelper.GetDescriptions(typeof(TComparisonOperator));
                System.Windows.Data.BindingOperations.SetBinding(comboBox,
                    System.Windows.Controls.ComboBox.SelectedValueProperty, binding);
            }
        }
#endif
        #endregion
    }
    /**@}*/
}