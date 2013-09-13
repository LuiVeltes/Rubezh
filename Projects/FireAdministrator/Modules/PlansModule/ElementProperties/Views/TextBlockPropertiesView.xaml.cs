﻿using System.Windows.Controls;

namespace PlansModule.Views
{
	public partial class TextBlockPropertiesView : UserControl
	{
		public TextBlockPropertiesView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(TextBlockPropertiesView_Loaded);
		}

		void TextBlockPropertiesView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_textBox.Focus();
		}
	}
}