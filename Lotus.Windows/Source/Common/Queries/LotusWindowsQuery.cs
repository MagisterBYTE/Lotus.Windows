using System;
using System.ComponentModel;
using System.Reflection;

using Lotus.Core;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsCommonQueries Подсистема запросов данных
     * \ingroup WindowsCommon
     * \brief Подсистема запросов данных.
     * @{
     */
    /// <summary>
    /// Функции для фильтрации данных.
    /// </summary>
    public enum TFilterFunction
    {
        /// <summary>
        /// Равно аргументу.
        /// </summary>
        Equals = 0,

        /// <summary>
        /// Не равно аргументу.
        /// </summary>
        NotEqual = 1,

        /// <summary>
        /// Меньше аргумента.
        /// </summary>
        LessThan = 2,

        /// <summary>
        /// Меньше или равно аргумента.
        /// </summary>
        LessThanOrEqual = 3,

        /// <summary>
        /// Больше аргумента.
        /// </summary>
        GreaterThan = 4,

        /// <summary>
        /// Больше или равно аргумента.
        /// </summary>
        GreaterThanOrEqual = 5,

        /// <summary>
        /// Между первым аргументом (меньшим) и вторым аргументом (большим).
        /// </summary>
        Between = 6,

        /// <summary>
        /// Аргумент(строка) может находиться в любом месте с учётом регистра.
        /// Аргумент(иной) значение должно присутствовать в аргументе массива.
        /// </summary>
        Contains = 7,

        /// <summary>
        /// Аргумент(строка) должна находиться в начале с учётом регистра.
        /// </summary>
        StartsWith = 8,

        /// <summary>
        /// Аргумент(строка) должна находиться в конце с учётом регистра.
        /// </summary>
        EndsWith = 9,

        /// <summary>
        /// Аргумент(строка) должна сравниваться с учётом оператора Like.
        /// </summary>
        Like = 10,

        /// <summary>
        /// Не равно пустой или NULL строке. Аргумент НЕ требуется.
        /// Не равно значению NULL для иных объектов.
        /// </summary>
        NotEmpty = 11,

        /// <summary>
        /// Равно пустой или NULL строке. Аргумент НЕ требуется.
        /// Равно значению NULL для иных объектов.
        /// </summary>
        Empty = 12,

        /// <summary>
        /// Любой из проверяемых элементов списка должен находиться в массиве аргумента.
        /// </summary>
        /// <remarks>
        /// filter [1, 2]
        /// item01 [1,2,3] -> true
        /// item02 [4,2,3] -> true
        /// item03 [2,3]   -> true
        /// item04 [1,2]   -> true
        /// item05 [4,5]   -> false
        /// </remarks>
        IncludeAny = 13,

        /// <summary>
        /// Все из проверяемых элементов списка должен находиться в массиве аргумента.
        /// </summary>
        /// <remarks>
        /// filter [1, 2]
        /// item01 [1,2,3] -> true
        /// item02 [4,2,3] -> false
        /// item03 [2,3]   -> false
        /// item04 [1,2]   -> true
        /// item05 [4,5]   -> false
        /// </remarks>
        IncludeAll = 14,

        /// <summary>
        /// Проверяемые элементы списка должен быть равны массиву аргумента.
        /// </summary>
        /// <remarks>
        /// filter [1, 2]
        /// item01 [1,2,3] -> false
        /// item02 [4,2,3] -> false
        /// item03 [2,3]   -> false
        /// item04 [1,2]   -> true
        /// item05 [4,5]   -> false
        /// </remarks>
        IncludeEquals = 15,

        /// <summary>
        /// Ни один из проверяемых элементов списка не должен находиться в массиве аргумента.
        /// </summary>
        /// <remarks>
        /// filter [1, 2]
        /// item01 [1,2,3] -> false
        /// item02 [4,2,3] -> false
        /// item03 [2,3]   -> false
        /// item04 [1,2]   -> false
        /// item05 [4,5]   -> true
        /// </remarks>
        IncludeNone = 16,
    }
    
    /// <summary>
    /// Класс представляющий запрос.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Запрос представляет собой набор определённых условий по которым извлекаются данные или фильтруются для отображения
    /// </para>
    /// <para>
    /// Он может применяться как к списку с объектам конкретных типов так для работы с сырыми данным
    /// </para>
    /// <para>
    /// Запрос при этом представлен в виде стандартного SQL запроса и предиката
    /// </para>
    /// </remarks>
    public class QueryBase : PropertyChangedBase
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsSQLQuery = new(nameof(SQLQuery));
        #endregion

        #region Fields
        protected internal ListArray<QueryItem> _items;
        protected internal string _sqlQuery;
        #endregion

        #region Properties
        /// <summary>
        /// Элементы запроса.
        /// </summary>
        public ListArray<QueryItem> Items
        {
            get
            {
                return _items;
            }
        }

        /// <summary>
        /// Стандартный SQL запрос.
        /// </summary>
        public string SQLQuery
        {
            get
            {
                ComputeSQLQuery();
                return _sqlQuery;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public QueryBase()
        {
            _items = [];
        }
        #endregion

        #region ILotusNotify methods
        /// <summary>
        /// Информирование данного объекта о начале изменения данных указанного объекта.
        /// </summary>
        /// <param name="source">Объект данные которого будут меняться.</param>
        /// <param name="dataName">Имя данных.</param>
        /// <returns>Статус разрешения/согласования изменения данных.</returns>
        public bool OnNotifyUpdating(object source, string dataName)
        {
            return true;
        }

        /// <summary>
        /// Информирование данного объекта об окончании изменении данных указанного объекта.
        /// </summary>
        /// <param name="source">Объект данные которого изменились.</param>
        /// <param name="dataName">Имя данных.</param>
        public void OnNotifyUpdated(object source, string dataName)
        {
            OnPropertyChanged(PropertyArgsSQLQuery);
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Проверяет, соответствует ли объект текущему условию фильтрации.
        /// </summary>
        /// <param name="item">Проверяемый объект.</param>
        /// <returns>Статус проверки.</returns>
        public bool MatchesFilter(object? item)
        {
            if (item is null) return true;

            foreach (var queryItem in Items)
            {
                if (queryItem == null || string.IsNullOrEmpty(queryItem.PropertyName) || queryItem.NotCalculation) return true;
                var status = queryItem.MatchesFilter(item);
                if(status) return false;
            }

            return true;
        }

        /// <summary>
        /// Вычисление SQL запроса на основе элементов запроса.
        /// </summary>
        public void ComputeSQLQuery()
        {
            var sql_query = "";

            for (var i = 0; i < _items.Count; i++)
            {
                if (_items[i].ComputeSQLQuery(ref sql_query))
                {
                    if (i < _items.Count - 1)
                    {
#pragma warning disable S1643 // Strings should not be concatenated using '+' in a loop
                        sql_query += " AND";
#pragma warning restore S1643 // Strings should not be concatenated using '+' in a loop
                    }
                }
            }

            _sqlQuery = sql_query;
        }
        #endregion
    }

    /// <summary>
    /// Класс представляющий элемент запроса.
    /// </summary>
    public abstract class QueryItem : PropertyChangedBase, ILotusNotCalculation
    {
        #region Static fields
        public static readonly PropertyChangedEventArgs PropertyArgsSQLQueryItem = new(nameof(SQLQueryItem));
        public static readonly PropertyChangedEventArgs PropertyArgsNotCalculation = new(nameof(NotCalculation));
        #endregion

        #region Fields
        protected internal string _propertyName;
        protected internal QueryBase _queryOwned;
        protected internal bool _notCalculation;
        #endregion

        #region Properties
        /// <summary>
        /// Имя свойства/столбца.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        /// <summary>
        /// Запрос.
        /// </summary>
        public QueryBase QueryOwned
        {
            get
            {
                return _queryOwned;
            }
            set
            {
                _queryOwned = value;
            }
        }

        /// <summary>
        /// Элемент стандартного SQL запроса.
        /// </summary>
        public string SQLQueryItem
        {
            get
            {
                return ToString()!;
            }
        }

        /// <summary>
        /// Элемент запроса не участвует в запросе.
        /// </summary>
        public bool NotCalculation
        {
            get { return _notCalculation; }
            set
            {
                _notCalculation = value;
                OnPropertyChanged(PropertyArgsNotCalculation);
                QueryOwned?.OnNotifyUpdated(this, nameof(NotCalculation));
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public QueryItem()
        {
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Проверяет, соответствует ли объект текущему условию фильтрации.
        /// </summary>
        /// <param name="item">Проверяемый объект.</param>
        /// <returns>Статус проверки.</returns>
        public abstract bool MatchesFilter(object? item);

        /// <summary>
        /// Формирование SQL запроса.
        /// </summary>
        /// <param name="sqlQuery">SQL запрос.</param>
        /// <returns>Статус формирования элемента запроса.</returns>
        public virtual bool ComputeSQLQuery(ref string sqlQuery)
        {
            return false;
        }
        #endregion
    }
    /**@}*/
}