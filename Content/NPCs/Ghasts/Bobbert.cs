/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Ghasts
{
    public class Bobbert : ModNPC  
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bobbert");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.damage = 45;
            NPC.width = 40;
			NPC.height = 38;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit36;
			NPC.DeathSound = SoundID.NPCDeath39;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("Bobbert may look like a small, useless spirit, but his ghastly explosions are incredibly dangerous.")
			});
		}
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightSkyBlue);

            for (int numEffect = 0; numEffect < 4; numEffect++)
            {
                Color newColor = color;
                newColor = NPC.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (numEffect / 4 * 6.28318548f + NPC.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * numEffect;

                Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, drawOrigin, NPC.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Lerp(new Color(0, 255, 235), new Color(0, 174, 255), fade), new Color(0, 84, 255), fade);

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, color, NPC.rotation, 
            NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.04f;

            int MaxSpeed = 35;

            //flies to players X position
            if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed) 
            {
                MoveSpeedX--;
            }
            else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed)
            {
                MoveSpeedX++;
            }

            NPC.velocity.X = MoveSpeedX * 0.1f;
            
            //flies to players Y position
            if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -MaxSpeed)
            {
                MoveSpeedY--;
            }
            else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= MaxSpeed)
            {
                MoveSpeedY++;
            }

            NPC.velocity.Y = MoveSpeedY * 0.1f;
        }

        public override bool CheckDead() 
		{
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 0, 
                ModContent.ProjectileType<BobbertExplosion>(), NPC.damage / 3, 0f, Main.myPlayer, 0f, 0f);
            }

            return true;
		}
    }
}
*/