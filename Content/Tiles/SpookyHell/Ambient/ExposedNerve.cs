using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

using Spooky.Content.NPCs.Boss.Orroboro;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
    public class ExposedNerve1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(242, 98, 107));
            DustType = DustID.Blood;
            HitSound = SoundID.NPCHit13;
        }

        /*
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                NPC.SpawnOnPlayer(Main.myPlayer, ModContent.NPCType<OrroboroHead>());
            }
        }
        */
    }

    public class ExposedNerve2 : ExposedNerve1
    {
    }
}