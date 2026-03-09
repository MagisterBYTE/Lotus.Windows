using System;
using System.ComponentModel;
using System.Numerics;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsCommonQueries
	*@{*/
    /// <summary>
    /// Класс представляющий элемент запроса для значений даты-времени.
    /// </summary>
    /// <remarks>
    /// Поддерживаются стандартные операторы сравнения и стандартная операция BETWEEN.
    /// </remarks>
    public class QueryItemDateTime : QueryItem
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
        protected internal DateTime _comparisonValueLeft;
        protected internal DateTime _comparisonValueRight;
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
                return QueryItemDateTime.FilterFunctionsStatic;
            }
        }

        /// <summary>
        /// Значение для сравнения слева (или для сравнения по равенству).
        /// </summary>
        public DateTime ComparisonValueLeft
        {
            get
            {
                return _comparisonValueLeft;
            }
            set
            {
                if (ComparisonValueLeft != value)
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
        public DateTime ComparisonValueRight
        {
            get
            {
                return _comparisonValueRight;
            }
            set
            {
                if (_comparisonValueRight != value)
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
        public QueryItemDateTime()
        {
            _comparisonValueLeft = DateTime.MinValue;
            _comparisonValueRight = DateTime.MinValue;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="filterFunction">Функция фильтрации.</param>
        /// <param name="comparisonValue">Значение для сравнения.</param>
        public QueryItemDateTime(TFilterFunction filterFunction, DateTime comparisonValue)
        {
            _filterFunction = filterFunction;
            _comparisonValueLeft = comparisonValue;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="comparisonValueLeft">Значение для сравнения слева.</param>
        /// <param name="comparisonValueRight">Значение для сравнения справа.</param>
        public QueryItemDateTime(DateTime comparisonValueLeft, DateTime comparisonValueRight)
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
            if(item is null) return false;

            // Извлекаем свойство
            var valueRaw = XReflection.GetPropertyValue(item, PropertyName);

            if (valueRaw is null) return false;

            var value = XDateTimeConverter.ToDateTime(valueRaw, DateTime.MinValue);
            if(value == DateTime.MinValue) return false;

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
            if (_notCalculation == false)
            {
                if (_filterFunction == TFilterFunction.Equals)
                {
                    if (_comparisonValueRight > _comparisonValueLeft)
                    {
                        sqlQuery += " " + _propertyName + " BETWEEN " + _comparisonValueLeft.ToString()
                            + " AND " + _comparisonValueRight.ToString();
                        return true;
                    }
                }
                else
                {
                    //sqlQuery += " " + _propertyName + _filterFunction.GetOperatorOfString() + _comparisonValueLeft.ToString();
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
    /**@}*/
}