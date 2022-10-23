using bomberman.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    public class PlayerSprite : PlayerDecorator
    {
        public PlayerSprite(IPlayerDecorator playerDecorator) 
            : base(playerDecorator)
        {
        }

        public override IPlayerDecorator decorate()
        {
            PlayerModel playerModel = (PlayerModel)base.decorate();

            playerModel.PlayerPictureBox = new PictureBox();
            playerModel.PlayerPictureBox.Size = new Size(Constants.BLOCK_SIZE, Constants.BLOCK_SIZE);
            playerModel.PlayerPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            playerModel.PlayerPictureBox.BackColor = Color.Transparent;
            playerModel.PlayerPictureBox.Image = getPlayerSprite();
            playerModel.PlayerPictureBox.Location = getPosition();

            return playerModel;
        }
    }
}
