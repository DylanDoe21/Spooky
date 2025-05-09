using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Buffs.Debuff
{
	public class SmokerCoughDebuff : ModBuff
	{
        public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (Main.rand.NextBool(100))
			{
                SoundEngine.PlaySound(SoundID.NPCHit27 with { Pitch = -1f }, npc.Center);

				Projectile.NewProjectile(null, npc.Center, Vector2.Zero, ModContent.ProjectileType<CoughSmokeCloudSmall>(), 35, 0f, Main.myPlayer);
			}
        }
    }
}
