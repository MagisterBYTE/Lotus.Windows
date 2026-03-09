# План рефакторинга Lotus.Windows

## Цель
Привести проект Lotus.Windows к современным стандартам WPF: централизованные стили, MVVM-подход, убрать логику из code-behind.

## Правила выполнения
- Любое действие выполнять только после подтверждения
- При анализе C# руководствоваться правилами: `D:\CODE\LotusPlatform\Lotus.Basis\Lotus.Core\.claude\rules\BaseRulesAndQuality.md`
- Работы выполнять строго последовательно по шагам (см. ниже)

## Порядок работы для каждого компонента
1. Анализ компонента
2. Анализ зависимостей
3. Рефакторинг
4. Размещение компонента в `Lotus.Windows.App/MainWindow.xaml` для тестирования
5. Ожидание подтверждения перед переходом к следующему компоненту

## Задачи

### 1. Стили (Themes/Styles)
- Привести все стили к единым требованиям
- Каждый стиль компонента — в отдельном файле в папке `Themes/Styles/`
- Имена стилей по шаблону: начинаются с `Lotus`, заканчиваются на `StyleKey`
- Пример: `LotusButtonCommandIconStyleKey`

### 2. Рефакторинг WPF-компонентов
- Убрать логику из code-behind
- Убрать ручное управление событиями
- Убрать константы из XAML (вынести в ресурсы/стили)
- При необходимости корректировать базовую часть кода (например, `Lotus.Core`)

### 3. Тестовое приложение (Lotus.Windows.App)
- Для каждого компонента (или группы компонентов) создать отдельный `TabItem` в `MainWindow.xaml`
- Компонент размещать с разными значениями свойств для полноценного тестирования
- Тестирование выполняет пользователь

## Прогресс

| Компонент | Стиль | Рефакторинг | Тест |
|-----------|-------|-------------|------|
| **Editors** — `LotusTextBox` | `LotusTextBoxEditor.xaml` | ✅ | ✅ |
| **Editors** — `LotusNumericEditor` | (встроен в стиль) | ✅ | ✅ |
| **Editors** — `LotusMeasurementEditor` | (встроен в стиль) | ✅ | ✅ |
| **Editors** — `LotusVector2DEditor` | (встроен в стиль) | ✅ | ✅ |
| **Editors** — `LotusVector3DEditor` | (встроен в стиль) | ✅ | ✅ |
| **Common** — `LotusButtonCommandIcon` | `LotusButtonCommandIcon.xaml` | ✅ | ✅ |
| **Common** — `LotusMenuItemIcon` | `LotusMenuItemIcon.xaml` | ✅ (фикс: IconSource→MiddleIcon) | ✅ |
| **Special** — `LotusColorPicker` | `LotusColorPicker.xaml` | ✅ | ✅ |
| **Special** — `LotusPixelRulerControl` | — (OnRender) | ✅ | ✅ |
| **Special** — `LotusLoggerControl` | `LotusLoggerControl.xaml` | ✅ (убран INotifyPropertyChanged) | ✅ |
| **Special** — `LotusWindowLongTaskInformer` | — (Window) | ✅ (x:FieldModifier → DependencyProperty) | ✅ |
| **Special** — `LotusGrowlNotification` | `LotusGrowlNotification.xaml` | ✅ (вынесены ресурсы, переименованы поля) | ✅ |
| **Data** — `LotusDataGrid` + фильтры | `Default/LotusDataGrid.xaml` | ✅ (QueryBase, ColumnQueryItem, DataTemplate-фильтры, цвета в ресурсы) | ✅ |
| **Data** — `LotusTreeView` | `Default/LotusTreeView.xaml` | ✅ (ресурсы проекта, DefaultStyleKeyProperty, AddHandler, TemplateSelector) | ✅ |

## Реорганизация Themes/Styles
Папки: `Default/` (12 стилей), `Editors/`, `Common/`, `Special/` — ✅

## Итог рефакторинга LotusDataGrid

**Что сделано:**
- Удалён `LotusDataGrid.xaml` (компонентный XAML), класс переведён на `DefaultStyleKeyProperty.OverrideMetadata`
- Добавлено свойство `SelectedObjects` (`ObservableCollection<object>`) — синхронизируется через `SelectionChanged`, корректно при любой сортировке и фильтрации
- Добавлен attached DP `ColumnFilterType` на `DataGridColumn` — тип определяется автоматически в `OnAutoGeneratingColumn`
- `LotusColumnFilterControl` полностью переписан: `FilterType`/`PropertyName` DPs, всплывающее событие `FilterChangedEvent`, делегирование в 4 типа фильтров (String/Number/DateTime/Enum)
- Фильтрация работает через `CollectionViewSource.GetDefaultView(ItemsSource).Filter` — in-memory, не зависит от источника данных
- Стиль `LotusDataGridStyleKey` добавлен `BasedOn="{StaticResource {x:Type DataGrid}}"` — наследует системный шаблон DataGrid со всеми PART_*
- Неявный стиль `LotusDataGrid` в `Themes/Styles/Default/LotusDataGrid.xaml` добавляет `ColumnHeaderStyle` с фильтр-строкой в заголовках