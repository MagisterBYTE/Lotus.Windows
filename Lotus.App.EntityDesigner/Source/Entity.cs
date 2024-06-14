//=====================================================================================================================
using System;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.CompilerServices;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace EntityDesigner
	{
		
		/// <summary>
		/// Тип реализации поля на бэке
		/// </summary>
		
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

		
		/// <summary>
		/// Поле сущности
		/// </summary>
		
		public class EntityField : PropertyChangedBase
		{
			#region ======================================= ДАННЫЕ ====================================================
			private TEntityFieldImplement _fieldImplement;
			private string _fieldType;
			private string _fieldname;
			private string _name;
			private string _desc;
			private bool _isCollection;
			private bool _isKey;
			private bool _isEnum;
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
					OnPropertyChanged();
				}
			}

			/// <summary>
			/// Тип поля
			/// </summary>
			public string FieldType
			{
				get { return _fieldType; }
				set
				{
					_fieldType = value;
					OnPropertyChanged();
				}
			}

			/// <summary>
			/// Названия поля на бэке
			/// </summary>
			public string FieldName
			{
				get { return _fieldname; }
				set
				{
					_fieldname = value;
					OnPropertyChanged();
				}
			}
			#endregion
		}

		
		/// <summary>
		/// Основной элемент для управления масштабированием и перемещением контента в области просмотра
		/// </summary>
		
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

			#region Main methods
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
			public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
		
		/**@}*/
		
	}
}
//=====================================================================================================================