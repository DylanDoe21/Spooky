using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb
{
    public class HauntedSkull : ModNPC  
    {
        public static readonly SoundStyle ChargeSound = new("Spooky/Content/Sounds/Catacomb/HauntedSkull", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
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
            NPC.lifeMax = Main.hardMode ? 150 : 50;
            NPC.damage = Main.hardMode ? 60 : 30;
            NPC.defense = Main.hardMode ? 20 : 10;
            NPC.width = 30;
			NPC.height = 38;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 10;
			AIType = NPCID.CursedSkull;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HauntedSkull"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome>()))
            {
                return 20f;
            }

            return 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

            Color newColor;

            if (NPC.localAI[0] < 420)
            {
                newColor = Color.Green;
            }
            else
            {
                newColor = Color.Red;
            }

            for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
                Color color = NPC.GetAlpha(newColor) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
                spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
            }
            
            return true;
		}

        public override void FindFrame(int frameHeight)
        {   
            //charging frame
            if (NPC.localAI[0] < 420)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else 
            {
                NPC.frame.Y = 1 * frameHeight;
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] >= 420)
            {
                NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                NPC.rotation = NPC.velocity.ToRotation();

                if (NPC.spriteDirection == -1)
                {
                    NPC.rotation += MathHelper.Pi;
                }
            }

            if (NPC.localAI[0] == 420)
            {
                NPC.velocity *= 0.2f;

                Vector2 ChargeDirection = player.Center - NPC.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= -5;
                ChargeDirection.Y *= -5;  
                NPC.velocity.X = ChargeDirection.X;
                NPC.velocity.Y = ChargeDirection.Y;
            }

            if (NPC.localAI[0] == 440)
            {
                SoundEngine.PlaySound(ChargeSound, NPC.Center);

                Vector2 ChargeDirection = player.Center - NPC.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= 12;
                ChargeDirection.Y *= 12;  
                NPC.velocity.X = ChargeDirection.X;
                NPC.velocity.Y = ChargeDirection.Y;
            }

            if (NPC.localAI[0] >= 480)
            {
                NPC.velocity *= 0.97f;
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HauntedSkullGore").Type);
            }
        }
    }
}