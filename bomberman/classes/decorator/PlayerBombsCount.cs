using bomberman.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.decorator
{
    public class PlayerBombsCount : PlayerDecorator
    {
        public PlayerBombsCount(IPlayerDecorator playerDecorator)
            : base(playerDecorator)
        {
        }

        public override IPlayerDecorator decorate()
        {
            PlayerModel playerModel = (PlayerModel)base.decorate();

            playerModel.PlayerBombsPlacedLabel = new Label();
            playerModel.PlayerBombsPlacedLabel.Location = new Point(getPosition().X, getPosition().Y + 18);
            playerModel.PlayerBombsPlacedLabel.Size = new Size(15, 10);
            playerModel.PlayerBombsPlacedLabel.Font = new Font("Arial", 5);
            playerModel.PlayerBombsPlacedLabel.Text = getPlacedBombs().ToString();
            playerModel.PlayerBombsPlacedLabel.Parent = playerModel.PlayerPictureBox;
            playerModel.PlayerBombsPlacedLabel.BackColor = Color.Transparent;

            return playerModel;
        }
    }
}
