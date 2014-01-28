﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDDriver
{
	public class SendResult
	{
		public List<byte> Bytes { get; set; }
		public bool HasError { get; set; }
		public string Error { get; set; }

		public SendResult(string error)
		{
			HasError = true;
			Error = error;
			Bytes = new List<byte>();
		}

		public SendResult(List<byte> bytes)
		{
			Bytes = bytes;
			HasError = false;
		}
	}
}