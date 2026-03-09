using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFCommon
	*@{*/
    /// <summary>
    /// Статический класс для реализации методов загрузки <see cref="BitmapSource"/> из различных источников.
    /// </summary>
    public static class XWindowsLoaderBitmap
    {
        #region Fields
        /// <summary>
        /// Словарь иконок файла по имени расширения.
        /// </summary>
        public static readonly Dictionary<string, BitmapSource> IconFilesExtension = [];
        #endregion

        /// <summary>
        /// Загрузка изображения из ресурсов сборки.
        /// </summary>
        /// <param name="resourceName">Имя ресурса.</param>
        /// <returns>Изображение.</returns>
        public static BitmapSource? LoadBitmapFromResource(string resourceName)
        {
            var image = Properties.Resources.ResourceManager.GetObject(resourceName);
            if (image is System.Drawing.Bitmap source)
            {

                var h_bitmap = source.GetHbitmap();
                var result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bitmap, IntPtr.Zero,
                    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                XNative.DeleteObject(h_bitmap);

                return result;
            }

            return null;
        }

        /// <summary>
        /// Загрузка изображения из ресурсов сборки.
        /// </summary>
        /// <param name="resourceManager">Менеджер ресурсов.</param>
        /// <param name="resourceName">Имя ресурса.</param>
        /// <returns>Изображение.</returns>
        public static BitmapSource? LoadBitmapFromResource(System.Resources.ResourceManager resourceManager, string resourceName)
        {
            var image = resourceManager.GetObject(resourceName);

            if (image is System.Drawing.Bitmap source)
            {
                var h_bitmap = source.GetHbitmap();
                var result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bitmap, IntPtr.Zero,
                    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                XNative.DeleteObject(h_bitmap);

                return result;
            }

            return null;
        }

        /// <summary>
        /// Загрузка изображения из файла.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Изображение.</returns>
        public static BitmapSource LoadBitmapFromFile(string fileName)
        {
            var file_stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            var bitmap = new BitmapImage(new Uri(fileName));
            bitmap.BeginInit();
            bitmap.StreamSource = file_stream;
            bitmap.EndInit();

            file_stream.Close();

            return bitmap;
        }

        /// <summary>
        /// Создание объекта BitmapSource из стандартного дескриптора изображения.
        /// </summary>
        /// <param name="hBitmap">Дескриптор изображения.</param>
        /// <returns>Объект BitmapSource.</returns>
        public static BitmapSource CreateFromHBitmap(IntPtr hBitmap)
        {
            var result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            XNative.DeleteObject(hBitmap);

            return result;
        }

        /// <summary>
        /// Создание объекта BitmapSource из стандартного дескриптора изображения.
        /// </summary>
        /// <param name="hBitmap">Дескриптор изображения.</param>
        /// <param name="width">Требуемая ширина изображения.</param>
        /// <param name="height">Требуемая высота изображения.</param>
        /// <returns>Объект BitmapSource.</returns>
        public static BitmapSource CreateFromHBitmap(IntPtr hBitmap, int width, int height)
        {
            var result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(width, height));

            XNative.DeleteObject(hBitmap);

            return result;
        }

        /// <summary>
        /// Получение иконки файла связанного с типом файла.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="flags">Флаги получения иконок .</param>
        /// <returns>Изображение.</returns>
        public static BitmapSource GetIconFromFileTypeFromShell(string fileName, uint flags)
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext) == false && IconFilesExtension.TryGetValue(ext, out var bitmap_source))
            {
                return bitmap_source;
            }

            XNative.SHGetFileInfo(fileName, 0, ref XNative.ShellFileInfoDefault,
                (uint)Marshal.SizeOf(XNative.ShellFileInfoDefault), flags);

            //The icon is returned in the hIcon member of the shinfo struct
            var icon = System.Drawing.Icon.FromHandle(XNative.ShellFileInfoDefault.IconHandle);

            var h_bitmap = icon.ToBitmap().GetHbitmap();
            var result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bitmap, IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            XNative.DeleteObject(h_bitmap);

            if (string.IsNullOrEmpty(ext) == false)
            {
                IconFilesExtension.Add(ext, result);
            }

            return result;
        }

        /// <summary>
        /// Получение иконки файла связанного с типом файла.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Изображение.</returns>
        public static BitmapSource GetIconFromFileTypeFromExtract(string fileName)
        {
            if (Path.HasExtension(fileName))
            {
                var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
                if (sysicon is not null)
                {
                    var bmp_src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        sysicon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    sysicon.Dispose();

                    return bmp_src;
                }
            }

            return GetIconFromFileTypeFromShell(fileName, (uint)(TShellAttribute.Icon | TShellAttribute.SmallIcon));

        }
    }
    /**@}*/
}