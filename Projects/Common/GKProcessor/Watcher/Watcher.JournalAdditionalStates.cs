﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public partial class Watcher
	{
		void ParseAdditionalStates(JournalParser journalParser)
		{
			var journalItem = journalParser.JournalItem;
			var description = EventDescriptionAttributeHelper.ToName(journalItem.JournalEventDescriptionType);
			var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalParser.GKObjectNo);

			if (descriptor != null && descriptor.Device != null)
			{
				var deviceState = descriptor.Device.InternalState;
				if (journalItem.JournalEventNameType == JournalEventNameType.Неисправность)
				{
					if (!string.IsNullOrEmpty(description))
					{
						AddAdditionalState(deviceState, description, XStateClass.Failure);
						if (descriptor.Device.DriverType == GKDriverType.Battery)
						{
							var batteryNamesGroup = BatteryJournalHelper.BatteryNamesGroups.FirstOrDefault(x => x.Names.Contains(description));
							if (batteryNamesGroup != null)
							{
								foreach (var name in batteryNamesGroup.Names)
								{
									if (name != description)
									{
										deviceState.AdditionalStates.RemoveAll(x => x.Name == name);
									}
								}
							}
						}
					}
				}
				if (journalItem.JournalEventNameType == JournalEventNameType.Неисправность_устранена)
				{
					if (string.IsNullOrEmpty(description))
					{
						deviceState.AdditionalStates.RemoveAll(x => x.StateClass == XStateClass.Failure);
					}
					else
					{
						deviceState.AdditionalStates.RemoveAll(x => x.Name == description);
						if (descriptor.Device.DriverType == GKDriverType.Battery)
						{
							var batteryNamesGroup = BatteryJournalHelper.BatteryNamesGroups.FirstOrDefault(x => x.ResetName == description);
							if (batteryNamesGroup != null)
							{
								foreach (var name in batteryNamesGroup.Names)
								{
									deviceState.AdditionalStates.RemoveAll(x => x.Name == name);
								}
							}
						}

						if (description == "Обрыв АЛС 1-2")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 1");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 2");
						}
						if (description == "Обрыв АЛС 3-4")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 3");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 4");
						}
						if (description == "Обрыв АЛС 5-6")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 5");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 6");
						}
						if (description == "Обрыв АЛС 7-8")
						{
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 7");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Обрыв АЛС 8");
						}
					}
				}
				if (journalItem.JournalEventNameType == JournalEventNameType.Информация)
				{
					switch (description)
					{
						case "Низкий уровень":
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Высокий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Аварийный уровень");
							AddAdditionalState(deviceState, "Низкий уровень", XStateClass.Info);
							break;
						case "Высокий уровень":
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Аварийный уровень");
							AddAdditionalState(deviceState, "Низкий уровень", XStateClass.Info);
							AddAdditionalState(deviceState, "Высокий уровень", XStateClass.Info);
							break;
						case "Аварийный уровень":
							AddAdditionalState(deviceState, "Низкий уровень", XStateClass.Info);
							AddAdditionalState(deviceState, "Высокий уровень", XStateClass.Info);
							AddAdditionalState(deviceState, "Аварийный уровень", XStateClass.Failure);
							break;
						case "Уровень норма":
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Низкий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Высокий уровень");
							deviceState.AdditionalStates.RemoveAll(x => x.Name == "Аварийный уровень");
							break;
					}
				}
			}
		}

		void AddAdditionalState(GKBaseInternalState baseState, string description, XStateClass stateClass)
		{
			if (!baseState.AdditionalStates.Any(x => x.Name == description))
			{
				var additionalState = new GKAdditionalState()
				{
					StateClass = stateClass,
					Name = description
				};
				baseState.AdditionalStates.Add(additionalState);
			}
		}
	}

	public static class BatteryJournalHelper
	{
		static BatteryJournalHelper()
		{
			BatteryNamesGroups = new List<BatteryNamesGroup>();

			var batteryNamesGroup1 = new BatteryNamesGroup("Выход 1");
			batteryNamesGroup1.Names.Add("КЗ Выхода 1");
			batteryNamesGroup1.Names.Add("Перегрузка Выхода 1");
			batteryNamesGroup1.Names.Add("Напряжение Выхода 1 выше нормы");
			BatteryNamesGroups.Add(batteryNamesGroup1);

			var batteryNamesGroup2 = new BatteryNamesGroup("Выход 2");
			batteryNamesGroup2.Names.Add("КЗ Выхода 2");
			batteryNamesGroup2.Names.Add("Перегрузка Выхода 2");
			batteryNamesGroup2.Names.Add("Напряжение Выхода 2 выше нормы");
			BatteryNamesGroups.Add(batteryNamesGroup2);

			var batteryNamesGroup3 = new BatteryNamesGroup("АКБ 1");
			batteryNamesGroup3.Names.Add("АКБ 1 Разряд");
			batteryNamesGroup3.Names.Add("АКБ 1 Глубокий Разряд");
			batteryNamesGroup3.Names.Add("АКБ 1 Отсутствие");
			BatteryNamesGroups.Add(batteryNamesGroup3);

			var batteryNamesGroup4 = new BatteryNamesGroup("АКБ 2");
			batteryNamesGroup4.Names.Add("АКБ 2 Разряд");
			batteryNamesGroup4.Names.Add("АКБ 2 Глубокий Разряд");
			batteryNamesGroup4.Names.Add("АКБ 2 Отсутствие");
			BatteryNamesGroups.Add(batteryNamesGroup4);
		}

		public static List<BatteryNamesGroup> BatteryNamesGroups { get; private set; }

		public class BatteryNamesGroup
		{
			public BatteryNamesGroup(string resetName)
			{
				ResetName = resetName;
				Names = new List<string>();
			}

			public string ResetName { get; set; }
			public List<string> Names { get; set; }
		}
	}
}