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
    public class QueryItemNumber : QueryItem
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsFilterFunction = new(nameof(FilterFunction));
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueLeft = new(nameof(ComparisonValueLeft));
        private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueRight = new(nameof(ComparisonValueRight));
        private static readonly TFilterFunction[] FilterFunctionsStatic =
        [
            TFilterFunction.Equals,
            TFilterFunction.NotEqual,
            TFilterFunction.LessThan,
            TFilterFunction.LessThanOrEqual,
            TFilterFunction.GreaterThan,
            TFilterFunction.GreaterThanOrEqual,
            TFilterFunction.Between
        ];
        #endregion

        #region Fields
        protected internal TFilterFunction _filterFunction;
        protected internal double _comparisonValueLeft;
        protected internal double _comparisonValueRight;
        #endregion

        #region Properties
        /// <summary>
        /// Функция фильтрации.
        /// </summary>
        public TFilterFunction FilterFunction
        {
            get
            {
                return _filterFunction;
            }
            set
            {
                if (_filterFunction != value)
                {
                    _filterFunction = value;
                    OnPropertyChanged(PropertyArgsFilterFunction);
                    OnPropertyChanged(PropertyArgsSQLQueryItem);
                    QueryOwned?.OnNotifyUpdated(this, nameof(FilterFunction));
                }
            }
        }

        /// <summary>
        /// Набор доступных функций фильтрации для DateTime
        /// </summary>
        public TFilterFunction[] FilterFunctions
        {
            get
            {
                return QueryItemNumber.FilterFunctionsStatic;
            }
        }

        /// <summary>
        /// Значение для сравнения слева (или для сравнения по равенству).
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
        public QueryItemNumber()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="filterFunction">Функция фильтрации.</param>
        /// <param name="comparisonValue">Значение для сравнения.</param>
        public QueryItemNumber(TFilterFunction filterFunction, double comparisonValue)
        {
            _filterFunction = filterFunction;
            _comparisonValueLeft = comparisonValue;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="comparisonValueLeft">Значение для сравнения слева.</param>
        /// <param name="comparisonValueRight">Значение для сравнения справа.</param>
        public QueryItemNumber(double comparisonValueLeft, double comparisonValueRight)
        {
            _filterFunction = TFilterFunction.Equals;
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
        /// Проверяет, соответствует ли объект текущему условию фильтрации.
        /// </summary>
        /// <param name="item">Проверяемый объект.</param>
        /// <returns>Статус проверки.</returns>
        public override bool MatchesFilter(object? item)
        {
            if (item is null) return false;

            // Извлекаем свойство
            var valueRaw = XReflection.GetPropertyValue(item, PropertyName);

            if (valueRaw is null) return false;

            var value = XNumberConverter.ToDouble(valueRaw, Double.MinValue);
            if (value == Double.MinValue) return false;

            var status = false;
            switch (_filterFunction)
            {
                case TFilterFunction.Equals:
                    status = _comparisonValueLeft == value;
                    break;
                case TFilterFunction.NotEqual:
                    status = _comparisonValueLeft != value;
                    break;
                case TFilterFunction.LessThan:
                    status = _comparisonValueLeft < value;
                    break;
                case TFilterFunction.LessThanOrEqual:
                    status = _comparisonValueLeft <= value;
                    break;
                case TFilterFunction.GreaterThan:
                    status = _comparisonValueLeft > value;
                    break;
                case TFilterFunction.GreaterThanOrEqual:
                    status = _comparisonValueLeft >= value;
                    break;
                case TFilterFunction.Between:
                    status = (_comparisonValueLeft <= value && _comparisonValueRight >= value);
                    break;
                case TFilterFunction.Contains:
                    break;
                case TFilterFunction.StartsWith:
                    break;
                case TFilterFunction.EndsWith:
                    break;
                case TFilterFunction.Like:
                    break;
                case TFilterFunction.NotEmpty:
                    break;
                case TFilterFunction.Empty:
                    break;
                case TFilterFunction.IncludeAny:
                    break;
                case TFilterFunction.IncludeAll:
                    break;
                case TFilterFunction.IncludeEquals:
                    break;
                case TFilterFunction.IncludeNone:
                    break;
                default:
                    break;
            }

            return status;
        }

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
                //if(_filterFunction == TComparisonQueryOperator.Between)
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
                //	sql_query += " " + _propertyName + _filterFunction.GetOperatorOfString() + _comparisonValueLeft.ToString();
                //	return (true);
                //}
            }

            return false;
        }
        #endregion
    }
    /**@}*/
}