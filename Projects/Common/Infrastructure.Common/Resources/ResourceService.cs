﻿using System.Windows;

namespace Infrastructure.Common
{
	public class ResourceService
	{
		public void AddResource(ResourceDescription description)
		{
			if (Application.Current != null)
				Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = description.Source });
		}
	}
}