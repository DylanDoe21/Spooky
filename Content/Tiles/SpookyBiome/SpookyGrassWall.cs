using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class SpookyGrassWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(151, 65, 10));
            DustType = ModContent.DustType<HalloweenGrassDust>();
            HitSound = SoundID.Grass;
        }
    }
}