using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonViewer.ViewModels
{
	using Caliburn.Micro;

	public class WindowsManagerViewModel : Conductor<Screen>.Collection.OneActive
	{
		public WindowsManagerViewModel(Screen activeItem) { ActiveItem = activeItem; }

		public WindowsManagerViewModel()
		{
			
		}
	}
}
