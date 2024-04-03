//=====================================================================================================================
// Проект: Модуль для отображения 3D контента
// Раздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusViewerContent3D.xaml.cs
*		Элемент для просмотра и редактирования файлов формата 3D контента.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Object3D;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
//---------------------------------------------------------------------------------------------------------------------
using Helix = HelixToolkit.Wpf.SharpDX;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsViewerContent3DControls
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент для просмотра и редактирования файлов формата 3D контента
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusViewerContent3D : UserControl, ILotusViewerContentFile, IDisposable, INotifyPropertyChanged
		{
			#region ======================================= КОНСТАНТНЫЕ ДАННЫЕ ========================================
			/// <summary>
			/// Команда для увеличения выбранного объема. Аргумент - выбранный объём типа <see cref="Rect3D"/> 
			/// </summary>
			public const string COMMAND_ZOOM_EXTENTS = "ZoomExtents";

			/// <summary>
			/// Имя ортографической камеры
			/// </summary>
			public const string OrthographicCameraName = "Orthographic Camera";

			/// <summary>
			/// Имя перспективной камеры
			/// </summary>
			public const string PerspectiveCameraName = "Perspective Camera";
			#endregion

			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static readonly PropertyChangedEventArgs PropertyArgsShowWireframe = new PropertyChangedEventArgs(nameof(ShowWireframe));
			protected static readonly PropertyChangedEventArgs PropertyArgsRenderFlat = new PropertyChangedEventArgs(nameof(RenderFlat));
			protected static readonly PropertyChangedEventArgs PropertyArgsRenderEnvironmentMap = new PropertyChangedEventArgs(nameof(PropertyArgsRenderEnvironmentMap));

			protected static readonly PropertyChangedEventArgs PropertyArgsCamera = new PropertyChangedEventArgs(nameof(Camera));
			protected static readonly PropertyChangedEventArgs PropertyArgsCameraModel = new PropertyChangedEventArgs(nameof(CameraModel));

			protected static readonly PropertyChangedEventArgs PropertyArgsEffectsManager = new PropertyChangedEventArgs(nameof(EffectsManager));

			protected static readonly PropertyChangedEventArgs PropertyArgsScene = new PropertyChangedEventArgs(nameof(Scene));
			protected static readonly PropertyChangedEventArgs PropertyArgsSceneRoot = new PropertyChangedEventArgs(nameof(SceneRoot));
			protected static readonly PropertyChangedEventArgs PropertyArgsGroupModel = new PropertyChangedEventArgs(nameof(GroupModel));
			protected static readonly PropertyChangedEventArgs PropertyArgsSelectedModel = new PropertyChangedEventArgs(nameof(SelectedModel));

			protected static readonly PropertyChangedEventArgs PropertyArgsEnableAnimation = new PropertyChangedEventArgs(nameof(EnableAnimation));
			protected static readonly PropertyChangedEventArgs PropertyArgsSelectedAnimation = new PropertyChangedEventArgs(nameof(SelectedAnimation));
			protected static readonly PropertyChangedEventArgs PropertyArgsSpeedAnimation = new PropertyChangedEventArgs(nameof(SpeedAnimation));

			protected static readonly PropertyChangedEventArgs PropertyArgsGridGeometry = new PropertyChangedEventArgs(nameof(GridGeometry));
			protected static readonly PropertyChangedEventArgs PropertyArgsGridColor = new PropertyChangedEventArgs(nameof(GridColor));
			protected static readonly PropertyChangedEventArgs PropertyArgsGridTransform = new PropertyChangedEventArgs(nameof(GridTransform));

			private static string OpenFileFilter = $"{HelixToolkit.SharpDX.Core.Assimp.Importer.SupportedFormatsString}";
			private static string ExportFileFilter = $"{HelixToolkit.SharpDX.Core.Assimp.Exporter.SupportedFormatsString}";
			#endregion

			#region ======================================= СТАТИЧЕСКИЕ МЕТОДЫ ========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на поддерживаемый формат файла
			/// </summary>
			/// <param name="extension">Расширение файла</param>
			/// <returns>Статус поддержки</returns>
			//---------------------------------------------------------------------------------------------------------
			public static bool IsSupportFormatFile(string extension)
			{
				return OpenFileFilter.Contains(extension);
			}
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Имя файла
			/// </summary>
			public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName),
				typeof(string),
				typeof(LotusViewerContent3D),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Параметры визуализации
			protected internal bool _showWireframe = false;
			protected internal bool _renderFlat = false;
			protected internal bool _renderEnvironmentMap = true;
			protected internal TextureModel _environmentMap;

			// Камера
			protected internal string? _cameraModel;
			protected internal Helix.Camera _camera;
			protected internal Helix.OrthographicCamera _defaultOrthographicCamera;
			protected internal Helix.PerspectiveCamera _defaultPerspectiveCamera;

			// Рендер техника
			protected internal IEffectsManager _effectsManager;

			// Базовые модели
			protected internal Scene3D _scene;
			protected internal SceneNode _sceneRoot;
			protected internal Helix.SceneNodeGroupModel3D _groupModel;
			protected internal Helix.GeometryModel3D _selectedModel;

			// Анимация
			protected internal bool _enableAnimation = false;
			protected internal IList<Animation> _sceneAnimations;
			protected internal ObservableCollection<IAnimationUpdater> _animationsUpdater;
			protected internal IAnimationUpdater? _selectedAnimationUpdater = null;
			protected internal IAnimationUpdater? _animationUpdater;
			protected internal float _speedAnimation = 1.0f;

			// Параметры скелета
			protected internal List<BoneSkinMeshNode> _boneSkinNodes = new List<BoneSkinMeshNode>();
			protected internal List<BoneSkinMeshNode> _skeletonNodes = new List<BoneSkinMeshNode>();
			protected internal CompositionTargetEx _compositeHelper = new CompositionTargetEx();

			protected internal bool _isLoading = false;

			// Опорная сетка
			protected internal LineGeometry3D _gridGeometry;
			protected internal Color _gridColor;
			protected internal Transform3D _gridTransform;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Имя файла
			/// </summary>
			public string FileName
			{
				get { return (string)GetValue(FileNameProperty); }
				set { SetValue(FileNameProperty, value); }
			}

			//
			// ПАРАМЕТРЫ ВИЗУАЛИЗАЦИИ
			//
			/// <summary>
			/// Режим показа каркаса
			/// </summary>
			public Viewport3DX Helix3DViewer
			{
				get
				{
					return helix3DViewer;
				}
			}


			//
			// ПАРАМЕТРЫ ВИЗУАЛИЗАЦИИ
			//
			/// <summary>
			/// Режим показа каркаса
			/// </summary>
			public bool ShowWireframe
			{
				get
				{
					return _showWireframe;
				}
				set
				{
					if (_showWireframe != value)
					{
						_showWireframe = value;
						NotifyPropertyChanged(PropertyArgsShowWireframe);

						foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
						{
							return node.IsRenderable;
						}))
						{
							if (node is MeshNode mesh)
							{
								mesh.RenderWireframe = value;
							}
						}
					}
				}
			}

			/// <summary>
			/// Режим закраски по Гуро
			/// </summary>
			public bool RenderFlat
			{
				get
				{
					return _renderFlat;
				}
				set
				{
					if (_renderFlat != value)
					{
						_renderFlat = value;
						NotifyPropertyChanged(PropertyArgsRenderFlat);

						foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
						{
							return node.IsRenderable;
						}))
						{
							if (node is MeshNode mesh)
							{
								if (mesh.Material is PhongMaterialCore phong)
								{
									phong.EnableFlatShading = value;
								}
								else if (mesh.Material is PBRMaterialCore pbr)
								{
									pbr.EnableFlatShading = value;
								}
							}
						}
					}
				}
			}

			/// <summary>
			/// Ренденить карту окружения
			/// </summary>
			public bool RenderEnvironmentMap
			{
				get
				{
					return _renderEnvironmentMap;
				}
				set
				{
					if (_renderEnvironmentMap != value)
					{
						_renderEnvironmentMap = value;
						NotifyPropertyChanged(PropertyArgsRenderEnvironmentMap);
						if (_sceneRoot != null)
						{
							foreach (var node in _sceneRoot.Traverse())
							{
								if (node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
								{
									material.RenderEnvironmentMap = value;
								}
							}
						}
					}
				}
			}

			/// <summary>
			/// Карта окружения
			/// </summary>
			public TextureModel EnvironmentMap
			{
				get
				{
					return _environmentMap;
				}
			}

			//
			// КАМЕРА
			//
			/// <summary>
			/// Текущая модель камеры
			/// </summary>
			public string? CameraModel
			{
				get { return _cameraModel; }
				set
				{
					if (_cameraModel != value)
					{
						_cameraModel = value;
						NotifyPropertyChanged(PropertyArgsCameraModel);
						RaiseCameraModelChanged();
					}
				}
			}

			/// <summary>
			/// Камера
			/// </summary>
			public Helix.Camera Camera
			{
				get
				{
					return _camera;
				}

				protected set
				{
					_camera = value;
					NotifyPropertyChanged(PropertyArgsCamera);
					if (value is Helix.OrthographicCamera)
					{
						CameraModel = value is Helix.PerspectiveCamera
										   ? PerspectiveCameraName
										   : (string?)OrthographicCameraName;
					}
					else
					{
						CameraModel = value is Helix.PerspectiveCamera
										   ? PerspectiveCameraName
										   : null;
					}
				}
			}

			/// <summary>
			/// Набор моделей камеры
			/// </summary>
			public List<string> CameraModelCollection { get; private set; }

			/// <summary>
			/// Событие смены камеры
			/// </summary>
			public event EventHandler CameraModelChanged;

			//
			// РЕНДЕР ТЕХНИКА
			//
			/// <summary>
			/// Менеджер эффектов
			/// </summary>
			public IEffectsManager EffectsManager
			{
				get { return _effectsManager; }
				protected set
				{
					if (_effectsManager != value)
					{
						_effectsManager = value;
						NotifyPropertyChanged(PropertyArgsEffectsManager);
					}
				}
			}

			//
			// БАЗОВЫЕ МОДЕЛИ
			//
			/// <summary>
			/// Сцена
			/// </summary>
			public Scene3D Scene
			{
				get { return _scene; }
				set
				{
					if (_scene != value)
					{
						_scene = value;
						NotifyPropertyChanged(PropertyArgsScene);
					}
				}
			}

			/// <summary>
			/// Корневой узел отображаемой сцены
			/// </summary>
			public SceneNode SceneRoot
			{
				get { return _sceneRoot; }
				set
				{
					if (_sceneRoot != value)
					{
						_sceneRoot = value;
						NotifyPropertyChanged(PropertyArgsSceneRoot);
					}
				}
			}

			/// <summary>
			/// Все объекты сцены
			/// </summary>
			public Helix.SceneNodeGroupModel3D GroupModel
			{
				get { return _groupModel; }
				set
				{
					if (_groupModel != value)
					{
						_groupModel = value;
						NotifyPropertyChanged(PropertyArgsGroupModel);
					}
				}
			}

			/// <summary>
			/// Выбранная геометрия
			/// </summary>
			public Helix.GeometryModel3D SelectedModel
			{
				get { return _selectedModel; }
				set
				{
					if (_selectedModel != value)
					{
						_selectedModel = value;
						NotifyPropertyChanged(PropertyArgsSelectedModel);
					}
				}
			}

			//
			// АНИМАЦИЯ
			//
			/// <summary>
			/// Статус проигрывания анимации
			/// </summary>
			public bool EnableAnimation
			{
				get { return _enableAnimation; }
				set
				{
					if (_enableAnimation != value)
					{
						_enableAnimation = value;
						NotifyPropertyChanged(PropertyArgsEnableAnimation);
						if (value)
						{
							StartAnimation();
						}
						else
						{
							StopAnimation();
						}
					}
				}
			}

			/// <summary>
			/// Список всех анимации сцены
			/// </summary>
			public IList<Animation> SceneAnimations
			{
				get { return _sceneAnimations; }
				set
				{
					_sceneAnimations = value;
				}
			}

			/// <summary>
			/// Список проигрываемых анимаций
			/// </summary>
			public ObservableCollection<IAnimationUpdater> Animations
			{
				get { return _animationsUpdater; }
				set
				{
					_animationsUpdater = value;
				}
			}

			/// <summary>
			/// Текущая выбранная анимация
			/// </summary>
			public IAnimationUpdater? SelectedAnimation
			{
				get
				{
					return _selectedAnimationUpdater;
				}
				set
				{
					if (_selectedAnimationUpdater != value)
					{
						_selectedAnimationUpdater = value;
						NotifyPropertyChanged(PropertyArgsSelectedAnimation);
						StopAnimation();
						if (value != null)
						{
							_animationUpdater = value;
							_animationUpdater.Reset();
							_animationUpdater.RepeatMode = AnimationRepeatMode.Loop;
							//mAnimationUpdater.Speed = mSpeedAnimation;
						}
						else
						{
							_animationUpdater = null;
						}
						if (_enableAnimation)
						{
							StartAnimation();
						}
					}
				}
			}

			/// <summary>
			/// Скорость воспроизведения анимации
			/// </summary>
			public float SpeedAnimation
			{
				get
				{
					return _speedAnimation;
				}
				set
				{
					if (_speedAnimation != value)
					{
						_speedAnimation = value;
						NotifyPropertyChanged(PropertyArgsSpeedAnimation);
					}
				}
			}

			//
			// ОПОРНАЯ СЕТКА
			//
			/// <summary>
			/// Геометрия сетки
			/// </summary>
			public LineGeometry3D GridGeometry
			{
				get { return _gridGeometry; }
				set
				{
					if (_gridGeometry != value)
					{
						_gridGeometry = value;
						NotifyPropertyChanged(PropertyArgsGridGeometry);
					}
				}
			}

			/// <summary>
			/// Цвет линий сетки
			/// </summary>
			public Color GridColor
			{
				get { return _gridColor; }
				set
				{
					if (_gridColor != value)
					{
						_gridColor = value;
						NotifyPropertyChanged(PropertyArgsGridColor);
					}
				}
			}

			/// <summary>
			/// Трансформация сетки
			/// </summary>
			public Transform3D GridTransform
			{
				get { return _gridTransform; }
				set
				{
					if (_gridTransform != value)
					{
						_gridTransform = value;
						NotifyPropertyChanged(PropertyArgsGridTransform);
					}
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusViewerContent3D()
			{
				InitializeComponent();

				InitCamera();

				InitEffectsManager();

				InitGroupModel();

				InitAnimation();

				InitGrid();
			}
			#endregion

			#region ======================================= МЕТОДЫ IDisposable ========================================
			/// <summary>
			/// To detect redundant calls
			/// </summary>
			private bool disposedValue = false;


			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Освобождение управляемых ресурсов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Dispose()
			{
				// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
				Dispose(disposing: true);
				System.GC.SuppressFinalize(this);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Освобождение управляемых ресурсов
			/// </summary>
			/// <param name="disposing">Статус освобождения</param>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						if (EffectsManager != null)
						{
							var effectManager = EffectsManager as IDisposable;
							Disposer.RemoveAndDispose(ref effectManager);
						}
					}

					disposedValue = true;
				}
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение камеры
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseCameraModelChanged()
			{
				CameraModelChanged?.Invoke(this, new EventArgs());
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusViewerContentFile ============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание нового файла с указанным именем и параметрами
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			/// <param name="parameters_create">Параметры создания файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void NewFile(string file_name, CParameters? parameters_create)
			{
				// Method intentionally left empty.
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие указанного файла
			/// </summary>
			/// <param name="file_name">Полное имя файла</param>
			/// <param name="parameters_open">Параметры открытия файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void OpenFile(string? file_name, CParameters? parameters_open)
			{
				Assimp.PostProcessSteps post_process_steps = Assimp.PostProcessSteps.None;
				TreeView? tree_view_model_structure = null;

				// Если файл пустой то используем диалог
				if (string.IsNullOrEmpty(file_name))
				{
					file_name = XFileDialog.Open("Открыть файл", "", OpenFileFilter);
					if (file_name != null && file_name.IsExists())
					{
						if (parameters_open != null)
						{
							post_process_steps = parameters_open.GetValueOfType<Assimp.PostProcessSteps>(Assimp.PostProcessSteps.None);
							tree_view_model_structure = parameters_open.GetValueOfType<TreeView>();
						}

						// Загружаем файл
						Load(file_name, post_process_steps, tree_view_model_structure!);

						FileName = file_name;
						XLogger.LogInfoModule(nameof(LotusViewerContent3D), $"Открыт файл с именем: [{FileName}]");
					}
				}
				else
				{
					if (parameters_open != null)
					{
						post_process_steps = parameters_open.GetValueOfType<Assimp.PostProcessSteps>(Assimp.PostProcessSteps.None);
						tree_view_model_structure = parameters_open.GetValueOfType<TreeView>();
					}

					// Загружаем файл
					Load(file_name, post_process_steps, tree_view_model_structure!);

					FileName = file_name;
					XLogger.LogInfoModule(nameof(LotusViewerContent3D), $"Открыт файл с именем: [{FileName}]");
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранения файла
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SaveFile()
			{
				// Method intentionally left empty.
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранение файла под новым именем и параметрами
			/// </summary>
			/// <param name="file_name">Полное имя файла</param>
			/// <param name="parameters_save">Параметры сохранения файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void SaveAsFile(string file_name, CParameters? parameters_save)
			{
				if (string.IsNullOrEmpty(file_name))
				{
					if (string.IsNullOrEmpty(FileName) == false)
					{

					}
					else
					{

					}
				}
				else
				{
					if (XFilePath.CheckCorrectFileName(file_name))
					{

					}
				}
			}

			//-------------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Печать файла
			/// </summary>
			/// <param name="parameters_print">Параметры печати файла</param>
			//-------------------------------------------------------------------------------------------------------------
			public void PrintFile(CParameters? parameters_print)
			{
				// Method intentionally left empty.
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Экспорт файла под указанным именем и параметрами
			/// </summary>
			/// <param name="file_name">Полное имя файла</param>
			/// <param name="parameters_export">Параметры для экспорта файла</param>
			//---------------------------------------------------------------------------------------------------------
			public void ExportFile(string file_name, CParameters? parameters_export)
			{
				// Method intentionally left empty.
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие файла
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void CloseFile()
			{
				// Method intentionally left empty.
			}
			#endregion

			#region ======================================= МЕТОДЫ ИНИЦИАЛИЗАЦИИ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных камеры
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitCamera()
			{
				_defaultOrthographicCamera = new Helix.OrthographicCamera
				{
					Position = new Point3D(0, 0, 5),
					LookDirection = new Vector3D(-0, -0, -5),
					UpDirection = new Vector3D(0, 1, 0),
					NearPlaneDistance = 0.05,
					FarPlaneDistance = 5000
				};

				_defaultPerspectiveCamera = new Helix.PerspectiveCamera
				{
					Position = new Point3D(0, 0, 5),
					LookDirection = new Vector3D(-0, -0, -5),
					UpDirection = new Vector3D(0, 1, 0),
					NearPlaneDistance = 0.05,
					FarPlaneDistance = 5000
				};

				// camera models
				CameraModelCollection = new List<string>()
				{
					OrthographicCameraName,
					PerspectiveCameraName,
				};

				// on camera changed callback
				CameraModelChanged += (sender, args) =>
				{
					if (_cameraModel == OrthographicCameraName)
					{
						if (!(Camera is Helix.OrthographicCamera))
							Camera = _defaultOrthographicCamera;
					}
					else if (_cameraModel == PerspectiveCameraName)
					{
						if (!(Camera is Helix.PerspectiveCamera))
							Camera = _defaultPerspectiveCamera;
					}
					else
					{
						//throw new Helix3D.("Camera Model Error.");
					}
				};

				// default camera model
				CameraModel = PerspectiveCameraName;
				Camera = _defaultPerspectiveCamera;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных рендер техники
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitEffectsManager()
			{
				EffectsManager = new DefaultEffectsManager();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных базовой модели
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitGroupModel()
			{
				_groupModel = new Helix.SceneNodeGroupModel3D();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных анимации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitAnimation()
			{
				_animationsUpdater = new ObservableCollection<IAnimationUpdater>();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных опорной сетки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void InitGrid()
			{
				// floor plane grid
				GridGeometry = LineBuilder.GenerateGrid(new SharpDX.Vector3(0, 1, 0), 0, 10);
				GridColor = Colors.DarkGray;
				GridTransform = new TranslateTransform3D(-5, 0, -5);
			}
			#endregion

			#region Main methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка 3D контента из файла
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			/// <param name="post_process_steps">Флаги обработки контента</param>
			/// <param name="tree_view_model_structure">Дерево для просмотра внутренней структуры 3D контента</param>
			//---------------------------------------------------------------------------------------------------------
			public void Load(string file_name, Assimp.PostProcessSteps post_process_steps, TreeView tree_view_model_structure)
			{
				if (_isLoading)
				{
					return;
				}

				StopAnimation();

				_isLoading = true;

				// Загружаем в отдельной задачи
				Task.Run(() =>
				{
					var loader = new Importer();

					if (post_process_steps != Assimp.PostProcessSteps.None)
					{
						loader.Configuration.AssimpPostProcessSteps = post_process_steps;
					}

					return loader.Load(file_name);
				}).ContinueWith((result) =>
				{
					_isLoading = false;
					if (result.IsCompleted)
					{
						HelixToolkitScene helix_toolkit_scene = result.Result;
						if (helix_toolkit_scene == null) return;
						_sceneRoot = helix_toolkit_scene.Root;
						_sceneAnimations = helix_toolkit_scene.Animations;
						Animations.Clear();
						GroupModel.Clear();
						if (helix_toolkit_scene.Root != null)
						{
							foreach (var node in helix_toolkit_scene.Root.Traverse())
							{
								if (node is MaterialGeometryNode m)
								{
									if (m.Material is PBRMaterialCore pbr)
									{
										pbr.RenderEnvironmentMap = RenderEnvironmentMap;
									}
									else if (m.Material is PhongMaterialCore phong)
									{
										phong.RenderEnvironmentMap = RenderEnvironmentMap;
									}
								}
							}
						}

						GroupModel.AddNode(helix_toolkit_scene.Root);
						if (helix_toolkit_scene.HasAnimation)
						{
							var dict = helix_toolkit_scene.Animations.CreateAnimationUpdaters();
							foreach (var animation in dict.Values)
							{
								Animations.Add(animation);
							}
						}
						foreach (var node in helix_toolkit_scene.Root.Traverse())
						{
							//node.Tag = new AttachedNodeViewModel(node);
						}

						if (tree_view_model_structure != null)
						{
							tree_view_model_structure.ItemsSource = _sceneRoot.Items;
						}

					}
					else if (result.IsFaulted && result.Exception != null)
					{
						MessageBox.Show(result.Exception.Message);
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Старт анимации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void StartAnimation()
			{
				_compositeHelper.Rendering += CompositeHelper_Rendering;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Остановка анимации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void StopAnimation()
			{
				_compositeHelper.Rendering -= CompositeHelper_Rendering;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление анимации
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void CompositeHelper_Rendering(object? sender, RenderingEventArgs args)
			{
				if (_animationUpdater != null)
				{
					_animationUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler? PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(string property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
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