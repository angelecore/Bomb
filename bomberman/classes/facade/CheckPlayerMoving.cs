using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.facade
{
    public class CheckPlayerMoving : IValidator
    {
        public bool IsValid(Player player, Vector2f dirVector)
        {
            if (player.Direction != Directions.Idle)
            {
                var currentInvVec = Utils.MultiplyVector(
                    Utils.GetDirectionVector(player.Direction),
                    -1
                );

                // while the player hasnt stopped moving, you can only go backwards.
                if (!currentInvVec.Equals(dirVector))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
