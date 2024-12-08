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
}