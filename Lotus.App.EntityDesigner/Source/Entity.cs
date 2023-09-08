//=====================================================================================================================
using System;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace EntityDesigner
	{
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип реализации поля на бэке
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TEntityFieldImplement
		{
			/// <summary>
			/// Автосвойство
			/// </summary>
			AutoProperty,

			/// <summary>
			/// Поле-свойство
			/// </summary>
			FieldProperty
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Поле сущности
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class EntityField : PropertyChangedBase
		{
			#region ======================================= ДАННЫЕ ====================================================
			private TEntityFieldImplement _fieldImplement;
			private String _fieldType;
			private String _fieldname;
			private String _name;
			private String _desc;
			private Boolean _isCollection;
			private Boolean _isKey;
			private Boolean _isEnum;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Тип реализации поля на бэке
			/// </summary>
			public TEntityFieldImplement FieldImplement
			{
				get { return _fieldImplement; }
				set 
				{ 
					_fieldImplement = value;
					NotifyPropertyChanged();
				}
			}

			/// <summary>
			/// Тип поля
			/// </summary>
			public String FieldType
			{
				get { return _fieldType; }
				set
				{
					_fieldType = value;
					NotifyPropertyChanged();
				}
			}

			/// <summary>
			/// Названия поля на бэке
			/// </summary>
			public String FieldName
			{
				get { return _fieldname; }
				set
				{
					_fieldname = value;
					NotifyPropertyChanged();
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Основной элемент для управления масштабированием и перемещением контента в области просмотра
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class Entity
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства.
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства.
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================