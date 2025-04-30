using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Desert
{
    public class DesertSandWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(112, 74, 41));
            DustType = DustID.Sand;
        }
    }

    public class DesertSandWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Desert/DesertSandWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(112, 74, 41));
            DustType = DustID.Sand;
        }
	}
}