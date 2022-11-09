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
            /*var vector2fMockFirst = new Mock<Vector2f>(3, 3);
            var vector2fMockSecond = new Mock<Vector2f>(3, 7);
            var utils = new Mock<Utils>();
            vector2fMockFirst.Setup(x => x.X).Returns(3);
            vector2fMockFirst.Setup(x => x.Y).Returns(3);
            vector2fMockSecond.Setup(x => x.X).Returns(3);
            vector2fMockSecond.Setup(x => x.Y).Returns(7);
            var vector = new Vector2f(6, 10);
            Assert.Equal(vector.X, Utils.AddVectors(vector2fMockFirst, vector2fMockSecond);*/
            var vector1 = new Vector2f(3, 3);
            var vector2 = new Vector2f(3, 7);
            var vector3 = new Vector2f(6, 10);
            Assert.Equal(vector3, Utils.AddVectors(vector1, vector2));
        }
        [Fact]
        public void testMultiplyVectorsReturnsNewCorrectVector()
        {
            var vector1 = new Vector2f(3, 7);
            var vector2 = new Vector2f(9, 21);
            Assert.Equal(vector2, Utils.MultiplyVector(vector1, 3));
        }

    }
}
