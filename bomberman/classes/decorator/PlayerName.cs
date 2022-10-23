using bomberman.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes.decorator
{
    public class PlayerName : PlayerDecorator
    {
        public PlayerName(IPlayerDecorator playerDecorator)
            : base(playerDecorator)
        {
        }

        public override IPlayerDecorator decorate()
        {
            PlayerModel playerModel = (PlayerModel)base.decorate();

            playerModel.PlayerNameLabel = new Label();
            playerModel.PlayerNameLabel.Location = getPosition();
            playerModel.PlayerNameLabel.Size = new Size(Constants.BLOCK_SIZE, 8);
            playerModel.PlayerNameLabel.Font = new Font("Arial", 5);
            playerModel.PlayerNameLabel.Text = getName();
            playerModel.PlayerNameLabel.Parent = playerModel.PlayerPictureBox;
            playerModel.PlayerNameLabel.BackColor = Color.Transparent;

            return playerModel;
        }
    }
}
