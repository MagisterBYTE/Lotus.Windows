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
    /// Поддерживаются опции поиска представление типом <see cref="TStringSearchOption"/> и стандартная операция LIKE.
    /// </remarks>
    public class CQueryItemString : CQueryItem
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsSearchOption = new(nameof(SearchOption));
        private static readonly PropertyChangedEventArgs PropertyArgsSearchValue = new(nameof(SearchValue));
        #endregion

        #region Fields
        protected internal TStringSearchOption _searchOption;
        protected internal string _searchValue;
        #endregion

        #region Properties
        /// <summary>
        /// Опции поиска в строке.
        /// </summary>
        public TStringSearchOption SearchOption
        {
            get
            {
                return _searchOption;
            }
            set
            {
                if (_searchOption != value)
                {
                    _searchOption = value;
                    OnPropertyChanged(PropertyArgsSearchOption);
                    OnPropertyChanged(PropertyArgsSQLQueryItem);
                    QueryOwned?.OnNotifyUpdated(this, nameof(SearchOption));
                }
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
        public CQueryItemString()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="searchOption">Опции поиска в строке.</param>
        /// <param name="searchValue">Значение для сравнения.</param>
        public CQueryItemString(TStringSearchOption searchOption, string searchValue)
        {
            _searchOption = searchOption;
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
        /// Формирование SQL запроса.
        /// </summary>
        /// <param name="sqlQuery">SQL запрос.</param>
        /// <returns>Статус формирования элемента запроса.</returns>
        public override bool ComputeSQLQuery(ref string sqlQuery)
        {
            if ((_notCalculation == false) && (string.IsNullOrEmpty(_searchValue) == false))
            {
                switch (_searchOption)
                {
                    case TStringSearchOption.Start:
                        {
                            sqlQuery += " " + _propertyName + " LIKE '" + _searchValue + "%'";
                        }
                        break;
                    case TStringSearchOption.End:
                        {
                            sqlQuery += " " + _propertyName + " LIKE '%" + _searchValue + "'";
                        }
                        break;
                    case TStringSearchOption.Contains:
                        {
                            sqlQuery += " " + _propertyName + " LIKE '%" + _searchValue + "%'";
                        }
                        break;
                    case TStringSearchOption.Equal:
                        break;
                    default:
                        break;
                }

                return true;
            }

            return false;
        }
        #endregion

        #region Binding methods
#if USE_WINDOWS
        /// <summary>
        /// Привязка текстового поля к строке поиска.
        /// </summary>
        /// <param name="textBox">Текстовое поле.</param>
        public void BindingTextBoxToSearchValue(in System.Windows.Controls.TextBox textBox)
        {
            if (textBox != null)
            {
                var binding = new System.Windows.Data.Binding
                {
                    Source = this,
                    Path = new System.Windows.PropertyPath(path: nameof(SearchValue))
                };

                System.Windows.Data.BindingOperations.SetBinding(textBox,
                    System.Windows.Controls.TextBox.TextProperty, binding);
            }
        }

        /// <summary>
        /// Привязка выпадающего списка к опциям поиска.
        /// </summary>
        /// <param name="comboBox">Выпадающий список.</param>
        public void BindingComboBoxToSearchOption(in System.Windows.Controls.ComboBox comboBox)
        {
            if (comboBox != null)
            {
                var binding = new System.Windows.Data.Binding
                {
                    Source = this,
                    Path = new System.Windows.PropertyPath(path: nameof(SearchOption)),
                    Converter = EnumToStringConverter.Instance
                };

                comboBox.ItemsSource = XEnumHelper.GetDescriptions(typeof(TStringSearchOption));
                System.Windows.Data.BindingOperations.SetBinding(comboBox,
                    System.Windows.Controls.ComboBox.SelectedValueProperty, binding);
            }
        }
#endif
        #endregion
    }
    /**@}*/
}