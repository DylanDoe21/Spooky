using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasWoodWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(108, 50, 21));
            DustType = DustID.WoodFurniture;
        }
    }

    public class ChristmasWoodWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasWoodWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(108, 50, 21));
            DustType = DustID.WoodFurniture;
        }
    }
}