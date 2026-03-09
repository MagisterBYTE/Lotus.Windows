using System;
using System.ComponentModel;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsCommonQueries
	*@{*/
    /// <summary>
    /// Класс представляющий элемент запроса для строковых значений.
    /// </summary>
    /// <remarks>
    /// Поддерживаются стандартные операторы для строк и стандартная операция LIKE.
    /// </remarks>
    public class QueryItemString : QueryItem
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsFilterFunction = new(nameof(FilterFunction));
        private static readonly PropertyChangedEventArgs PropertyArgsSearchValue = new(nameof(SearchValue));
        private static readonly TFilterFunction[] FilterFunctionsStatic =
        [
            TFilterFunction.Equals,
            TFilterFunction.NotEqual,
            TFilterFunction.Contains,
            TFilterFunction.StartsWith,
            TFilterFunction.EndsWith,
            TFilterFunction.Like
        ];
        #endregion

        #region Fields
        protected internal TFilterFunction _filterFunction;
        protected internal string _searchValue;
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
                return QueryItemString.FilterFunctionsStatic;
            }
        }

        /// <summary>
        /// Значение для сравнения.
        /// </summary>
        public string SearchValue
        {
            get
            {
                return _searchValue;
            }
            set
            {
                if (_searchValue != value)
                {
                    _searchValue = value;
                    OnPropertyChanged(PropertyArgsSearchValue);
                    OnPropertyChanged(PropertyArgsSQLQueryItem);
                    QueryOwned?.OnNotifyUpdated(this, nameof(SearchValue));
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public QueryItemString()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="filterFunction">Функция фильтрации.</param>
        /// <param name="searchValue">Значение для сравнения.</param>
        public QueryItemString(TFilterFunction filterFunction, string searchValue)
        {
            _filterFunction = filterFunction;
            _searchValue = searchValue;
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

            var value = valueRaw.ToString();

            if (value is null) return false;

            var status = false;
            switch (_filterFunction)
            {
                case TFilterFunction.Equals:
                    status = _searchValue == value;
                    break;
                case TFilterFunction.NotEqual:
                    status = _searchValue != value;
                    break;
                case TFilterFunction.LessThan:
                    break;
                case TFilterFunction.LessThanOrEqual:
                    break;
                case TFilterFunction.GreaterThan:
                    break;
                case TFilterFunction.GreaterThanOrEqual:
                    break;
                case TFilterFunction.Between:
                    break;
                case TFilterFunction.Contains:
                    status = value.Contains(_searchValue, StringComparison.OrdinalIgnoreCase);
                    break;
                case TFilterFunction.StartsWith:
                    status = value.StartsWith(_searchValue, StringComparison.OrdinalIgnoreCase);
                    break;
                case TFilterFunction.EndsWith:
                    status = value.EndsWith(_searchValue, StringComparison.OrdinalIgnoreCase);
                    break;
                case TFilterFunction.Like:
                    status = value.Contains(_searchValue, StringComparison.OrdinalIgnoreCase);
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
            if ((_notCalculation == false) && (string.IsNullOrEmpty(_searchValue) == false))
            {
                switch (_filterFunction)
                {
                    case TFilterFunction.StartsWith:
                        {
                            sqlQuery += " " + _propertyName + " LIKE '" + _searchValue + "%'";
                        }
                        break;
                    case TFilterFunction.EndsWith:
                        {
                            sqlQuery += " " + _propertyName + " LIKE '%" + _searchValue + "'";
                        }
                        break;
                    case TFilterFunction.Contains:
                        {
                            sqlQuery += " " + _propertyName + " LIKE '%" + _searchValue + "%'";
                        }
                        break;
                    case TFilterFunction.Equals:
                        break;
                    default:
                        break;
                }

                return true;
            }

            return false;
        }
        #endregion
    }
    /**@}*/
}