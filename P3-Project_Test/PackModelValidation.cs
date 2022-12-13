using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Project_Test
{
    public class PackModelValidation
    {

        [Theory]
        [InlineData("23kat")]
        [InlineData("tøj")]
        [InlineData("Værktøj")]
        public void Name_Valid(string value)
        {
            PackModel test = new();
            test.Name = value;
            Assert.True(test.validName());

        }

        [Theory]
        [InlineData("")]
        public void Name_Invalid(string value)
        {
            PackModel test = new();
            test.Name = value;
            Assert.False(test.validName());

        }


        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(100000)]
        [InlineData(652)]
        public void Price_Valid(int value)
        {
            PackModel test = new();
            test.Price = value;
            Assert.True(test.validPrice());

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        [InlineData(-446.52)]
        public void Price_Invalid(int value)
        {
            PackModel test = new();
            test.Price = value;
            Assert.False(test.validPrice());

        }

        [Fact]
        public void Options_Contains_Object_Valid()
        {
            PackModel test = new();
            List<List<int>> list = new();
            list.Add(new List<int>());
            test.Options = list;

            Assert.True(test.validOptions());
        }

        [Fact]
        public void Options_Dosent_Contains_Object_Invalid()
        {
            PackModel test = new();

            Assert.False(test.validOptions());
        }

    }
}
