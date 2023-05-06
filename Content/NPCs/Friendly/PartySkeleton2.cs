using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Friendly
{
    public class PartySkeleton2 : ModNPC  
    {
        bool hasDrink = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.defense = 5;
            NPC.width = 20;
			NPC.height = 80;
            NPC.friendly = true;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override bool CanChat() 
        {
			return true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (hasDrink)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/PartySkeleton2Cup").Value;

                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.RaveyardBiome>()))
                {
                    return 12f;
                }
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter += 1;
            //running animation
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0 || NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //still frame
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                "HAHAHAHAH! You look really funny, you must not be from around here. You don't smell like you've partied before.",
				"You look pretty spooky. Also, that giant clown behind you is really funny!"
			};

			return Main.rand.Next(Dialogue);
		}
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] == 1)
            {
                string[] names = { "Bonehead ", "Skull", "Grug" };
                NPC.GivenName = Main.rand.Next(names);

                if (Main.rand.NextBool(12))
                {
                    hasDrink = true;
                }
            }

            if (!Main.LocalPlayer.InModBiome(ModContent.GetInstance<Biomes.RaveyardBiome>()))
            {
                NPC.alpha += 5;

                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
            }
        }
    }
}