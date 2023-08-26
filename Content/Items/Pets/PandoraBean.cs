using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class PandoraBean : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 32;
			Item.height = 34;
			Item.shoot = ModContent.ProjectileType<PandoraBeanPet>();
			Item.buffType = ModContent.BuffType<PandoraBeanBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}