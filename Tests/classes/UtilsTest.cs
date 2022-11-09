using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using bomberman.classes;
using Moq;

namespace Tests.classes
{
    public class UtilsTest
    {
        [Fact]
        public void testAddVectorsReturnsNewCorrectVector()
        {
            var vector2fMockFirst = new Mock<Vector2f>(3, 3);
            var vector2fMockSecond = new Mock<Vector2f>(3, 7);
            var vector = new Vector2f(6, 10);
            Assert.Equal(vector, Utils.AddVectors(vector2fMockFirst.Object, vector2fMockSecond.Object));
        }
        [Fact]
        public void testMultiplyVectorsReturnsNewCorrectVector()
        {
            var vector1Mock = new Mock<Vector2f>(3, 7);
            var vector2 = new Vector2f(9, 21);
            Assert.Equal(vector2, Utils.MultiplyVector(vector1Mock.Object, 3));
        }

    }
}
