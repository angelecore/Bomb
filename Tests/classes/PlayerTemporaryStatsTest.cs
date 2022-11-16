using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using bomberman.classes;

namespace Tests.classes
{
    public class PlayerTemporaryStatsTest
    {
        [Fact]
        public void testToStringReturnsRadiusTextIfNoSpeedAmountIsAssigned()
        {
            PlayerTemporaryStats stats = new PlayerTemporaryStats();
            string expectedString = "Effect timer: Always | Add radius";
            Assert.Equal(expectedString, stats.ToString());
        }

        [Fact]
        public void testToStringReturnsSpeedTextIfNoSpeedAmountIsAssigned()
        {
            PlayerTemporaryStats stats = new PlayerTemporaryStats();
            stats.AddSpeedAmount = 1;
            string expectedString = "Effect timer: Always | Add speed";
            Assert.Equal(expectedString, stats.ToString());
        }
    }
}
