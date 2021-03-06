﻿using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum PermissionType
	{
		[DescriptionAttribute("Все")]
		All,

		[DescriptionAttribute("Администратор")]
		Adm_All,

		[DescriptionAttribute("Просмотр конфигурации")]
		Adm_ViewConfig,

		[DescriptionAttribute("Применение конфигурации")]
		Adm_SetNewConfig,

		[DescriptionAttribute("Запись конфигурации в приборы")]
		Adm_WriteDeviceConfig,

		[DescriptionAttribute("Изменение ПО в приборах")]
		Adm_ChangeDevicesSoft,

		[DescriptionAttribute("Управление правами пользователей")]
		Adm_Security,

		[DescriptionAttribute("ОЗ")]
		Oper_All,

		[DescriptionAttribute("Вход")]
		Oper_Login,

		[DescriptionAttribute("Выход")]
		Oper_Logout,

		[DescriptionAttribute("Выход без пароля")]
		Oper_LogoutWithoutPassword,

		[DescriptionAttribute("Не требуется подтверждение тревог")]
		Oper_NoAlarmConfirm,

		[DescriptionAttribute("Постановка, снятие зон с охраны")]
		Oper_SecurityZone,

		[DescriptionAttribute("Управление устройствами, зонами и направлениями")]
		Oper_ControlDevices,

		[DescriptionAttribute("Управление интерфейсом")]
		Oper_ChangeView,

		[DescriptionAttribute("Разрешить не подтверждать команды паролем")]
		Oper_MayNotConfirmCommands,

		[DescriptionAttribute("СКД")]
		Oper_SKD,

		[DescriptionAttribute("Управление особо охраняемыми охранными зонами")]
		Oper_ExtraGuardZone,

		[DescriptionAttribute("Просмотр журнала")]
		Oper_Journal_View,

		[DescriptionAttribute("Просмотр архива")]
		Oper_Archive_View,

		[DescriptionAttribute("Настройка отображения архива")]
		Oper_Archive_Settings,

		[DescriptionAttribute("Список точек доступа")]
		Oper_Reports_Doors,

		[DescriptionAttribute("Отчет по событиям системы контроля доступа")]
		Oper_Reports_Events,

		[DescriptionAttribute("Маршрут сотрудника/посетителя")]
		Oper_Reports_EmployeeRoot,

		[DescriptionAttribute("Сведения о пропусках")]
		Oper_Reports_Cards,

		[DescriptionAttribute("Доступ в зоны сотрудников/посетителей")]
		Oper_Reports_Employees_Access,

		[DescriptionAttribute("Права доступа сотрудников/посетителей")]
		Oper_Reports_Employees_Rights,

		[DescriptionAttribute("Список подразделений организации")]
		Oper_Reports_Departments,

		[DescriptionAttribute("Список должностей организации")]
		Oper_Reports_Positions,

		[DescriptionAttribute("Местонахождение сотрудников/посетителей")]
		Oper_Reports_EmployeeZone,

		[DescriptionAttribute("Справка о сотруднике/посетителе")]
		Oper_Reports_Employee,

		[DescriptionAttribute("Дисциплинарный отчет")]
		Oper_Reports_Discipline,

		[DescriptionAttribute("Отчет по графикам работ")]
		Oper_Reports_Schedules,

		[DescriptionAttribute("Отчет по оправдательным документам")]
		Oper_Reports_Documents,

		[DescriptionAttribute("Справка по отработанному времени")]
		Oper_Reports_WorkTime,

		[DescriptionAttribute("Табель учета рабочего времени (форма Т-13)")]
		Oper_Reports_T13,

		[DescriptionAttribute("Просмотр устройств")]
		Oper_Strazh_Devices_View,

		[DescriptionAttribute("Управление замками")]
		Oper_Strazh_Devices_Control,

		[DescriptionAttribute("Просмотр зон")]
		Oper_Strazh_Zones_View,

		[DescriptionAttribute("Управление замками")]
		Oper_Strazh_Zones_Control,

		[DescriptionAttribute("Просмотр точек доступа")]
		Oper_Strazh_Doors_View,

		[DescriptionAttribute("Управление замками")]
		Oper_Strazh_Doors_Control,

		[DescriptionAttribute("Просмотр сотрудников")]
		Oper_SKD_Employees_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление сотрудника")]
		Oper_SKD_Employees_Edit,

		[DescriptionAttribute("Просмотр посетителей")]
		Oper_SKD_Guests_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление посетителя")]
		Oper_SKD_Guests_Edit,

		[DescriptionAttribute("Просмотр подразделений")]
		Oper_SKD_Departments_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление подразделения")]
		Oper_SKD_Departments_Etit,

		[DescriptionAttribute("Добавление сотрудника в подразделение, удаление сотрудника из подразделения")]
		Oper_SKD_Departments_Employees,

		[DescriptionAttribute("Просмотра должностей")]
		Oper_SKD_Positions_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление должности")]
		Oper_SKD_Positions_Etit,

		[DescriptionAttribute("Просмотр списка дополнительных колонок")]
		Oper_SKD_AdditionalColumns_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление дополнительных колонок")]
		Oper_SKD_AdditionalColumns_Etit,

		[DescriptionAttribute("Просмотр пропусков")]
		Oper_SKD_Cards_View,

		[DescriptionAttribute("Архивирование, восстановление пропусков")]
		Oper_SKD_Cards_Etit,

		[DescriptionAttribute("Просмотр шаблонов доступа")]
		Oper_SKD_AccessTemplates_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление шаблонов доступа")]
		Oper_SKD_AccessTemplates_Etit,

		[DescriptionAttribute("Просмотр шаблонов пропусков")]
		Oper_SKD_PassCards_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление шаблонов пропусков")]
		Oper_SKD_PassCards_Etit,

		[DescriptionAttribute("Просмотр организаций")]
		Oper_SKD_Organisations_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление организации")]
		Oper_SKD_Organisations_Edit,

		[DescriptionAttribute("Привязка пользователей к организации, открепление пользователей от организации")]
		Oper_SKD_Organisations_Users,

		[DescriptionAttribute("Привязка точек доступа к организации, открепление точек доступа от организации")]
		Oper_SKD_Organisations_Doors,

		[DescriptionAttribute("Просмотр дневных графиков")]
		Oper_SKD_TimeTrack_DaySchedules_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление дневных графиков")]
		Oper_SKD_TimeTrack_DaySchedules_Edit,

		[DescriptionAttribute("Просмотр графиков")]
		Oper_SKD_TimeTrack_ScheduleSchemes_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление графиков")]
		Oper_SKD_TimeTrack_ScheduleSchemes_Edit,

		[DescriptionAttribute("Просмотр сокращённых дней")]
		Oper_SKD_TimeTrack_Holidays_View,

		[DescriptionAttribute("Добавление, редактирование, архивирование, восстановление сокращённых дней")]
		Oper_SKD_TimeTrack_Holidays_Edit,

		[DescriptionAttribute("Просмотр графиков работ")]
		Oper_SKD_TimeTrack_Schedules_View,

		[DescriptionAttribute("Создание, редактирование, архивирование, восстановление графиков работ")]
		Oper_SKD_TimeTrack_Schedules_Edit,

		[DescriptionAttribute("Просмотр журнала учета рабочего времени")]
		Oper_SKD_TimeTrack_Report_View,

		[DescriptionAttribute("Ручное редактирование графика проходов")]
		Oper_SKD_TimeTrack_Parts_Edit,

		[DescriptionAttribute("Создание, редактирование, удаление, оправдательных документов")]
		Oper_SKD_TimeTrack_Documents_Edit,

		[DescriptionAttribute("Добавление, удаление, редактирование типа документов в списке 'Типы документов'")]
		Oper_SKD_TimeTrack_DocumentTypes_Edit,
	}
}