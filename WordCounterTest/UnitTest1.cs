using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using txtWordCounter;
using System.IO;
using System.Collections.Generic;

namespace WordCounterTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Arrange
            var rootpath = @"C:\Users\USER\source\repos\txtWordCounter\WordCounterTest\bin\Debug";
            var words = new WordCounter();
            //Act
            words.RecursiveRun(Directory.GetDirectories(rootpath));
            //Assert
            
        }
    }
}
