using P3_Project.Models.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace P3_Project_Integration
{
    public class ItemModel_Database_Test
    {

        StorageDB db = new StorageDB("Data Source=LENOVO-PC\\ASH;Initial Catalog=model;Integrated Security=True;MultipleActiveResultSets=True");


        [Theory]
        [MemberData(nameof(ItemModelIndividualValidData.SplitCountData), MemberType = typeof(ItemModelIndividualValidData))]
        //[ClassData(typeof(CreateItemModelValidData))]
        public void Create_ItemModel_Valid(ItemModel model)
        {
            db.DB.CreateTable("test1", new ItemModel());
            //foreach(ItemModel model in models)
            //{
            //    db.DB.AddRowToTable("test", model);
            //}
            db.DB.AddRowToTable("test1", model);

            ItemModel loadedModel = db.DB.GetRow("test1", model, "1");



            Assert.Equal(model, loadedModel);
            db.DB.DeleteTable("test1");
        }

        [Theory]
        [ClassData(typeof(ItemModelListValidData))]
        public void Create_ItemModels_valid(List<ItemModel> modelList)
        {
            db.DB.CreateTable("test2", new ItemModel());
            foreach (ItemModel model in modelList)
            {
                db.DB.AddRowToTable("test2", model);
            }

            List<ItemModel> loadedModel = db.DB.GetAllElements("test2", new ItemModel());

            bool checkValue = true;
            foreach (ItemModel model in modelList)
            {
                if (!model.Equals(loadedModel[model.Id - 1]))
                    checkValue = false;
            }

            db.DB.DeleteTable("test2");
            Assert.True(checkValue);
        }


        [Theory]
        [ClassData(typeof(ItemModelListValidData))]
        public void Check_For_Element_ItemModels_valid(List<ItemModel> modelList)
        {
            db.DB.CreateTable("test3", new ItemModel());
            foreach (ItemModel model in modelList)
            {
                db.DB.AddRowToTable("test3", model);
            }

            Random rnd = new Random();
            int index = rnd.Next(modelList.Count()-1);



            Assert.True(db.DB.CheckRow("test3", "ModelName", modelList[index].ModelName));
            db.DB.DeleteTable("test3");
        }

        [Theory]
        [MemberData(nameof(ItemModelIndividualValidData.SplitCountData), MemberType = typeof(ItemModelIndividualValidData))]
        //[ClassData(typeof(CreateItemModelValidData))]
        public void Update_ItemModel_Valid(ItemModel model)
        {
            db.DB.CreateTable("test4", new ItemModel());
            db.DB.AddRowToTable("test4", model);

            Random rnd = new Random();
            int value = rnd.Next();

            db.DB.UpdateField("test4", "Id", "1", "ModelPrice", value.ToString());

            ItemModel loadedModel = db.DB.GetRow("test4", model, "1");



            Assert.True(loadedModel.ModelPrice == value);
            db.DB.DeleteTable("test4");
        }


        [Theory]
        [MemberData(nameof(ItemModelIndividualValidData.SplitCountData), MemberType = typeof(ItemModelIndividualValidData))]
        //[ClassData(typeof(CreateItemModelValidData))]
        public void Delete_ItemModel_Valid(ItemModel model)
        {
            db.DB.CreateTable("test5", new ItemModel());

            db.DB.AddRowToTable("test5", model);

            db.DB.RemoveRow("test5", "ModelName", model.ModelName);



            Assert.False(db.DB.CheckRow("test5","ModelName",model.ModelName));
            db.DB.DeleteTable("test5");
        }

    }


    public class ItemModelIndividualValidData : IEnumerable<object[]>
    {
        public static IEnumerable<object[]> SplitCountData =>
            new List<object[]>
            {
            new object[]
            {
                new ItemModel(){
                    ModelName= "T-shirt",
                    ModelPrice=36000000,
                    Id = 1,
                    Type = "Tøj",
                },
            },
            new object[]
            {
                new ItemModel()
                {
                    ModelName= "T-shirt2",
                    Id = 1,
                    ModelPrice=3650000,
                    Type = "Tøj",
                }
            }
            };

        public IEnumerator<object[]> GetEnumerator()
        {
            return SplitCountData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)SplitCountData).GetEnumerator();
        }


        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ItemModelListValidData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                new List<ItemModel>()
                {
                    new ItemModel(){
                        ModelName= "T-shirt",
                        ModelPrice=36000000,
                        Id = 1,
                        Type = "Tøj",
                    },
                    new ItemModel()
                    {
                        ModelName= "T-shirt2",
                        Id = 2,
                        ModelPrice=3650000,
                        Type = "Tøj",
                    },
                    new ItemModel()
                    {
                        ModelName= "Pants",
                        Id = 3,
                        ModelPrice=3600,
                        Type = "Tilbehør",
                    },
                    new ItemModel()
                    {
                        ModelName= "Hoodienie",
                        Id = 4,
                        ModelPrice=3,
                        Type = "Tøj",
                    }
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

   