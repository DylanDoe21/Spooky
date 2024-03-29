using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class LivingFleshWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(78, 26, 21));
            DustType = DustID.Blood;
            HitSound = SoundID.NPCHit20;
        }
    }
}