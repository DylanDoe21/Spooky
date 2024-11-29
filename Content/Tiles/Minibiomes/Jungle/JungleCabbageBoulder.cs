using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Jungle
{
    public class JungleCabbageBoulder : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(52, 135, 46));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            int SpawnX = i * 16 + 22;
            int SpawnY = j * 16 + 24;

            int Damage = Main.masterMode ? 150 / 3 : (Main.expertMode ? 150 / 2 : 150);

            Projectile.NewProjectile(new EntitySource_TileBreak(i * 16, j * 16), SpawnX, SpawnY, 0, -1, ModContent.ProjectileType<JungleCabbageBoulderProj>(), Damage, 10, Main.myPlayer);
        }
    }
}