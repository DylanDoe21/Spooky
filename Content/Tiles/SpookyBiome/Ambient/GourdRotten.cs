using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class GourdRotten : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(120, 96, 62));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            //rot gourd material, only if rot gourd has been defeated
            if (Main.rand.NextBool() && Flags.downedRotGourd)
            {
                int randomAmount = Main.rand.Next(2, 6);

                for (int numItems = 0; numItems < randomAmount; numItems++) 
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<RottenChunk>());
                }
            }

            //rot gourd summon
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<RottenSeed>());
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            bool isPlayerNear = WorldGen.PlayerLOS(i, j);

            if (!Main.gamePaused && Main.instance.IsActive && isPlayerNear)
            {
                if (Main.rand.NextBool(300))
                {
                    Projectile.NewProjectile(null, new Vector2(i * 16, j * 16), Vector2.Zero, ModContent.ProjectileType<RottenGourdFly>(), 0, 0f, Main.myPlayer);
                }
            }
        }
    }
}