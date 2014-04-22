﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public class AdditionalColumnTypeTranslator : OrganisationElementTranslator<DataAccess.AdditionalColumnType, AdditionalColumnType, AdditionalColumnTypeFilter>
	{
		public AdditionalColumnTypeTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override OperationResult CanSave(AdditionalColumnType item)
		{
			bool sameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (sameName)
				return new OperationResult("Тип колонки с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override AdditionalColumnType Translate(DataAccess.AdditionalColumnType tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.DataType = (AdditionalColumnDataType)tableItem.DataType;
			result.PersonType = (PersonType)tableItem.PersonType;
			result.IsInGrid = tableItem.IsInGrid;
			return result;
		}

		ShortAdditionalColumnType TranslateToShort(DataAccess.AdditionalColumnType tableItem)
		{
			return new ShortAdditionalColumnType
			{
				UID = tableItem.UID,
				DataType = (AdditionalColumnDataType)tableItem.DataType,
				Description = tableItem.Description,
				Name = tableItem.Name,
				OrganisationUID = tableItem.OrganisationUID,
				IsInGrid = tableItem.IsInGrid
			};
		}

		protected override void TranslateBack(DataAccess.AdditionalColumnType tableItem, AdditionalColumnType apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.DataType = (int?)apiItem.DataType;
			tableItem.PersonType = (int)apiItem.PersonType;
			tableItem.IsInGrid = apiItem.IsInGrid;
		}

		public AdditionalColumnType Get(Guid? uid)
		{
			if (uid == null)
				return null;
			try
			{
				var result = Table.Where(x => x != null &&
					!x.IsDeleted &&
					x.UID == uid.Value).FirstOrDefault();
				return Translate(result);
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public OperationResult<IEnumerable<ShortAdditionalColumnType>> GetList(AdditionalColumnTypeFilter filter)
		{
			try
			{
				var result = new List<ShortAdditionalColumnType>();
				foreach (var tableItem in GetTableItems(filter))
				{
					var shortPosition = TranslateToShort(tableItem);
					result.Add(shortPosition);
				}
				var operationResult = new OperationResult<IEnumerable<ShortAdditionalColumnType>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<ShortAdditionalColumnType>>(e.Message);
			}
		}

		public List<Guid> GetTextColumnTypes()
		{
			return Table.Where(x => x.DataType == (int?)AdditionalColumnDataType.Text && x.IsInGrid).Select(x => x.UID).ToList();
		}

		protected override Expression<Func<DataAccess.AdditionalColumnType, bool>> IsInFilter(AdditionalColumnTypeFilter filter)
		{
			var result = base.IsInFilter(filter);
			var names = filter.Names;
			if (names != null && names.Count != 0)
				result = result.And(e => names.Contains(e.Name));
			if (filter.Type.HasValue)
			{
				var dataType = (int)filter.Type.Value;
				result = result.And(e => e.DataType == dataType);
			}
			if (filter.PersonType != null)
			{
				result = result.And(e => e.PersonType == (int?)filter.PersonType);
			}
			return result;
		}
	}
}