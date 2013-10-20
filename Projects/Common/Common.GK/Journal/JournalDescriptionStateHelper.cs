﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class JournalDescriptionStateHelper
	{
		public static XStateClass GetStateClassByName(string name)
		{
			var journalDescriptionState = JournalDescriptionStates.FirstOrDefault(x => x.Name == name);
			if (journalDescriptionState != null)
				return journalDescriptionState.StateClass;
			return XStateClass.Norm;
		}

		static JournalDescriptionStateHelper()
		{
			JournalDescriptionStates = new List<JournalDescriptionState>();
			Add("Технология", XStateClass.TechnologicalRegime, "Перевод в технологический режим для обновления ПО, чтения и записи конфигурации");
			Add("Установка часов", XStateClass.Info, "Синхронизация часов ГК и операционной системы");
			Add("Смена ПО", XStateClass.TechnologicalRegime, "Изменение программного обеспечения ГК и КАУ");
            Add("Смена БД", XStateClass.TechnologicalRegime, "Изменение базы данных ГК и КАУ");
			Add("Работа", XStateClass.On, "Работа шкафов управления");
            Add("Вход пользователя в прибор", XStateClass.Info, "Вход пользователя в ГК");
            Add("Выход пользователя из прибора", XStateClass.Info, "Выход пользователя из ГК");
            Add("Ошибка управления", XStateClass.Failure, "Ошибка управления исполнительным устройством");
			Add("Введен новый пользователь", XStateClass.Info, "Добавление нового пользователя ГК");
            Add("Изменена учетная информация пользователя", XStateClass.Info, "Изменение учетной информации пользователя ГК");
			Add("Произведена настройка сети", XStateClass.Info, "Изменение конфигурации сети, к которой подключён ГК");
			Add("Неизвестный тип", XStateClass.Unknown, "Подключено устройство неподдерживаемого типа");
            Add("Устройство с таким адресом не описано при конфигурации", XStateClass.Unknown, "На шлейфе КАУ, обнаружено устройство с адресом, не описанным в конфигурации");
            Add("При конфигурации описан другой тип", XStateClass.Unknown, "По адресу обнаружено устройство с типом, не соответствующим типу, описанному в конфигурации");
			Add("Изменился заводской номер", XStateClass.Info, "По адресу обнаружено устройство с другим заводским номером");
			Add("Пожар-1", XStateClass.Fire1, "Событие инициируется зоной");
            Add("Сработка-1", XStateClass.Fire1, "Событие инициируется устройством");
            Add("Пожар-2", XStateClass.Fire2, "Событие инициируется зоной");
            Add("Сработка-2", XStateClass.Fire2, "Событие инициируется устройством");
            Add("Внимание", XStateClass.Attention, "Событие инициируется зоной");
            Add("Неисправность", XStateClass.Failure, "Событие инициируется устройством");
            Add("Тест", XStateClass.Test, "Событие инициируется тест-кнопкой устройства");
			Add("Запыленность", XStateClass.Service, "Критический уровень запылённости датчика");
            Add("Информация", XStateClass.Info, "Событие инициируется устройством");
			Add("Состояние", XStateClass.Info, "Изменение состояния объекта");
			Add("Режим работы", XStateClass.Info, "Изменение режима работы: ручной, автоматический, отключено");
			Add("Параметры", XStateClass.Info, "Запись параметров в устройство");
			Add("Норма", XStateClass.Norm, "Переход в состояние Норма");
			Add("Вход пользователя в систему", XStateClass.Info, "Вход пользователя в ОПС Firesec");
            Add("Выход пользователя из системы", XStateClass.Info, "Выход пользователя из ОПС Firesec");
			Add("Команда оператора", XStateClass.Info, "Команда на сброс, управление ИУ, отключение, снятие отключения");
		}

		static void Add(string name, XStateClass stateClass, string description)
		{
            var journalDescriptionState = new JournalDescriptionState(name, stateClass, description);
			JournalDescriptionStates.Add(journalDescriptionState);
		}

		public static List<JournalDescriptionState> JournalDescriptionStates { get; private set; }
	}
}