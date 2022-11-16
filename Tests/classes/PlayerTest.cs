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
    public class PlayerTest
    {
        [Fact]
        public void testUpdateTemporaryStatsDoesNotRemoveIfNotFinished()
        {
            Player player = new Player("1", "name", new Vector2f(3, 3));
            PlayerTemporaryStats playerTemporaryStat = new PlayerTemporaryStats();
            playerTemporaryStat.ActiveTimer = null;
            playerTemporaryStat.Applied = true;
            player.TemporaryStats.Add(playerTemporaryStat);
            var playerStatsCount = player.TemporaryStats.Count;
            player.UpdateTemporaryStats((float)3.33);
            Assert.Equal(playerStatsCount, player.TemporaryStats.Count);
        }

        [Fact]
        public void testUpdateTemporaryStatsDoesRemoveStatIfFinished()
        {
            Player player = new Player("1", "name", new Vector2f(3, 3));
            PlayerTemporaryStats playerTemporaryStat = new PlayerTemporaryStats();
            playerTemporaryStat.ActiveTimer = (float)0.8;
            playerTemporaryStat.Applied = false;
            player.TemporaryStats.Add(playerTemporaryStat);
            var playerStatsCount = player.TemporaryStats.Count;
            player.UpdateTemporaryStats((float)3.33);
            Assert.NotEqual(playerStatsCount, player.TemporaryStats.Count);
        }
    }
}
