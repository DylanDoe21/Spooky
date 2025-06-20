using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
    public class OceanBiomassWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(18, 56, 23));
            DustType = DustID.Blood;
			HitSound = SoundID.NPCDeath1;
        }
    }

    public class OceanBiomassWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Ocean/OceanBiomassWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(18, 56, 23));
            DustType = DustID.Blood;
			HitSound = SoundID.NPCDeath1;
        }
    }
}