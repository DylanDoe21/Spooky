using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class TarBlobSmallClub : ModNPC
    {
        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/TarBlobSmallClubBestiary",
                Position = new Vector2(-25f, 25f),
                PortraitPositionXOverride = 12f,
                PortraitPositionYOverride = 13.5f
            };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 90;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 46;
			NPC.height = 46;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarBlobSmallClub"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[0]];

			if (Parent.active && Parent.type == ModContent.NPCType<TarBlobSmall>())
            {
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/TarPits/TarBlobSmallChain");

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NPC.Center;
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter;
				Vector2 p3 = myCenter;

				int segments = 12;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = (drawPosNext - drawPos2);
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					drawColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, NPC.GetAlpha(drawColor), rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

            //draw the npc itself
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}
        
        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];

            Vector2 vector92 = new Vector2(NPC.Center.X, NPC.Center.Y);
            float num740 = Parent.Center.X - vector92.X;
            float num741 = Parent.Center.Y - vector92.Y;
            NPC.rotation = (float)Math.Atan2((double)num741, (double)num740) + 4.71f;

            if (!Parent.active || Parent.type != ModContent.NPCType<TarBlobSmall>())
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TarBlobSmallClubGore").Type);
                }

                NPC.active = false;
            }

            NPC.ai[3] += NPC.ai[1]; //NPC.ai[1] is passed down from the parent as the rotation speed for the club
            
            //adjust the distance of the club if the player is too close to the tar blob parent
            int distance = (int)NPC.ai[2]; //NPC.ai[2] is passed down from the parent as the maximum distance the club can be away from the parent
            if (player.Distance(Parent.Center) < NPC.ai[2])
            {
                distance = (int)player.Distance(Parent.Center);
            }
            else
            {
                distance = (int)NPC.ai[2];
            }

            double rad = NPC.ai[3] * (Math.PI / 180);
            NPC.position.X = Parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = Parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;
        }
    }
}