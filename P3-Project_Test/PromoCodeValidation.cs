using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Project_Test
{
    public class PromoCodeValidation
    {

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(100000)]
        [InlineData(652)]
        public void Values_Type_Fixed_Valid(int value)
        {
            PromoCode test = new();
            test.DiscountType = PromoCodeDiscountType.Fixed;
            test.Value = value;
            Assert.True(test.validValue());

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        [InlineData(-100)]
        [InlineData(-100000)]
        [InlineData(0)]
        public void Values_Type_Fixed_Invalid(int value)
        {
            PromoCode test = new();
            test.DiscountType = PromoCodeDiscountType.Fixed;
            test.Value = value;
            Assert.False(test.validValue());

        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(33)]
        [InlineData(88)]
        [InlineData(100)]
        public void Values_Type_Percentage_Valid(int value)
        {
            PromoCode test = new();
            test.DiscountType = PromoCodeDiscountType.Percentage;
            test.Value = value;
            Assert.True(test.validValue());

        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(-33)]
        [InlineData(-88)]
        [InlineData(101)]
        public void Values_Type_Percentage_Invalid(int value)
        {
            PromoCode test = new();
            test.DiscountType = PromoCodeDiscountType.Percentage;
            test.Value = value;
            Assert.False(test.validValue());

        }

        [Fact]
        public void Code_Valid()
        {
            PromoCode test = new();

            test.Code = "not empty";

            Assert.True(test.validCode());
        }

        [Fact]
        public void Code_Invalid()
        {
            PromoCode test = new();

            test.Code = "";

            Assert.False(test.validCode());
        }

        [Theory]
        [InlineData(PromoCodeDiscountType.Percentage)]
        [InlineData(PromoCodeDiscountType.Fixed)]
        public void DiscountType_Valid(PromoCodeDiscountType value)
        {
            PromoCode test = new();
            test.DiscountType = value;
            Assert.True(test.validDiscountType());

        }
        [Theory]
        [InlineData((PromoCodeDiscountType) (-1))]
        [InlineData((PromoCodeDiscountType)5)]
        public void DiscountType_Invalid(PromoCodeDiscountType value)
        {
            PromoCode test = new();
            test.DiscountType = value;
            Assert.False(test.validDiscountType());

        }


        [Theory]
        [InlineData(PromoCodeItemType.Some)]
        [InlineData(PromoCodeItemType.All)]
        [InlineData(PromoCodeItemType.AllItems)]
        [InlineData(PromoCodeItemType.AllPacks)]
        public void ItemType_Valid(PromoCodeItemType value)
        {
            PromoCode test = new();
            test.ItemType = value;
            Assert.True(test.validItemType());

        }
        [Theory]
        [InlineData((PromoCodeItemType)(-1))]
        [InlineData((PromoCodeItemType)5)]
        public void ItemType_Invalid(PromoCodeItemType value)
        {
            PromoCode test = new();
            test.ItemType = value;
            Assert.False(test.validItemType());

        }

        [Fact]
        public void Items_Valid()
        {
            PromoCode test = new();
            List<PromoCodeSomeItemType> list = new();
            list.Add(new PromoCodeSomeItemType());
            test.Items = list;
            test.ItemType = PromoCodeItemType.Some;
            Assert.True(test.validItems());
        }

        [Theory]
        [InlineData(PromoCodeItemType.All)]
        [InlineData(PromoCodeItemType.AllItems)]
        [InlineData(PromoCodeItemType.AllPacks)]
        public void Items_Incorrect_ItemType_Invalid(PromoCodeItemType type)
        {
            PromoCode test = new();
            List<PromoCodeSomeItemType> list = new();
            list.Add(new PromoCodeSomeItemType());
            test.Items = list;
            test.ItemType = type;
            Assert.False(test.validItems());

        }

        [Fact]
        public void Items_Correct_ItemType_Empty_List_Invalid()
        {
            PromoCode test = new();
            test.ItemType = PromoCodeItemType.Some;
            Assert.False(test.validItems());

        }
    }
}
