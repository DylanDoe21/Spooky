using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class YellowSpore : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 24;
			Item.height = 24;
			Item.shoot = ModContent.ProjectileType<ShroomHopperPet>();
			Item.buffType = ModContent.BuffType<ShroomHopperBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}