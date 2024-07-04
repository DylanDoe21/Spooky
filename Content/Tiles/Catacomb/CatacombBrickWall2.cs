using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Spooky.Content.NPCs.Boss.BigBone;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombBrickWall2 : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(44, 15, 15));
            DustType = DustID.t_Lihzahrd;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }
    }

    public class CatacombBrickWallBigBoneBG : CatacombBrickWall2
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall2";

        private Asset<Texture2D> BGTexture;
        private Asset<Texture2D> BGRootTexture;
        private Asset<Texture2D> BGRootGlowTexture;

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.05f;
            b = 0.01f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //wall background
            BGTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/BigBoneArenaBG");
            BGRootTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/BigBoneArenaRoots");
            BGRootGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/BigBoneArenaRootsGlow");

            float XParallax = (Main.LocalPlayer.Center.X / 16 - i) * 0.02f;
            float YParallax = (Main.LocalPlayer.Center.Y / 16 - j) * 0.02f;

            Vector2 DrawPosition = (new Vector2(i, j) - new Vector2((1680 / 2) / 16, (1102 / 2) / 16) + TileOffset) * 16 - Main.screenPosition;
            Vector2 DrawPositionParallax = (new Vector2(i, j) - new Vector2((1680 / 2) / 16 + XParallax, (1102 / 2) / 16 + YParallax) + TileOffset) * 16 - Main.screenPosition;
            
            spriteBatch.Draw(BGTexture.Value, DrawPosition, new Rectangle(0, 0, 1680, 1102), new Color(70, 45, 45));
            spriteBatch.Draw(BGTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1680, 1102), new Color(70, 45, 45));
            spriteBatch.Draw(BGRootTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1680, 1102), new Color(75, 50, 50));

            if (NPC.AnyNPCs(ModContent.NPCType<BigBone>()))
            {
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                Color color = Color.Lerp(Color.Transparent, Color.Gold * 0.15f, time);

                spriteBatch.Draw(BGRootGlowTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1680, 1102), color);
            }

            for (int X = i * 16 - 700; X <= i * 16 + 700; X += 20)
            {
                for (int Y = j * 16 - 400; Y <= j * 16 + 600; Y += 20)
                {
                    if (!Main.tile[X / 16, Y / 16].HasTile)
                    {
                        Lighting.AddLight(new Vector2(X, Y), new Vector3(0.67f, 0.45f, 0.17f));
                    }
                }
            }
        }
    }

    public class CatacombBrickWall2Safe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall2";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(44, 15, 15));
            DustType = DustID.t_Lihzahrd;
        }
    }
}