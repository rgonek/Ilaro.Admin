﻿using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Configuration;
using Ilaro.Admin.Sample.Northwind.Models;

namespace Ilaro.Admin.Sample.Northwind.Configurators
{
    public class EmployeeConfigurator : EntityConfiguration<Employee>
    {
        public EmployeeConfigurator()
        {
            Group("Employee");

            DisplayProperties(x => x.FirstName, x => x.LastName, x => x.Title,
                x => x.TitleOfCourtesy, x => x.BirthDate, x => x.HireDate,
                x => x.Address, x => x.City, x => x.Region, x => x.PostalCode,
                x => x.Country, x => x.HomePhone, x => x.Extension);

            PropertiesGroup("Main", x => x.FirstName, x => x.LastName,
                x => x.Title, x => x.TitleOfCourtesy, x => x.BirthDate,
                x => x.HireDate, x => x.HomePhone, x => x.Extension, x => x.PhotoPath);
            PropertiesGroup("Address", x => x.Address, x => x.City,
                x => x.Region, x => x.PostalCode, x => x.Country);
            PropertiesGroup("Notes", true, x => x.Notes);
            PropertiesGroup("ManyToMany", x => x.EmployeeTerritories);

            Property(x => x.LastName, x =>
            {
                //x.Required().StringLength(20);
            });

            Property(x => x.FirstName, x =>
            {
                //x.Required().StringLength(10);
            });

            Property(x => x.Title, x =>
            {
                //x.StringLength(30);
            });

            Property(x => x.TitleOfCourtesy, x =>
            {
                //x.StringLength(25);
            });

            Property(x => x.BirthDate, x =>
            {
                x.Type(DataType.DateTime);
            });

            Property(x => x.HireDate, x =>
            {
                x.Format("dd.MM.yyyy");
                x.Template(editor: Templates.Editor.Date);
            });

            Property(x => x.Address, x =>
            {
                //x.StringLength(60);
            });

            Property(x => x.City, x =>
            {
                //x.StringLength(15);
            });

            Property(x => x.Region, x =>
            {
                //x.StringLength(15);
            });

            Property(x => x.PostalCode, x =>
            {
                //x.StringLength(15);
            });

            Property(x => x.Country, x =>
            {
                //x.StringLength(15);
            });

            Property(x => x.HomePhone, x =>
            {
                //x.StringLength(24);
            });

            Property(x => x.Extension, x =>
            {
                //x.StringLength(4);
            });

            Property(x => x.Notes, x =>
            {
                x.Template(editor: Templates.Editor.Html);
            });

            Property(x => x.PhotoPath, x =>
            {
                x.Image("/content/employee", 100, 100);
            });

            Property(x => x.Manager, x =>
            {
                x.ForeignKey("ReportsTo");
            });

            Property(x => x.Orders, x =>
            {
                x.Cascade(CascadeOption.Delete);
            });

            Property(x => x.EmployeeTerritories, x =>
            {
                x.Cascade(CascadeOption.Delete);
                x.ManyToMany();
                x.Display("Territories");
            });
        }
    }
}