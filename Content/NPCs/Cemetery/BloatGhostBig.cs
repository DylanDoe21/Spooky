using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Cemetery
{
	public class BloatGhostBig : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;

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
            NPC.lifeMax = 120;
            NPC.damage = 20;
			NPC.defense = 0;
			NPC.width = 82;
			NPC.height = 66;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BloatGhost"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = new(tex.Width * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int numEffect = 0; numEffect < 4; numEffect++)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

                Color newColor = color;
                newColor = NPC.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(NPC.Center.X - 1, NPC.Center.Y) + (numEffect / 4 * 6f + NPC.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 3) * numEffect;
                Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, drawOrigin, NPC.scale * 1.035f, effects, 0);
            }
            
            return true;
		}
        
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 2)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
    
            NPC.velocity *= 0;

            if (player.Distance(NPC.Center) >= 300f)
            {
                NPC.localAI[0]++;

                if (NPC.localAI[0] > 20)
                {
                    SoundEngine.PlaySound(SoundID.GlommerBounce, NPC.Center);

                    int SmallGhost = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BloatGhostSmall>());
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: SmallGhost);
                    }

                    NPC.active = false;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                    Main.dust[dustGore].color = Color.Red;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
	}
}