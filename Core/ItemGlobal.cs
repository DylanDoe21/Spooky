using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Costume;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Tiles.MusicBox;

namespace Spooky.Core
{
    public class ItemGlobal : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }

            //manually handle daffodils music box if her intro themes are playing since music boxes cant be assigned more than one song
            if (item.type == ItemID.MusicBox && Main.rand.NextBool(540) && 
            (Main.curMusic == MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/DaffodilWithIntro1") ||
            Main.curMusic == MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/DaffodilWithIntro2")))
            {
                SoundEngine.PlaySound(SoundID.Item166, player.Center);
                item.ChangeItemType(ModContent.ItemType<DaffodilBox>());
            }

            base.UpdateAccessory(item, player, hideVisual);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (player.HasBuff(ModContent.BuffType<CatacombDebuff>()))
            {
                int[] Torches = { 8, 430, 432, 427, 429, 428, 1245, 431, 974, 3114, 3004, 2274, 433, 523, 1333, 3045, 4383, 4384, 4385, 4386, 4387, 4388, 5293, 5353 };

                //disable tools, block placement, and the rod of discord
                if (item.pick > 0 || item.hammer > 0 || item.axe > 0 || (item.createTile > 0 && !Torches.Contains(item.type)) || 
                item.type is ItemID.RodofDiscord or ItemID.Clentaminator or ItemID.Clentaminator2)
                {
                    return false;
                }

                //disable the use of any explosives
                int[] Explosives = { 166, 3196, 3115, 3547, 4908, 4827, 167, 4826, 4825, 4423, 235, 4909, 2896, 4824 };

                if (Explosives.Contains(item.type))
                {
                    return false;
                }

                //disable the use of any wire items
                int[] WireTools = { 509, 850, 851, 3612, 3625, 3611, 510 };

                if (WireTools.Contains(item.type))
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