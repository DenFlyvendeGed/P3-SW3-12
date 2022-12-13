using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Project_Test
{
    public class ItemModelValidation
    {

        //ModelPrice
        //StockAlarm
        //Type
        //ModelName

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(100000)]
        [InlineData(652)]
        public void ModelPrice_Valid(int value)
        {
            ItemModel test = new();
            test.ModelPrice = value;
            Assert.True(test.validModelPrice());

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        [InlineData(-446.52)]
        public void ModelPrice_Invalid(int value)
        {
            ItemModel test = new();
            test.ModelPrice = value;
            Assert.False(test.validModelPrice());

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        [InlineData(69)]
        public void StockAlarm_Valid(int value)
        {
            ItemModel test = new();
            test.StockAlarm = value;
            Assert.True(test.validStockAlarm());

        }

        [Theory]
        [InlineData(-12)]
        [InlineData(-5)]
        [InlineData(-42)]
        public void StockAlarm_Invalid(int value)
        {
            ItemModel test = new();
            test.StockAlarm = value;
            Assert.False(test.validStockAlarm());

        }

        [Theory]
        [InlineData("Tøj")]
        [InlineData("Tilbehør")]
        public void Type_Valid(string value)
        {
            ItemModel test = new();
            test.Type = value;
            Assert.True(test.validType());

        }

        [Theory]
        [InlineData("")]
        [InlineData("tøj")]
        [InlineData("Værktøj")]
        public void Type_Invalid(string value)
        {
            ItemModel test = new();
            test.Type = value;
            Assert.False(test.validType());

        }

        [Theory]
        [InlineData("23kat")]
        [InlineData("tøj")]
        [InlineData("Værktøj")]
        public void ModelName_Valid(string value)
        {
            ItemModel test = new();
            test.ModelName = value;
            Assert.True(test.validModelName());

        }

        [Theory]
        [InlineData("")]
        public void ModelName_Invalid(string value)
        {
            ItemModel test = new();
            test.ModelName = value;
            Assert.False(test.validModelName());

        }


    }
}
