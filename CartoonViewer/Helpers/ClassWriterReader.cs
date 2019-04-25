namespace CartoonViewer.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using Newtonsoft.Json;
	using ViewModels;
	using static Creator;

	public static class ClassWriterReader
	{
		/// <summary>
		/// Запись необходимого класса в файл
		/// </summary>
		/// <typeparam name="T">Исходный класс</typeparam>
		/// <param name="writedClass">Экзеспляр записываемого класса</param>
		/// <param name="fileName">Имя файла хранения класса</param>
		/// <param name="fileException">Расширение файла хранения класса</param>
		/// <param name="fileFolderPath">Расположение папки с файлом хранения класса</param>
		public static void WriteClassInFile<T>(T writedClass, string fileName,
			string fileException = null, string fileFolderPath = null)
			where T : class
		{
			var fullPath = $"{fileFolderPath}\\{fileName}{fileException}";

			if(File.Exists(fullPath) is false)
			{
				fullPath = CreateFile($"{fileName}", false,
									  fileException, fileFolderPath);
			}

			using(var sw = new StreamWriter(fullPath))
			{
				sw.WriteLine(JsonConvert.SerializeObject(value: writedClass));
			}
		}

		/// <summary>
		/// Запись коллекции классов в файл
		/// </summary>
		/// <param name="classCollection">Коллекция классов</param>
		/// <param name="fileName">Имя файла хранения коллекции</param>
		/// <param name="fileException">Расширение файла хранения коллекции</param>
		/// <param name="fileFolderPath">Расположение папки с файлом хранения коллекции</param>
		public static void WriteClassCollectionInFile(ICollection classCollection, string fileName,
			string fileException = null, string fileFolderPath = null)
		{
			var fullPath = $"{fileFolderPath}\\{fileName}{fileException}";

			if(File.Exists(fullPath) is false)
			{
				fullPath = CreateFile($"{fileName}", false,
									  fileException, fileFolderPath);
			}

			using(var sw = new StreamWriter(fullPath))
			{
				foreach(var currentClass in classCollection)
				{
					sw.WriteLine(JsonConvert.SerializeObject(currentClass));
				}
			}
		}
		/// <summary>
		/// Чтение и возврат записанного класса из файла
		/// </summary>
		/// <typeparam name="T">Исходный класс</typeparam>
		/// <param name="filePath">Расположение файла с данными</param>
		/// <returns>Записанный класс</returns>
		public static T ReadClassFromFile<T>(string filePath)
			where T : class
		{
			T result;
			try
			{
				using(var sr = new StreamReader(filePath))
				{
					result = JsonConvert.DeserializeObject<T>(sr.ReadLine());
				}
			}
			catch(Exception e)
			{
				Helper.WinMan.ShowDialog(new DialogViewModel(e.Message, Helper.DialogType.INFO));
				return null;
			}


			return result;
		}
		/// <summary>
		/// Чтение и возврат коллекции записанного класса
		/// </summary>
		/// <typeparam name="T">Исходный класс</typeparam>
		/// <param name="filePath">Расположение файла с данными</param>
		/// <returns>Записанная коллекция классов</returns>
		public static ICollection<T> ReadClassCollectionFromFile<T>(string filePath)
			where T : class
		{
			ICollection<T> result = new List<T>();

			try
			{
				using(var sr = new StreamReader(filePath))
				{
					while(sr.EndOfStream is false)
					{
						var readedClass = JsonConvert.DeserializeObject<T>(sr.ReadLine());
						result.Add(readedClass);
					}
				}
			}
			catch(Exception e)
			{
				Helper.WinMan.ShowDialog(new DialogViewModel(e.Message, Helper.DialogType.INFO));
				return null;
			}

			return result;
		}
	}
}
