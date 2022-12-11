namespace bomberman.classes
{
    public enum BlockType
    {
        InDestructable,
        Destructable,
        Empty,
        Fire,
        Regenerating,

    }

    public class Block
    {
        public Vector2f Position;
        public BlockType Type { get; set; }

        public Block(Vector2f position, BlockType objType)
        {
            this.Position = position;
            this.Type = objType;
        }

    }
}
