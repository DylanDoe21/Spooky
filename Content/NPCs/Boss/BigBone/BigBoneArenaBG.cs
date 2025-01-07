using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    public class BigBoneArenaBG : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private Asset<Texture2D> BGTexture;
        private Asset<Texture2D> BGRootTexture;
        private Asset<Texture2D> BGRootGlowTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 1;
            NPC.height = 1;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			NPC.immortal = true;
			NPC.noGravity = true;
			NPC.hide = true;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
		}

        public override bool CheckActive()
        {
            return false;
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

		public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
            if (!NPC.IsABestiaryIconDummy)
			{
                //wall background
                BGTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/BigBoneArenaBG");
                BGRootTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/BigBoneArenaRoots");
                BGRootGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/BigBoneArenaRootsGlow");

                float XParallax = (Main.LocalPlayer.Center.X - NPC.Center.X) * 0.02f;

                Vector2 DrawPosition = (NPC.Center - new Vector2((1680 / 2), (1102 / 2))) - screenPos;
                Vector2 DrawPositionParallax = (NPC.Center - new Vector2((1680 / 2) + XParallax, (1102 / 2))) - screenPos;
                
                spriteBatch.Draw(BGTexture.Value, DrawPosition, new Rectangle(0, 0, 1680, 1102), new Color(70, 45, 45));
                spriteBatch.Draw(BGTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1680, 1102), new Color(70, 45, 45));
                spriteBatch.Draw(BGRootTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1680, 1102), new Color(75, 50, 50));

                if (NPC.AnyNPCs(ModContent.NPCType<BigBone>()))
                {
                    float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                    Color color = Color.Lerp(Color.Transparent, Color.Gold * 0.15f, time);

                    spriteBatch.Draw(BGRootGlowTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1680, 1102), color);
                }

				int Increment = 20;

                //0 = colored, 1 = white, 2 = retro, 3 = trippy
				switch (Lighting.LegacyEngine.Mode)
				{
					case 0:
					{
                        Increment = 20;
						break;
					}
                    case 1:
					{
                        Increment = 50;
						break;
					}
                    case 2:
					{
                        Increment = 60;
						break;
					}
                    case 3:
					{
                        Increment = 50;
						break;
					}
				}

				for (int X = (int)NPC.Center.X - 700; X <= (int)NPC.Center.X + 700; X += Increment)
                {
                    for (int Y = (int)NPC.Center.Y - 400; Y <= (int)NPC.Center.Y + 600; Y += Increment)
                    {
                        if (!Main.tile[X / 16, Y / 16].HasTile)
                        {
                            Lighting.AddLight(new Vector2(X, Y), new Vector3(0.67f, 0.45f, 0.17f));
                        }
                    }
                }
            }

			return false;
        }

        public override void AI()
        {
            NPC.velocity *= 0;
        }
    }
}