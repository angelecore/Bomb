using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using bomberman.classes;

namespace Tests.classes
{
    public class Vector2fTest
    {
        [Fact]
        public void testToStringReturnsFormattedData()
        {
            var vector2fMock = new Mock<Vector2f>(3, 3);
            var realVector = new Vector2f(3, 3);
            //vector2fMock.Setup(x => x.GetType()).Returns(realVector.GetType());
            Assert.True(realVector.Equals(vector2fMock));
        }

        [Fact]
        public void testEqualsReturnsFalse()
        {
            var vector2fMock = new Mock<Vector2f>(3, 3);
            var realVector = new Vector2f(5, 5);
            //vector2fMock.Setup(x => x.Equals(secondVector2fMock)).Returns(true);
            Assert.True(realVector.Equals(vector2fMock));
        }

    }
}
