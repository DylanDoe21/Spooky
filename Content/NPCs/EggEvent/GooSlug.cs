using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using Spooky.Core;

using Spooky.Content.Items.SpookyHell.EggEvent;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.NPCs.EggEvent
{
    public class GooSlug : ModNPC  
    {
        int CurrentFrameX = 0; //0 = moving  1 = eat
		int SaveDirection;

		private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle ChompSound = new("Spooky/Content/Sounds/EggEvent/GooSlugChomp", SoundType.Sound) { Volume = 0.4f, PitchVariance = 0.7f };
        public static readonly SoundStyle ChewSound = new("Spooky/Content/Sounds/EggEvent/GooSlugChew", SoundType.Sound) { Volume = 0.4f, PitchVariance = 0.5f };
		public static readonly SoundStyle SpitSound = new("Spooky/Content/Sounds/EggEvent/GooSlugSpit", SoundType.Sound) { Volume = 0.4f, PitchVariance = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 380;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.width = 54;
			NPC.height = 96;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.Crawdad;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.GooSlug"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/GooSlugGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);
            
            NPC.frameCounter++;

            //normal animations
            if (NPC.localAI[0] == 0)
            {
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping frame
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                //falling frame
                if (NPC.velocity.Y > 0)
                {
                    NPC.frame.Y = 9 * frameHeight;
                }
            }
            //eating animation
            else
            {
                if (NPC.frameCounter > 2)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 10)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (NPC.alpha == 255)
            {
                NPC.alpha = 0;
            }

            if (NPC.Hitbox.Intersects(player.Hitbox) && !player.GetModPlayer<SpookyPlayer>().EatenByGooSlug && player.GetModPlayer<SpookyPlayer>().GooSlugEatCooldown <= 0)
            {
				if (NPC.localAI[0] == 0)
				{
                    SoundEngine.PlaySound(ChompSound, NPC.Center);

					SaveDirection = NPC.direction;

					NPC.localAI[0]++;

                    NPC.netUpdate = true;
				}
			}
			else
			{
				NPC.spriteDirection = NPC.direction;

				NPC.aiStyle = 3;
			}

			if (NPC.localAI[0] > 0)
            {
				NPC.aiStyle = 0;

				NPC.spriteDirection = SaveDirection;

				player.position = NPC.Center - new Vector2(player.width / 2 + (NPC.spriteDirection == 1 ? -25 : 25), 0);

				//set player to eaten so that it hides them
                player.GetModPlayer<SpookyPlayer>().EatenByGooSlug = true;
                player.GetModPlayer<SpookyPlayer>().GooSlugEatCooldown = 180;

				//set frame to eating animation
				if (NPC.localAI[1] == 0)
				{
					CurrentFrameX = 1;

					NPC.frame.Y = 0;

					NPC.localAI[1]++;

                    NPC.netUpdate = true;
				}

				NPC.localAI[2]++;

                if (NPC.localAI[2] % 30 == 0)
                {
                    SoundEngine.PlaySound(ChewSound, NPC.Center);
                }

				//reset all variables
				if (NPC.localAI[2] >= 160 || !player.active || player.dead)
				{
					SoundEngine.PlaySound(SpitSound, NPC.Center);

					player.velocity.X = NPC.spriteDirection == -1 ? -12 : 12;
					player.velocity.Y = Main.rand.Next(-10, -5);
					player.immuneTime += 60; //give the player immune time when being spit out

					CurrentFrameX = 0;

					NPC.localAI[0] = 0;
					NPC.localAI[1] = 0;
					NPC.localAI[2] = 0;

                    NPC.netUpdate = true;
				}
            }
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GooChompers>(), 40));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/GooSlugGore" + Main.rand.Next(1, 3)).Type);
                    }
                }

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/GooSlugEyeGore").Type);
                    }
                }

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/GooSlugBoneGore").Type);
                }
            }
        }
    }
}