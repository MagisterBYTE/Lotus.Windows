using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsCommonQueries
	*@{*/
    /// <summary>
    /// Класс представляющий элемент запроса для перечисляемых значений.
    /// </summary>
    public class QueryItemEnum : QueryItem
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsFilterFunction = new(nameof(FilterFunction));
        private static readonly PropertyChangedEventArgs PropertyArgsSourceItems = new(nameof(SourceItems));
        private static readonly PropertyChangedEventArgs PropertyArgsFiltredItems = new(nameof(FilteredItems));
        private static readonly TFilterFunction[] FilterFunctionsStatic =
        [
            TFilterFunction.IncludeAny,
            TFilterFunction.IncludeAll,
            TFilterFunction.IncludeEquals,
            TFilterFunction.IncludeNone
        ];
        #endregion

        #region Fields
        protected internal List<object> _sourceItems;
        protected internal List<object> _filteredItems;
        protected internal TFilterFunction _filterFunction = TFilterFunction.IncludeAny;
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
        /// Набор доступных функций фильтрации для Enum
        /// </summary>
        public TFilterFunction[] FilterFunctions
        {
            get
            {
                return QueryItemEnum.FilterFunctionsStatic;
            }
        }

        /// <summary>
        /// Коллекция которая является источником данных.
        /// </summary>
        public List<object> SourceItems
        {
            get
            {
                return _sourceItems;
            }
            set
            {
                _sourceItems = value;
                OnPropertyChanged(PropertyArgsSourceItems);
            }
        }

        /// <summary>
        /// Список элементов которые выбраны.
        /// </summary>
        public List<object> FilteredItems
        {
            get
            {
                return _filteredItems;
            }
            set
            {
                _filteredItems = value;
                OnPropertyChanged(PropertyArgsFiltredItems);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public QueryItemEnum()
        {
            _filteredItems = [];
            _sourceItems = [];
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="enumType">Тип перечисления.</param>
        public QueryItemEnum(Type enumType)
        {
            _filteredItems = [];
            _sourceItems = [.. XEnumHelper.GetDescriptions(enumType)];
        }
        #endregion

        #region System methods
        /// <summary>
        /// Преобразование к текстовому представлению.
        /// </summary>
        /// <returns>Наименование объекта.</returns>
        public override string ToString()
        {
            return JoinFilteredItems();
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
            if (_filteredItems.Count == 0) return true;

            var valueRaw = XReflection.GetPropertyValue(item, PropertyName);
            if (valueRaw is null) return false;

            return _filterFunction switch
            {
                TFilterFunction.IncludeAny => _filteredItems.Contains(valueRaw),
                TFilterFunction.IncludeNone => !_filteredItems.Contains(valueRaw),
                TFilterFunction.IncludeAll => _filteredItems.All(f => Equals(f, valueRaw)),
                TFilterFunction.IncludeEquals => _filteredItems.Count == 1 && Equals(_filteredItems[0], valueRaw),
                _ => true
            };
        }

        /// <summary>
        /// Уведомить об изменении набора выбранных элементов фильтра.
        /// </summary>
        public void NotifyFilteredItemsChanged()
        {
            OnPropertyChanged(PropertyArgsSQLQueryItem);
            QueryOwned?.OnNotifyUpdated(this, nameof(FilteredItems));
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
                if (_filteredItems.Count > 0)
                {
                    var included = new StringBuilder(_filteredItems.Count * 10);
                    for (var i = 0; i < _filteredItems.Count; i++)
                    {
                        if (i != 0)
                        {
                            included.Append(", ");
                        }

                        included.Append("'" + _filteredItems[i].ToString() + "'");
                    }

                    sqlQuery += " " + _propertyName + " IN (" + included.ToString() + ")";
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Соединение выбранных элементов одну строку.
        /// </summary>
        /// <returns>Строка с выбранными элементами.</returns>
        public string JoinFilteredItems()
        {
            if (_filteredItems.Count > 0)
            {
                var included = new StringBuilder(_filteredItems.Count * 10);
                for (var i = 0; i < _filteredItems.Count; i++)
                {
                    if (i != 0)
                    {
                        included.Append(", ");
                    }

                    included.Append(_filteredItems[i].ToString());
                }

                return included.ToString();
            }

            return string.Empty;
        }
        #endregion
    }
    /**@}*/
}