using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientShootius : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 30;
			Item.DamageType = DamageClass.Summon;
			Item.autoReuse = true;
            Item.width = 40;
            Item.height = 50;
            Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 8);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }
    }
}