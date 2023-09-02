using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Costume;
using Spooky.Content.Projectiles.SpookyHell;
using Mono.Cecil;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Spooky.Core
{
    public class ItemGlobal : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.HasBuff(ModContent.BuffType<CatacombDebuff>()))
            {
                int[] Torches = { 8, 430, 432, 427, 429, 428, 1245, 431, 974, 3114, 3004, 2274, 433, 523, 1333, 3045, 4383, 4384, 4385, 4386, 4387, 4388, 5293, 5353 };

                //disable tools, block placement, and the rod of discord
                if (item.pick > 0 || item.hammer > 0 || item.axe > 0 || (item.createTile > 0 && !Torches.Contains(item.type)) || 
                item.type == ItemID.RodofDiscord || item.type == ItemID.Clentaminator || item.type == ItemID.Clentaminator2)
                {
                    return false;
                }

                //disable the use of any explosives
                int[] Explosives = { 166, 3196, 3115, 3547, 4908, 4827, 167, 4826, 4825, 4423, 235, 4909, 2896, 4824 };

                if (Explosives.Contains(item.type))
                {
                    return false;
                }

                //disable the use of buckets
                int[] LiquidItems = { 205, 206, 207, 1128, 3031, 4820, 5302, 5364 };

                if (LiquidItems.Contains(item.type))
                {
                    return false;
                }
            }

            return base.CanUseItem(item, player);
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (player.GetModPlayer<SpookyPlayer>().PandoraChalice)
            {
                if (item.type == ItemID.LesserManaPotion)
                {

                }

                if (item.type == ItemID.ManaPotion)
                {

                }

                if (item.type == ItemID.GreaterManaPotion)
                {

                }

                if (item.type == ItemID.SuperManaPotion)
                {

                }
            }

            return base.UseItem(item, player);
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<SpookyPlayer>().MocoNose && player.HasBuff(ModContent.BuffType<BoogerFrenzyBuff>()) && !player.HasBuff(ModContent.BuffType<BoogerFrenzyCooldown>()))
            {
                //whips and summon weapons should not shoot out a booger, as well as items that do not shoot projectiles
                if (item.DamageType != DamageClass.SummonMeleeSpeed && item.DamageType != DamageClass.Summon && item.shoot > 0 && item.shootSpeed > 0)
                {
                    int newProjectile = Projectile.NewProjectile(source, position, velocity * 1.5f, ModContent.ProjectileType<BlasterBoogerSmall>(), (int)knockback, player.whoAmI);
                    Main.projectile[newProjectile].DamageType = item.DamageType;
                }
            }

            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
		{
			if (item.type == ItemID.GoodieBag)
			{
				int[] DevMasks = new int[] { 
                ModContent.ItemType<BananalizardHead>(), 
                ModContent.ItemType<DylanDoeHead>(), 
                ModContent.ItemType<HatHead>(), 
                ModContent.ItemType<KrakenHead>(),
                ModContent.ItemType<PelusaHead>(),
                ModContent.ItemType<SeasaltHead>(), 
                ModContent.ItemType<TacoHead>(), 
                ModContent.ItemType<WaasephiHead>() };

                itemLoot.Add(ItemDropRule.OneFromOptions(30, DevMasks));
			}
		}
	}
}