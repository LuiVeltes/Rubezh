﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using System.Collections.ObjectModel;

namespace TestUSB
{
	internal class ReadPolling
	{
		public static UsbDevice MyUsbDevice;

		#region SET YOUR USB Vendor and Product ID!

		public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(0xC251, 0x1303);

		#endregion

		public static void Run()
		{
			ErrorCode ec = ErrorCode.None;

			try
			{
				// Find and open the usb device.
				MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

				// If the device is open and ready
				if (MyUsbDevice == null) throw new Exception("Device Not Found.");

				// If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
				// it exposes an IUsbDevice interface. If not (WinUSB) the 
				// 'wholeUsbDevice' variable will be null indicating this is 
				// an interface of a device; it does not require or support 
				// configuration and interface selection.
				IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
				if (!ReferenceEquals(wholeUsbDevice, null))
				{
					// This is a "whole" USB device. Before it can be used, 
					// the desired configuration and interface must be selected.

					// Select config #1
					wholeUsbDevice.SetConfiguration(1);

					// Claim interface #0.
					wholeUsbDevice.ClaimInterface(0);
				}

				// open read endpoint 1.
				UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);


				byte[] readBuffer = new byte[1024];
				while (ec == ErrorCode.None)
				{
					int bytesRead;

					// If the device hasn't sent data in the last 5 seconds,
					// a timeout error (ec = IoTimedOut) will occur. 
					ec = reader.Read(readBuffer, 5000, out bytesRead);

					if (bytesRead == 0) throw new Exception(string.Format("{0}:No more bytes!", ec));
					Console.WriteLine("{0} bytes read", bytesRead);

					// Write that output to the console.
					Console.Write(Encoding.Default.GetString(readBuffer, 0, bytesRead));
				}

				Console.WriteLine("\r\nDone!\r\n");
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
			}
			finally
			{
				if (MyUsbDevice != null)
				{
					if (MyUsbDevice.IsOpen)
					{
						// If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
						// it exposes an IUsbDevice interface. If not (WinUSB) the 
						// 'wholeUsbDevice' variable will be null indicating this is 
						// an interface of a device; it does not require or support 
						// configuration and interface selection.
						IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
						if (!ReferenceEquals(wholeUsbDevice, null))
						{
							// Release interface #0.
							wholeUsbDevice.ReleaseInterface(0);
						}

						MyUsbDevice.Close();
					}
					MyUsbDevice = null;

					// Free usb resources
					UsbDevice.Exit();

				}

				// Wait for user input..
				Console.ReadKey();
			}
		}

		public static void Run2()
		{
			UsbRegDeviceList allDevices = UsbDevice.AllDevices;
			foreach (UsbRegistry usbRegistry in allDevices)
			{
				if (usbRegistry.Open(out MyUsbDevice))
				{
					Console.WriteLine(MyUsbDevice.Info.ToString());
					for (int iConfig = 0; iConfig < MyUsbDevice.Configs.Count; iConfig++)
					{
						UsbConfigInfo configInfo = MyUsbDevice.Configs[iConfig];
						Console.WriteLine(configInfo.ToString());

						ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
						for (int iInterface = 0; iInterface < interfaceList.Count; iInterface++)
						{
							UsbInterfaceInfo interfaceInfo = interfaceList[iInterface];
							Console.WriteLine(interfaceInfo.ToString());

							ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
							for (int iEndpoint = 0; iEndpoint < endpointList.Count; iEndpoint++)
							{
								Console.WriteLine(endpointList[iEndpoint].ToString());
							}
						}
					}
				}
			}
		}
	}
}