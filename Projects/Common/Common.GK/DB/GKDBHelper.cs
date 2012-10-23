﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common.GK
{
    public static class GKDBHelper
    {
        public static void Add()
        {
            try
            {
                using (var dataContext = ConnectionManager.CreateGKDataContext())
                {
                    var journal = new Journal();
                    journal.DateTime = DateTime.Now;
                    journal.ObjectUID = Guid.NewGuid();
                    journal.GKObjectNo = 1;
                    journal.Description = "Event Description";
                    dataContext.Journal.InsertOnSubmit(journal);
                    dataContext.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "GKDBHelper.Add");
            }
        }

        public static void Select()
        {
            try
            {
                using (var dataContext = ConnectionManager.CreateGKDataContext())
                {
                    var query = "SELECT * FROM Journal";
                    var result = dataContext.ExecuteQuery<Journal>(query);
                    var journalRecordsCount = result.Count();
                    Trace.WriteLine("Journal Count = " + journalRecordsCount.ToString());
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "GKDBHelper.Select");
            }
        }
    }
}