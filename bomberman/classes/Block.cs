namespace bomberman.classes
{
    public enum BlockType
    {
        InDestructable,
        Destructable,
        Empty,
        Fire,
        Stanby,
        Regenerating
    }

    public class Block
    {
        public Vector2f Position;
        public BlockType Type { get; set; }

        public float regentimer = 0;

        public Block(Vector2f position, BlockType objType)
        {
            this.Position = position;
            this.Type = objType;
        }

    }
}
