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

        public void ChangeState(BlockType state)
        {
            switch(this.Type)
            {
                case BlockType.InDestructable:
                    return;
                case BlockType.Destructable:
                    if (state == BlockType.Fire || state == BlockType.Empty)
                    { this.Type = state;}
                    return;
                case BlockType.Empty: 
                    if(state == BlockType.Fire || state == BlockType.Regenerating)
                    { this.Type = state; }
                    return;
                case BlockType.Fire:
                    if (state == BlockType.Empty)
                    { this.Type = state; }
                    return;
                case BlockType.Regenerating:
                    if (state == BlockType.Fire || state == BlockType.Empty)
                    { this.Type = state; }
                    return;
                    
                default: return;

            }
        }

    }
}
