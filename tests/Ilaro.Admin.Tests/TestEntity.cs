using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Tests
{
    public class TestEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsSpecial { get; set; }

        public DateTime DateAdd { get; set; }

        public decimal Price { get; set; }

        public double Percent { get; set; }

        public IList<string> Tags { get; set; }

        public IList<int> Dimensions { get; set; }

        public IList<TestEntity> Siblings { get; set; }

        public TestEntity Parent { get; set; }

        public int ParentId { get; set; }

        [ForeignKey("ChildId")]
        public TestEntity Child { get; set; }

        public int ChildId { get; set; }

        [ForeignKey("EntityName")]
        public int RoleId { get; set; }

        public TestEnum Option { get; set; }

        public StringSplitOptions SplitOption { get; set; }
    }

    public enum TestEnum
    {
        Option1,
        Option2
    }
}
