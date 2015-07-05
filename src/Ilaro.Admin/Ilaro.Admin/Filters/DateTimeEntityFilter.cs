using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using Resources;

namespace Ilaro.Admin.Filters
{
    public class DateTimeEntityFilter : IEntityFilter
    {
        public Property Property { get; set; }

        public SelectList Options { get; set; }

        public string Value { get; set; }

        public void Initialize(Property property, string value = "")
        {
            Value = value ?? String.Empty;

            Property = property;

            var options = new Dictionary<string, string>
            {
                { IlaroAdminResources.All, String.Empty },
                { IlaroAdminResources.Today, DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.Yesterday, DateTime.Today.AddDays(-1).ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastWeek, DateTime.Today.AddDays(-7).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastMonth, DateTime.Today.AddMonths(-1).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastQuarter, DateTime.Today.AddMonths(-3).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastHalfAYear, DateTime.Today.AddMonths(-6).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") },
                { IlaroAdminResources.LastYear, DateTime.Today.AddYears(-1).ToString("yyyy.MM.dd") + "-" + DateTime.Today.ToString("yyyy.MM.dd") }
            };

            Options = new SelectList(options, "Value", "Key", Value);
        }

        public string GetSqlCondition(string alias, ref List<object> args)
        {
            if (Value.Contains('-') == false)
            {
                var sql = "({0}[{1}] >= @{2} AND {0}[{1}] <= @{3})".Fill(alias, Property.ColumnName, args.Count, args.Count + 1);
                args.Add(Value + " 00:00");
                args.Add(Value + " 23:59");
                return sql;
            }

            var dates = Value.Split('-');

            if (dates.Length != 2)
                return null;

            if (dates[0].IsNullOrEmpty() == false && dates[1].IsNullOrEmpty() == false)
            {
                var sql = "({0}[{1}] >= @{2} AND {0}[{1}] <= @{3})".Fill(alias, Property.ColumnName, args.Count, args.Count + 1);
                args.Add(dates[0] + " 00:00");
                args.Add(dates[1] + " 23:59");
                return sql;
            }
            if (dates[0].IsNullOrEmpty() && dates[1].IsNullOrEmpty() == false)
            {
                var sql = "{0}[{1}] <= @{2}".Fill(alias, Property.ColumnName, args.Count);
                args.Add(dates[1] + " 23:59");
                return sql;
            }
            if (dates[0].IsNullOrEmpty() == false && dates[1].IsNullOrEmpty())
            {
                var sql = "{0}[{1}] >= @{2}".Fill(alias, Property.ColumnName, args.Count);
                args.Add(dates[0] + " 00:00");
                return sql;
            }

            return null;
        }
    }
}