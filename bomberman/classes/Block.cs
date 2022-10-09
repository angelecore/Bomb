namespace bomberman.classes
{
    public enum BlockType
    {
        InDestructable,
        Destructable,
        Empty
    }
    internal class Block
    {
        public Control BlockObj { get; set; }
        public BlockType Type { get; set; }

        public Block(Control blockObj, BlockType objType)
        {
            this.BlockObj = blockObj;
            this.Type = objType;
        }

    }
}
