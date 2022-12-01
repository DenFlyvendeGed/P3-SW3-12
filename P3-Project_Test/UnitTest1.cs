using System.Xml.Linq;
using Xunit.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;


namespace P3_Project_Test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void Test1()
        {
            var temp = "my class!";
            output.WriteLine("This is output from {0}", temp);
        }

        [Fact]
        public void Test2()
        {
            var temp = "my class2!";
            output.WriteLine("This is output from {0}", temp);
        }
        [Fact]
        public void Test3()
        {
            var temp = "my class3!";
            output.WriteLine("This is output from {0}", temp);
        }
    }


    
}