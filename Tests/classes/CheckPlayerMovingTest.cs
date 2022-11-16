using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using bomberman.classes.facade;
using Moq;
using bomberman.classes;

namespace Tests.classes
{
    public class CheckPlayerMovingTest
    {
        [Fact]
        public void testIsValidReturnsTrueOnIdleDirections()
        {
            CheckPlayerMoving checkPlayerMoving = new CheckPlayerMoving();
            var vector2fMock = new Mock<Vector2f>(3, 3);
            var playerMock = new Mock<Player>("1", "name", vector2fMock.Object);
            Assert.True(checkPlayerMoving.IsValid(playerMock.Object, vector2fMock.Object));
        }

        [Fact]
        public void testIsValidReturnsFalseOnNonEqualDirections()
        {
            CheckPlayerMoving checkPlayerMoving = new CheckPlayerMoving();
            var vector2fMock = new Mock<Vector2f>(3, 3);
            var playerMock = new Mock<Player>("1", "name", vector2fMock.Object);
            playerMock.Object.Direction = Directions.Right;
            Assert.False(checkPlayerMoving.IsValid(playerMock.Object, vector2fMock.Object));
        }
    }
}
