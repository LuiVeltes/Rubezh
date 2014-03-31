﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public abstract class IsDeletedTranslator<TableT, ApiT, FilterT> : TranslatorBase<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IIsDeletedDatabaseElement, new()
		where ApiT : SKDIsDeletedModel, new()
		where FilterT : IsDeletedFilter
	{
		public IsDeletedTranslator(DataAccess.SKDDataContext context) : base(context) { }

		protected override ApiT Translate(TableT tableItem)
		{
			return TranslateIsDeleted<ApiT, TableT>(tableItem);
		}
		protected override void TranslateBack(TableT tableItem, ApiT apiItem)
		{
			TranslateBackIsDeleted<ApiT, TableT>(apiItem, tableItem);
		}
		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			result = result.And(base.IsInFilter(filter));
			var IsDeletedExpression = PredicateBuilder.True<TableT>();
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					IsDeletedExpression = e => e.IsDeleted;
					break;
				case DeletedType.Not:
					IsDeletedExpression = e => !e.IsDeleted;
					break;
				default:
					break;
			}
			result = result.And(IsDeletedExpression);
			return result;
		}

		public virtual OperationResult MarkDeleted(IEnumerable<ApiT> items)
		{
			var operationResult = new OperationResult();
			try
			{
				foreach (var item in items)
				{
					var verifyResult = CanDelete(item);
					if (verifyResult.HasError)
						return verifyResult;
					if (item != null)
					{
						var databaseItem = (from x in Table where x.UID.Equals(item.UID) select x).FirstOrDefault();
						if (databaseItem != null)
						{
							databaseItem.IsDeleted = true;
							databaseItem.RemovalDate = DateTime.Now;
						}
					}
				}
				Table.Context.SubmitChanges();
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public static Expression<Func<TableT, bool>> IsInDeleted(FilterT filter)
		{
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					return e => e != null && e.IsDeleted;
				case DeletedType.Not:
					return e => e != null && !e.IsDeleted;
				default:
					return e => true;
			}
		}

		protected static ApiType TranslateIsDeleted<ApiType, TableType>(TableType tableItem)
			where ApiType: SKDIsDeletedModel, new()
			where TableType : DataAccess.IIsDeletedDatabaseElement
		{
			var result = TranslateBase<ApiType, TableType>(tableItem);
			result.IsDeleted = tableItem.IsDeleted;
			result.RemovalDate = CheckDate(tableItem.RemovalDate);
			return result;
		}

		protected static void TranslateBackIsDeleted<ApiType, TableType>(ApiType apiItem, TableType tableItem)
			where ApiType: SKDIsDeletedModel, new()
			where TableType : DataAccess.IIsDeletedDatabaseElement
		{
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = CheckDate(apiItem.RemovalDate);
		}
	}
}
