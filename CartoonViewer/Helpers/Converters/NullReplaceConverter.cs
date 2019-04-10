﻿namespace CartoonViewer.Helpers.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;

	public class NullReplaceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value ?? parameter;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.Equals(parameter) ? null : value;
		}
	}
}
