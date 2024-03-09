using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class PhantomRollingSkull : ModNPC  
    {
        bool hasCollidedWithWall = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 35;
            NPC.defense = 5;
            NPC.width = 60;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit49;
			NPC.DeathSound = SoundID.NPCDeath33;
            NPC.aiStyle = 26;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

            var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
            Color newColor1 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Cyan);
            Color newColor2 = new Color(200 - NPC.alpha, 200 - NPC.alpha, 200 - NPC.alpha, 0).MultiplyRGBA(Color.Purple);

            for (int repeats = 0; repeats < 4; repeats++)
            {
                Color color = newColor1;
                color = NPC.GetAlpha(color);
                color *= 1f - fade;
                Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                Main.spriteBatch.Draw(texture, afterImagePosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.05f, effects, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, newColor2, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);

            return false;
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.rotation += 0.05f * (float)NPC.direction + (NPC.velocity.X / 40);

            //only run screenshake code if the player is close enough
            if (player.Distance(NPC.Center) < 250f)
            {
                //shake the screen if the skull is rolling at maximum speed
                if ((NPC.velocity.X >= 6 || NPC.velocity.X <= -6) && player.velocity.Y == 0 && Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                {
                    hasCollidedWithWall = false;

                    SpookyPlayer.ScreenShakeAmount = 3;
                }

                //collide with walls and play a sound
                if (!hasCollidedWithWall && (NPC.oldVelocity.X >= 5 || NPC.oldVelocity.X <= -5) && NPC.collideX)
                {   
                    SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.35f }, NPC.Center);

                    SpookyPlayer.ScreenShakeAmount = 10;

                    //set timer to slow down the npc after hitting a wall
                    NPC.localAI[0] = 60;

                    //set velocity to zero
                    NPC.velocity *= 0;

                    hasCollidedWithWall = true;
                }
            }

            if (NPC.localAI[0] > 0)
            {
                NPC.localAI[0]--;

                NPC.velocity.X *= 0.2f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 30; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                    Main.dust[dustGore].color = Main.rand.NextBool() ? Color.Cyan : Color.Purple;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 3f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
    }
}