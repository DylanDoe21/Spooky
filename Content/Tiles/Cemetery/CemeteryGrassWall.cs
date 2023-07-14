using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Cemetery
{
    public class CemeteryGrassWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(12, 62, 6));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }
    }
}