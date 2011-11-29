﻿using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using Firesec.Plans;
using FiresecAPI.Models;

namespace FiresecService.Converters
{
    public static class PlansConverter
    {
        public static PlansConfiguration Convert(surfaces innerPlans)
        {
            var plansConfiguration = new PlansConfiguration();

            if ((innerPlans != null) && (innerPlans.surface != null))
            {
                foreach (var innerPlan in innerPlans.surface)
                {
                    var plan = new Plan()
                    {
                        Caption = innerPlan.caption,
                        Height = Double.Parse(innerPlan.height) * 10,
                        Width = Double.Parse(innerPlan.width) * 10
                    };

                    foreach (var innerLayer in innerPlan.layer)
                    {
                        if (innerLayer.elements == null)
                            continue;

                        if (innerLayer.name == "План")
                        {
                            foreach (var innerElementLayer in innerLayer.elements)
                            {
                                switch (innerElementLayer.@class)
                                {
                                    case "TSCDePicture":
                                        int pictureIndex = 0;
                                        foreach (var innerPicture in innerElementLayer.picture)
                                        {
                                            if (string.IsNullOrEmpty(innerPicture.idx))
                                                innerPicture.idx = pictureIndex++.ToString();

                                            var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\Pictures\\Sample" + innerPicture.idx + "." + innerPicture.ext);
                                            if (File.Exists(directoryInfo.FullName) == false)
                                                continue;

                                            if (innerPicture.ext == "emf")
                                            {
                                                var metafile = new Metafile(directoryInfo.FullName);
                                                innerPicture.ext = "bmp";
                                                directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\Pictures\\Sample" + innerPicture.idx + "." + innerPicture.ext);
                                                metafile.Save(directoryInfo.FullName, ImageFormat.Bmp);
                                                metafile.Dispose();
                                            }

                                            byte[] backgroundPixels = File.ReadAllBytes(directoryInfo.FullName);

                                            var elementRectangle = new ElementRectangle()
                                            {
                                                Left = Parse(innerElementLayer.rect[0].left),
                                                Top = Parse(innerElementLayer.rect[0].top),
                                                Height = Parse(innerElementLayer.rect[0].bottom) - Parse(innerElementLayer.rect[0].top),
                                                Width = Parse(innerElementLayer.rect[0].right) - Parse(innerElementLayer.rect[0].left),
                                                BackgroundPixels = backgroundPixels
                                            };

                                            if ((elementRectangle.Left == 0) && (elementRectangle.Top == 0) && (elementRectangle.Width == plan.Width) && (elementRectangle.Height == plan.Height))
                                            {
                                                plan.BackgroundPixels = elementRectangle.BackgroundPixels;
                                            }
                                            else
                                            {
                                                plan.ElementRectangles.Add(elementRectangle);
                                            }
                                        }
                                        break;

                                    case "TSCDeLabel":
                                        var elementTextBlock = new ElementTextBlock()
                                        {
                                            Text = innerElementLayer.caption,
                                            Left = Parse(innerElementLayer.rect[0].left),
                                            Top = Parse(innerElementLayer.rect[0].top),
                                        };

                                        if (innerElementLayer.brush != null)
                                            try
                                            {
                                                elementTextBlock.BorderColor = (Color)ColorConverter.ConvertFromString(innerElementLayer.brush[0].color);
                                            }
                                            catch { ;}

                                        if (innerElementLayer.pen != null)
                                            try
                                            {
                                                elementTextBlock.ForegroundColor = (Color)ColorConverter.ConvertFromString(innerElementLayer.pen[0].color);
                                            }
                                            catch { ;}

                                        plan.ElementTextBlocks.Add(elementTextBlock);
                                        break;
                                }
                            }
                        }
                        if ((innerLayer.name == "Зоны") || (innerLayer.name == "Несвязанные зоны") || (innerLayer.name == "Пожарные зоны") || (innerLayer.name == "Охранные зоны"))
                        {
                            foreach (var innerZone in innerLayer.elements)
                            {
                                ulong? zoneNo = null;

                                long longId = long.Parse(innerZone.id);
                                int intId = (int)longId;
                                foreach (var zone in ConfigurationConverter.DeviceConfiguration.Zones)
                                {
                                    foreach (var zoneShapeId in zone.ShapeIds)
                                    {
                                        if ((zoneShapeId == longId.ToString()) ||
                                            (zoneShapeId == intId.ToString()))
                                        {
                                            zoneNo = zone.No;
                                        }
                                    }
                                }

                                switch (innerZone.@class)
                                {
                                    case "TFS_PolyZoneShape":
                                        if (innerZone.points != null)
                                        {
                                            var elementPolygonZone = new ElementPolygonZone()
                                            {
                                                ZoneNo = zoneNo,
                                                PolygonPoints = new PointCollection()
                                            };
                                            foreach (var innerPoint in innerZone.points)
                                            {
                                                var point = new System.Windows.Point()
                                                {
                                                    X = Parse(innerPoint.x),
                                                    Y = Parse(innerPoint.y)
                                                };
                                                innerPoint.y = innerPoint.y.Replace(".", ",");

                                                elementPolygonZone.PolygonPoints.Add(point);
                                            }
                                            elementPolygonZone.Normalize();
                                            plan.ElementPolygonZones.Add(elementPolygonZone);
                                        };
                                        break;

                                    case "TFS_ZoneShape":
                                        var elementRectangleZone = new ElementRectangleZone()
                                        {
                                            ZoneNo = zoneNo,
                                            Left = Math.Min(Parse(innerZone.rect[0].left), Parse(innerZone.rect[0].right)),
                                            Top = Math.Min(Parse(innerZone.rect[0].top), Parse(innerZone.rect[0].bottom)),
                                            Width = Math.Abs(Parse(innerZone.rect[0].right) - Parse(innerZone.rect[0].left)),
                                            Height = Math.Abs(Parse(innerZone.rect[0].bottom) - Parse(innerZone.rect[0].top))
                                        };

                                        plan.ElementRectangleZones.Add(elementRectangleZone);
                                        break;
                                }
                            }
                        };

                        if (innerLayer.name == "Устройства")
                        {
                            foreach (var innerDevice in innerLayer.elements)
                            {
                                if (innerDevice.rect != null)
                                {
                                    var innerRect = innerDevice.rect[0];

                                    long longId = long.Parse(innerDevice.id);
                                    int intId = (int)longId;

                                    var elementDevice = new ElementDevice()
                                    {
                                        Left = Parse(innerRect.left),
                                        Top = Parse(innerRect.top),
                                        Width = Parse(innerRect.right) - Parse(innerRect.left),
                                        Height = Parse(innerRect.bottom) - Parse(innerRect.top)
                                    };
                                    plan.ElementDevices.Add(elementDevice);

                                    foreach (var device in ConfigurationConverter.DeviceConfiguration.Devices)
                                    {
                                        foreach (var deviceShapeId in device.ShapeIds)
                                        {
                                            if ((deviceShapeId == longId.ToString()) ||
                                                (deviceShapeId == intId.ToString()))
                                            {
                                                elementDevice.DeviceUID = device.UID;
                                                device.PlanUIDs.Add(elementDevice.UID);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    plansConfiguration.Plans.Add(plan);
                }
            }

            DeleteDirectory(Environment.CurrentDirectory + "\\Pictures");

            return plansConfiguration;
        }

        static void DeleteDirectory(string directoryName)
        {
            try
            {
                if (Directory.Exists(directoryName))
                {
                    foreach (string file in Directory.GetFiles(directoryName))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                    Directory.Delete(directoryName, true);
                }
            }
            catch { return; }
        }

        static Double Parse(string input)
        {
            Double result;
            try
            {
                input = input.Replace(".", ",");
                result = Double.Parse(input);
                return result;
            }
            catch
            {
                input = input.Replace(",", ".");
                result = Double.Parse(input);
                return result;
            }
        }
    }
}
