using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class SpookyMushWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(63, 31, 110));
            DustType = ModContent.DustType<SpookyHellPurpleDust>();
            HitSound = SoundID.Dig;
        }
    }

    public class SpookyMushWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/SpookyHell/SpookyMushWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(63, 31, 110));
            DustType = ModContent.DustType<SpookyHellPurpleDust>();
            HitSound = SoundID.Dig;
        }
    }
}