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
        public void testToEqualsReturnsFalseOnWrongType()
        {
            var vector2fMock = new Mock<Vector2f>(3, 3);
            var realVector = new Vector2f(3, 3);
            Assert.False(realVector.Equals(vector2fMock.Object));
        }
        [Fact]
        public void testToEqualsReturnsFalseOnNull()
        {
            var realVector = new Vector2f(3, 3);
            Assert.False(realVector.Equals(null));
        }

        [Fact]
        public void testToEqualsReturnsFalseOnNonIdenticalCoordinates()
        {
            var vector1 = new Vector2f(3, 3);
            var vector2 = new Vector2f(4, 5);
            Assert.False(vector1.Equals(vector2));
        }

        [Fact]
        public void testToEqualsReturnsTrueOnIdenticalCoordinates()
        {
            var vector1 = new Vector2f(3, 3);
            var vector2 = new Vector2f(3, 3);
            Assert.True(vector1.Equals(vector2));
        }
    }
}
