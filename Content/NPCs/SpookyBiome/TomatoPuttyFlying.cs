using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class TomatoPuttyFlying : ModNPC
    {
        float MoveSpeedX = 0;
		float MoveSpeedY = 0;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 320;
            NPC.damage = 50;
            NPC.defense = 15;
            NPC.width = 50;
            NPC.height = 60;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoPuttyFlying"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture + "Body");

			float stretch = -NPC.velocity.Y * 0.025f;

			stretch = Math.Abs(stretch);

			//limit how much he can stretch
			if (stretch > 0.3f)
			{
				stretch = 0.3f;
			}

			//limit how much he can squish
			if (stretch < -0.3f)
			{
				stretch = -0.3f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			
			if (NPC.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}
			if (NPC.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw npc manually for stretching
			spriteBatch.Draw(NPCTexture.Value, new Vector2(NPC.Center.X, NPC.Center.Y - 28) - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, 0), scaleStretch, effects, 0f);

			return true;
		}

        public override void FindFrame(int frameHeight)
        {
            //flying animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 1)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.04f;

            float MaxSpeedX = 1;
            float MaxSpeedY = 4;

            //flies to players X position
            if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeedX) 
            {
                MoveSpeedX -= 0.5f;
            }
            else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeedX)
            {
                MoveSpeedX += 0.5f;
            }

            NPC.velocity.X += MoveSpeedX * 0.01f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeedX, MaxSpeedX);
            
            //flies to players Y position
            if (NPC.Center.Y >= player.Center.Y - 100 && MoveSpeedY >= -MaxSpeedY)
            {
                MoveSpeedY -= 0.5f;
            }
            else if (NPC.Center.Y <= player.Center.Y - 100 && MoveSpeedY <= MaxSpeedY)
            {
                MoveSpeedY += 0.5f;
            }

            NPC.velocity.Y += MoveSpeedY * 0.1f;
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeedY, MaxSpeedY);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 2, 5));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(0, Main.rand.Next(-15, -9)), ModContent.ProjectileType<TomatoPuttyFlyingFalling>(), NPC.damage, 4.5f);

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TomatoPuttyFlyingGore").Type);
                    }
                }
            }
        }
    }
}